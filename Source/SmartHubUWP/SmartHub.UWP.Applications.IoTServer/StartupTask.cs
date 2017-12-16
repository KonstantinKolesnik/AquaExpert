using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Infrastructure;
using Windows.ApplicationModel.Background;

namespace SmartHub.UWP.Applications.IoTServer
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral deferral;
        private Hub hub;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            //var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;

            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            StartHub();

            while (true) ;
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

            StopHub();

            deferral.Complete();
        }

        private void StartHub()
        {
            if (hub == null)
            {
                var assemblies = CoreUtils.GetSatelliteAssemblies(file => file.FileType == ".dll" && file.DisplayName.ToLower().StartsWith("smarthub"));

                hub = new Hub();
                hub.Init(assemblies);
                hub.StartServices();
            }
        }
        private void StopHub()
        {
            if (hub != null)
            {
                hub.StopServices();
                hub = null;
            }
        }
    }
}
