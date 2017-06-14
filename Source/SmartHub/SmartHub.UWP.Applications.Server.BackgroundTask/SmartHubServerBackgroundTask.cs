using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace SmartHub.UWP.Applications.Server.BackgroundTask
{
    public sealed class SmartHubServerBackgroundTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            //SendToast("Hi this is background Task");

            //var deferral = taskInstance.GetDeferral();

            //deferral.Complete();
        }
    }
}
