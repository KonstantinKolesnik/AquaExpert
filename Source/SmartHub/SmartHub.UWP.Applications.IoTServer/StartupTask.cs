using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Infrastructure;
using Windows.ApplicationModel.Background;

namespace SmartHub.UWP.Applications.IoTServer
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral deferral = null;
        private Hub hub;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();

            //var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;

            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

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
            switch (reason)
            {
                case BackgroundTaskCancellationReason.Abort:
                    //app unregistered background task (amoung other reasons).
                    break;
                case BackgroundTaskCancellationReason.Terminating:
                    //system shutdown
                    break;
                case BackgroundTaskCancellationReason.ConditionLoss:
                    break;
                case BackgroundTaskCancellationReason.SystemPolicy:
                    break;
            }

            if (hub != null)
            {
                hub.StopServices();
                hub = null;
            }

            deferral.Complete();
        }
    }
}
