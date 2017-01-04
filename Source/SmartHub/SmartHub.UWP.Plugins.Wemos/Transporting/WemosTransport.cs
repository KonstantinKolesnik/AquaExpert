using SmartHub.UWP.Plugins.Wemos.Core;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SmartHub.UWP.Plugins.Wemos.Transporting
{
    class WemosTransport
    {
        #region Fields
        private const string localService = "11111";
        private const string remoteService = "22222";
        private const string remoteMulticastAddress = "224.3.0.5";
        private const string remoteBroadcastAddress = "255.255.255.255";
        private DatagramSocket listenerSocket = null;
        private const string socketId = "WemosTransportMulticastSocket";
        private const string socketBackgroundgTaskName = "WemosMulticastActivityBackgroundTask";
        private IBackgroundTaskRegistration task = null;
        #endregion

        #region Events
        public event WemosMessageEventHandler MessageReceived;
        #endregion

        #region Public methods
        public async Task Open()
        {
            //CheckBackgroundTask();
            //await CheckSocketAsync();

            #region Simple socket creation
            if (listenerSocket == null)
            {
                listenerSocket = new DatagramSocket();
                listenerSocket.Control.DontFragment = true;
                listenerSocket.Control.MulticastOnly = true;
                listenerSocket.MessageReceived += DataReceived;

                try
                {
                    await listenerSocket.BindServiceNameAsync(localService);
                    listenerSocket.JoinMulticastGroup(new HostName(remoteMulticastAddress));
                }
                catch (Exception exception)
                {
                    Close();

                    // If this is an unknown status it means that the error is fatal and retry will likely fail.
                    if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                        throw;

                    //rootPage.NotifyUser("Start listening failed with error: " + exception.Message, NotifyType.ErrorMessage);
                }
            }
            #endregion
        }
        public async Task Send(WemosMessage msg)
        {
            if (msg != null)
            {
                var str = msg.ToDto();
                if (!string.IsNullOrEmpty(str))
                {
                    try
                    {
                        // GetOutputStreamAsync can be called multiple times on a single DatagramSocket instance to obtain
                        // IOutputStreams pointing to various different remote endpoints. The remote hostname given to
                        // GetOutputStreamAsync can be a unicast, multicast or broadcast address.
                        IOutputStream outputStream = await listenerSocket.GetOutputStreamAsync(new HostName(remoteMulticastAddress), remoteService);

                        DataWriter writer = new DataWriter(outputStream);
                        writer.WriteString(str);
                        await writer.StoreAsync();
                    }
                    catch (Exception exception)
                    {
                        // If this is an unknown status it means that the error is fatal and retry will likely fail.
                        if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                            throw;

                        //rootPage.NotifyUser("Send failed with error: " + exception.Message, NotifyType.ErrorMessage);
                    }
                }
            }
        }
        public void Close()
        {
            if (listenerSocket != null)
            {
                // DatagramSocket.Close() is exposed through the Dispose() method in C#.
                // The call below explicitly closes the socket, freeing the UDP port that it is currently bound to.
                listenerSocket.Dispose();
                listenerSocket = null;

                //Context.GetPlugin<SpeechPlugin>()?.Say("WEMOS UDP клиент остановлен");
            }
        }
        #endregion

        #region Private methods
        //private void NotifyUserFromAsyncThread(string strMessage, NotifyType type)
        //{
        //    var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => rootPage.NotifyUser(strMessage, type));
        //}
        private void DataReceived(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs eventArguments)
        {
            try
            {
                uint length = eventArguments.GetDataReader().UnconsumedBufferLength;
                string str = eventArguments.GetDataReader().ReadString(length);

                //NotifyUserFromAsyncThread("Received data from remote peer (Remote Address: " + eventArguments.RemoteAddress.CanonicalName + ", Remote Port: " + eventArguments.RemotePort + "): \"" + str + "\"", NotifyType.StatusMessage);

                foreach (var msg in WemosMessage.FromDto(str))
                    MessageReceived?.Invoke(this, new WemosMessageEventArgs(msg), eventArguments.RemoteAddress);
            }
            catch (Exception exception)
            {
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                    throw;

                //rootPage.NotifyUser("Error happened when receiving a datagram:" + exception.Message, NotifyType.ErrorMessage);
            }
        }

        private void CheckBackgroundTask()
        {
            foreach (var current in BackgroundTaskRegistration.AllTasks)
                if (current.Value.Name == socketBackgroundgTaskName)
                {
                    task = current.Value;
                    break;
                }

            // if there is no task allready created, create a new one
            if (task == null)
            {
                var taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = socketBackgroundgTaskName;
                taskBuilder.TaskEntryPoint = socketBackgroundgTaskName + ".SocketActivityTask";
                taskBuilder.SetTrigger(new SocketActivityTrigger());

                task = taskBuilder.Register();
            }
        }
        private async Task CheckSocketAsync()
        {
            try
            {
                SocketActivityInformation socketInformation;
                if (!SocketActivityInformation.AllSockets.TryGetValue(socketId, out socketInformation))
                {
                    var socket = new DatagramSocket();
                    socket.Control.DontFragment = true;
                    socket.Control.MulticastOnly = true;
                    socket.EnableTransferOwnership(task.TaskId, SocketActivityConnectedStandbyAction.Wake);

                    await socket.BindServiceNameAsync(localService);
                    socket.JoinMulticastGroup(new HostName(remoteMulticastAddress));

                    // To demonstrate usage of CancelIOAsync async, have a pending read on the socket and call 
                    // cancel before transfering the socket. 
                    //DataReader reader = new DataReader(socket.InputStream);
                    //reader.InputStreamOptions = InputStreamOptions.Partial;
                    //var read = reader.LoadAsync(250);
                    //read.Completed += (info, status) =>
                    //{
                    //};
                    //await socket.CancelIOAsync();

                    socket.TransferOwnership(socketId);
                    socket = null;
                }
                //rootPage.NotifyUser("Connected. You may close the application", NotifyType.StatusMessage);
            }
            catch (Exception exception)
            {
                //rootPage.NotifyUser(exception.Message, NotifyType.ErrorMessage);
            }
        }
        #endregion
    }
}
