using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using SmartHub.UWP.Core;
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

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // 
            // TODO: Insert code to perform background work
            //
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //

            Utils.ShowToast(ToastTemplateType.ToastText02, "Background " + taskInstance.Task.Name + " Starting...");

            var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;

            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            deferral = taskInstance.GetDeferral();
            this.taskInstance = taskInstance;

            timer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallback), TimeSpan.FromSeconds(5));
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            cancelRequested = true;
            cancelReason = reason;

            Utils.ShowToast(ToastTemplateType.ToastText02, "Background " + sender.Task.Name + " Cancel Requested...");
        }
        private void PeriodicTimerCallback(ThreadPoolTimer timer)
        {
            if (!cancelRequested)// && _progress < 100)
            {
                //_progress += 10;
                //taskInstance.Progress = _progress;

                Utils.ShowToast(ToastTemplateType.ToastText02, "Background " + taskInstance.Task.Name + " is running");
            }
            else
            {
                timer.Cancel();

                //var key = taskInstance.Task.Name;

                // Record that this background task ran.
                //var taskStatus = (_progress < 100) ? "Canceled with reason: " + _cancelReason.ToString() : "Completed";
                var taskStatus = " Canceled with reason: " + cancelReason.ToString();
                //var settings = ApplicationData.Current.LocalSettings;
                //settings.Values[key] = taskStatus;

                Utils.ShowToast(ToastTemplateType.ToastText02, "Background " + taskInstance.Task.Name + taskStatus);

                // Indicate that the background task has completed.
                deferral.Complete();
            }
        }
    }
}
