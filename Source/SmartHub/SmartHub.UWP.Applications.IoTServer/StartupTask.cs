using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Infrastructure;
using System;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using Windows.UI.Notifications;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace SmartHub.UWP.Applications.IoTServer
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskCancellationReason cancelReason = BackgroundTaskCancellationReason.Abort;
        private volatile bool cancelRequested = false;
        private ThreadPoolTimer timer = null;
        private BackgroundTaskDeferral deferral = null;
        private IBackgroundTaskInstance taskInstance = null;
        private Hub hub;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // 
            // TODO: Insert code to perform background work
            //
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //

            //Utils.ShowToast(ToastTemplateType.ToastText02, "Background " + taskInstance.Task.Name + " Starting...");

            var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;

            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            deferral = taskInstance.GetDeferral();
            this.taskInstance = taskInstance;

            timer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallback), TimeSpan.FromSeconds(5));

            RunServer();

            while (true) ;
        }

        private void RunServer()
        {
            if (hub == null)
            {
                var assemblies = Utils.GetSatelliteAssemblies(file => file.FileType == ".dll" && file.DisplayName.ToLower().StartsWith("smarthub"));

                hub = new Hub();
                hub.Init(assemblies);
                hub.StartServices();
            }
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //Utils.ShowToast(ToastTemplateType.ToastText02, "Background " + sender.Task.Name + " Cancel Requested...");

            cancelRequested = true;
            cancelReason = reason;
        }
        private void PeriodicTimerCallback(ThreadPoolTimer timer)
        {
            if (!cancelRequested)// && _progress < 100)
            {
                //Utils.ShowToast(ToastTemplateType.ToastText02, "Background " + taskInstance.Task.Name + " is running");

                //_progress += 10;
                //taskInstance.Progress = _progress;
            }
            else
            {
                //Utils.ShowToast(ToastTemplateType.ToastText02, "Background " + taskInstance.Task.Name + taskStatus);

                timer.Cancel();

                //var key = taskInstance.Task.Name;

                // Record that this background task ran.
                //var taskStatus = (_progress < 100) ? "Canceled with reason: " + _cancelReason.ToString() : "Completed";
                var taskStatus = " Canceled with reason: " + cancelReason.ToString();
                //var settings = ApplicationData.Current.LocalSettings;
                //settings.Values[key] = taskStatus;

                if (hub != null)
                {
                    hub.StopServices();
                    hub = null;
                }


                // Indicate that the background task has completed.
                deferral.Complete();
            }
        }
    }
}
