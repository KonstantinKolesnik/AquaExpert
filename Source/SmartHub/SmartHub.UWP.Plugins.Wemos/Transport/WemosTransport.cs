using SmartHub.UWP.Plugins.Wemos.Core;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SmartHub.UWP.Plugins.Wemos.Transport
{
    class WemosTransport
    {
        #region Fields
        private const string localService = "11111";
        private const string remoteService = "22222";
        private const string remoteMulticastAddress = "224.3.0.5";
        private const string remoteBroadcastAddress = "255.255.255.255";
        private DatagramSocket listenerSocket = null;
        private const string socketId = "ListenerSocket";
        private IBackgroundTaskRegistration task = null;
        #endregion

        #region Events
        public event WemosMessageEventHandler MessageReceived;
        #endregion

        #region Public methods
        public async Task Open()
        {
            if (listenerSocket == null)
            {
                listenerSocket = new DatagramSocket();
                listenerSocket.MessageReceived += DataReceived;

                listenerSocket.Control.DontFragment = true;

                // DatagramSockets conduct exclusive (SO_EXCLUSIVEADDRUSE) binds by default, effectively blocking
                // any other UDP socket on the system from binding to the same local port. This is done to prevent
                // other applications from eavesdropping or hijacking a DatagramSocket's unicast traffic.
                //
                // Setting the MulticastOnly control option to 'true' enables a DatagramSocket instance to share its
                // local port with any Win32 sockets that are bound using SO_REUSEADDR/SO_REUSE_MULTICASTPORT and
                // with any other DatagramSocket instances that have MulticastOnly set to true. However, note that any
                // attempt to use a multicast-only DatagramSocket instance to send or receive unicast data will result
                // in an exception being thrown.
                //
                // This control option is particularly useful when implementing a well-known multicast-based protocol,
                // such as mDNS and UPnP, since it enables a DatagramSocket instance to coexist with other applications
                // running on the system that also implement that protocol.
                listenerSocket.Control.MulticastOnly = true;

                //listenerSocket.TransferOwnership()

                // Start listen operation:
                try
                {
                    await listenerSocket.BindServiceNameAsync(localService);

                    //if (isMulticastSocket)
                    //{
                    // Join the multicast group to start receiving datagrams being sent to that group.
                    listenerSocket.JoinMulticastGroup(new HostName(remoteMulticastAddress));


                    //rootPage.NotifyUser("Listening on port " + listenerSocket.Information.LocalPort + " and joined to multicast group", NotifyType.StatusMessage);
                    //}
                    //else
                    //    rootPage.NotifyUser("Listening on port " + listenerSocket.Information.LocalPort, NotifyType.StatusMessage);

                    //Context.GetPlugin<SpeechPlugin>()?.Say("WEMOS UDP клиент запущен!");
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
        }
        public async Task Send(WemosMessage msg, bool isBrodcast)
        {
            if (msg != null)
            {
                if (isBrodcast)
                {
                    msg.NodeID = -1;
                    msg.LineID = -1;
                }

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
                    MessageReceived?.Invoke(this, new WemosMessageEventArgs(msg));
            }
            catch (Exception exception)
            {
                SocketErrorStatus socketError = SocketError.GetStatus(exception.HResult);

                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                    throw;

                //rootPage.NotifyUser("Error happened when receiving a datagram:" + exception.Message, NotifyType.ErrorMessage);
            }
        }
        #endregion
    }
}
