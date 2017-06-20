using SmartHub.UWP.Core;
using System;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using Windows.UI.Notifications;

namespace SmartHub.UWP.Applications.Server.BackgroundTask
{
    public sealed class SmartHubServerBackgroundTask : IBackgroundTask
    {
        private BackgroundTaskCancellationReason cancelReason = BackgroundTaskCancellationReason.Abort;
        private volatile bool cancelRequested = false;
        private ThreadPoolTimer timer = null;
        private BackgroundTaskDeferral deferral = null;
        private IBackgroundTaskInstance taskInstance = null;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            CoreUtils.ShowToast(ToastTemplateType.ToastText02, "Background " + taskInstance.Task.Name + " Starting...");

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

            CoreUtils.ShowToast(ToastTemplateType.ToastText02, "Background " + sender.Task.Name + " Cancel Requested...");
        }
        private void PeriodicTimerCallback(ThreadPoolTimer timer)
        {
            if (!cancelRequested)// && _progress < 100)
            {
                //_progress += 10;
                //taskInstance.Progress = _progress;

                CoreUtils.ShowToast(ToastTemplateType.ToastText02, "Background " + taskInstance.Task.Name + " is running");
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

                CoreUtils.ShowToast(ToastTemplateType.ToastText02, "Background " + taskInstance.Task.Name + taskStatus);

                // Indicate that the background task has completed.
                deferral.Complete();
            }
        }
    }
}
