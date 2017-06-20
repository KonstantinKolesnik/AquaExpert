using SmartHub.UWP.Applications.Server.BackgroundTask;
using SmartHub.UWP.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SmartHub.UWP.Applications.Server
{
    public sealed partial class MainPage : Page
    {
        private const string taskName = "Smart Hub Server";
        private IBackgroundTaskRegistration task = null;

        private ThreadPoolTimer timer = null;
        //private System.Threading.Timer timer;

        private Core.Infrastructure.Hub hub;

        public MainPage()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            //await CheckBackgroundTask();
            RunServer();

            //timer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(async (t) =>
            //{
            //    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //    {
            //        //Window.Current.Activate();
            //        //Utils.ShowToast(ToastTemplateType.ToastText02, "Timer tick");
            //    });
            //}), TimeSpan.FromSeconds(5));





            //timer = new System.Threading.Timer(async (o) =>
            //{
            //    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            //    {
            //        Window.Current.Activate();
            //    });

            //}, null, 0, (int) TimeSpan.FromSeconds(5).TotalMilliseconds);
        }

        private void RunServer()
        {
            if (hub == null)
            {
                var assemblies = CoreUtils.GetSatelliteAssemblies(file => file.FileType == ".dll" && file.DisplayName.ToLower().StartsWith("smarthub"));

                hub = new Core.Infrastructure.Hub();
                hub.Init(assemblies);
                hub.StartServices();
            }
        }

        private async Task CheckBackgroundTask()
        {
            IBackgroundTrigger trigger;
            //trigger = new ApplicationTrigger();
            //trigger = new SocketActivityTrigger();
            //trigger = new SystemTrigger(SystemTriggerType.TimeZoneChange, false);
            //trigger = new SystemTrigger(SystemTriggerType.PowerStateChange, true);
            //trigger = new SystemTrigger(SystemTriggerType.SessionConnected, true);
            //trigger = new SystemTrigger(SystemTriggerType.UserPresent, true);
            //trigger = new SystemTrigger(SystemTriggerType.UserAway, true);
            //trigger = new SystemTrigger(SystemTriggerType.OnlineIdConnectedStateChange, true);
            trigger = new SystemTrigger(SystemTriggerType.DefaultSignInAccountChange, true);
            

            // check if task is already registered:
            task = BackgroundTaskRegistration.AllTasks.Values.FirstOrDefault(t => t.Name == taskName);

            //!!!!!!!
            if (task != null)
                task.Unregister(true);

            // if not, register a new task:
            //if (task == null)
            {
                var access = await BackgroundExecutionManager.RequestAccessAsync();
                if (access == BackgroundAccessStatus.DeniedByUser || access == BackgroundAccessStatus.DeniedBySystemPolicy || access == BackgroundAccessStatus.Unspecified)
                {
                    CoreUtils.ShowToast(ToastTemplateType.ToastText02, "Background access denied!");
                    return;
                }

                var taskBuilder = new BackgroundTaskBuilder()
                {
                    Name = taskName,
                    TaskEntryPoint = typeof(SmartHubServerBackgroundTask).ToString(),
                    IsNetworkRequested = true
                };

                taskBuilder.SetTrigger(trigger);

                //taskBuilder.AddCondition(new SystemCondition(SystemConditionType.UserPresent));
                //taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));

                task = taskBuilder.Register();
            }

            if (task != null)
            {
                task.Completed += new BackgroundTaskCompletedEventHandler(Task_Completed);
                //await (trigger as ApplicationTrigger).RequestAsync();
            }
        }
        private void Task_Completed(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            //var settings = ApplicationData.Current.LocalSettings;
            //var key = task.TaskId.ToString();
            //var message = settings.Values[key].ToString();
            //UpdateUI(message);

            CoreUtils.ShowToast(ToastTemplateType.ToastText02, "Background " + task.Name + " completed");
        }
    }
}
