using SmartHub.UWP.Core;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace SmartHub.UWP.Applications.Server.BackgroundTask
{
    public sealed class SmartHubServerBackgroundTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Utils.ShowToast(ToastTemplateType.ToastText02, "Smart Hub background task run");

            //var deferral = taskInstance.GetDeferral();

            //deferral.Complete();
        }
    }
}
