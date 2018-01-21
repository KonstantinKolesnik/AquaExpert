using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Infrastructure;
using SmartHub.UWP.Plugins.Speech;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Media.Playback;
using Windows.System.Threading;

namespace SmartHub.UWP.Applications.IoTServer
{
    public sealed class StartupTask : IBackgroundTask
    {
        #region Fields
        private BackgroundTaskDeferral deferral;
        private Hub hub;
        //private ThreadPoolTimer timer;
        #endregion

        #region Task
        public /*async*/ void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            //var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;

            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            StartHub();
            //timer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(Timer_Tick), TimeSpan.FromMilliseconds(5000));

            //await ThreadPool.RunAsync(workItem => {
            //    //RestWebServer restWebServer = new RestWebServer(80);
            //    //try
            //    //{
            //    //    // initialize webserver
            //    //    restWebServer.RegisterController<Controller.Home.Home>();
            //    //    restWebServer.RegisterController<Controller.PhilipsHUE.Main>();
            //    //    await restWebServer.StartServerAsync();
            //    //}
            //    //catch (Exception ex)
            //    //{
            //    //    Log.e(ex);
            //    //    restWebServer.StopServer();
            //    //    Deferral.Complete();
            //    //}

            //    StartHub();
            //    hub?.Context.GetPlugin<SpeechPlugin>()?.Say(DateTime.Now.ToString());

            //}, WorkItemPriority.Normal);
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

            deferral?.Complete();
        }
        #endregion

        //private void Timer_Tick(ThreadPoolTimer timer)
        //{
        //    hub?.Context?.GetPlugin<SpeechPlugin>()?.Say(DateTime.Now.ToString());
        //}

        #region Hub
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
            hub?.StopServices();
            hub = null;
        }
        #endregion
    }
}
