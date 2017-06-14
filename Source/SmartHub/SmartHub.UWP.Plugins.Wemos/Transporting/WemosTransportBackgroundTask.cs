using SmartHub.UWP.Core;
using System;
using System.Text;
using Windows.ApplicationModel.Background;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SmartHub.UWP.Plugins.Wemos.Transporting
{
    public sealed class WemosTransportBackgroundTask : IBackgroundTask
    {
        private const string socketId = "WemosTransportMulticastSocket";

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            try
            {
                var details = taskInstance.TriggerDetails as SocketActivityTriggerDetails;
                var socketInformation = details.SocketInformation;

                switch (details.Reason)
                {
                    case SocketActivityTriggerReason.SocketActivity:
                        var socket = socketInformation.DatagramSocket;
                        //DataReader reader = new DataReader(socket.InputStream);
                        //reader.InputStreamOptions = InputStreamOptions.Partial;
                        //await reader.LoadAsync(250);
                        //var dataString = reader.ReadString(reader.UnconsumedBufferLength);
                        ////ShowToast(dataString);
                        //socket.TransferOwnership(socketInformation.Id);
                        break;

                    case SocketActivityTriggerReason.KeepAliveTimerExpired:
                        socket = socketInformation.DatagramSocket;
                        //DataWriter writer = new DataWriter(socket.OutputStream);
                        //writer.WriteBytes(Encoding.UTF8.GetBytes("Keep alive"));
                        //await writer.StoreAsync();
                        //writer.DetachStream();
                        //writer.Dispose();
                        //socket.TransferOwnership(socketInformation.Id);
                        break;

                    case SocketActivityTriggerReason.SocketClosed:
                        socket = new DatagramSocket();
                        socket.EnableTransferOwnership(taskInstance.Task.TaskId, SocketActivityConnectedStandbyAction.Wake);
                        //if (ApplicationData.Current.LocalSettings.Values["hostname"] == null)
                        //{
                        //    break;
                        //}
                        //var hostname = (String) ApplicationData.Current.LocalSettings.Values["hostname"];
                        //var port = (String) ApplicationData.Current.LocalSettings.Values["port"];
                        //await socket.ConnectAsync(new HostName(hostname), port);
                        socket.TransferOwnership(socketId);
                        break;

                    default:
                        break;
                }

                deferral.Complete();
            }
            catch (Exception ex)
            {
                Utils.ShowToast(Windows.UI.Notifications.ToastTemplateType.ToastText02, ex.Message);
                deferral.Complete();
            }
        }
    }
}


//private const string socketId = "WemosTransportMulticastSocket";
//private const string socketBackgroundgTaskName = "WemosMulticastActivityBackgroundTask";
//private IBackgroundTaskRegistration task = null;

//private void CheckBackgroundTask()
//{
//    foreach (var current in BackgroundTaskRegistration.AllTasks)
//        if (current.Value.Name == socketBackgroundgTaskName)
//        {
//            task = current.Value;
//            break;
//        }

//    // if there is no task allready created, create a new one
//    if (task == null)
//    {
//        var taskBuilder = new BackgroundTaskBuilder();
//        taskBuilder.Name = socketBackgroundgTaskName;
//        taskBuilder.TaskEntryPoint = socketBackgroundgTaskName + ".SocketActivityTask";
//        taskBuilder.SetTrigger(new SocketActivityTrigger());
//        //taskBuilder.SetTrigger(new SystemTrigger(SystemTriggerType.TimeZoneChange, false));
//        //taskBuilder.AddCondition(new SystemCondition(SystemConditionType.UserPresent));

//        task = taskBuilder.Register();
//        //task.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
//    }
//}
//private async Task CheckSocketAsync()
//{
//    try
//    {
//        SocketActivityInformation socketInformation;
//        if (!SocketActivityInformation.AllSockets.TryGetValue(socketId, out socketInformation))
//        {
//            var socket = new DatagramSocket();
//            socket.Control.DontFragment = true;
//            socket.Control.MulticastOnly = true;
//            socket.EnableTransferOwnership(task.TaskId, SocketActivityConnectedStandbyAction.Wake);

//            await socket.BindServiceNameAsync(localService);
//            socket.JoinMulticastGroup(new HostName(remoteMulticastAddress));

//            // To demonstrate usage of CancelIOAsync async, have a pending read on the socket and call 
//            // cancel before transfering the socket. 
//            //DataReader reader = new DataReader(socket.InputStream);
//            //reader.InputStreamOptions = InputStreamOptions.Partial;
//            //var read = reader.LoadAsync(250);
//            //read.Completed += (info, status) =>
//            //{
//            //};
//            //await socket.CancelIOAsync();

//            socket.TransferOwnership(socketId);
//            socket = null;
//        }
//        //rootPage.NotifyUser("Connected. You may close the application", NotifyType.StatusMessage);
//    }
//    catch (Exception exception)
//    {
//        //rootPage.NotifyUser(exception.Message, NotifyType.ErrorMessage);
//    }
//}
