using SmartHub.UWP.Applications.Server.BackgroundTask;
using SmartHub.UWP.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SmartHub.UWP.Applications.Server
{
    public sealed partial class MainPage : Page
    {
        private const string taskName = "Smart Hub Server";
        private IBackgroundTaskRegistration task = null;

        public MainPage()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await CheckBackgroundTask();
        }

        private async Task CheckBackgroundTask()
        {
            IBackgroundTrigger trigger;
            trigger = new ApplicationTrigger();
            //trigger = new SocketActivityTrigger();
            //trigger = new SystemTrigger(SystemTriggerType.TimeZoneChange, false);
            //trigger = new SystemTrigger(SystemTriggerType.PowerStateChange, true);
            //trigger = new SystemTrigger(SystemTriggerType.SessionConnected, true);

            // check if task is already registered:
            task = BackgroundTaskRegistration.AllTasks.Values.FirstOrDefault(t => t.Name == taskName);

            //!!!!!!!
            //if (task != null)
            //    task.Unregister(true);

            // if not, register a new task:
            if (task == null)
            {
                var access = await BackgroundExecutionManager.RequestAccessAsync();

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
                await (trigger as ApplicationTrigger).RequestAsync();
            }
        }
        private void Task_Completed(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            //var settings = ApplicationData.Current.LocalSettings;
            //var key = task.TaskId.ToString();
            //var message = settings.Values[key].ToString();
            //UpdateUI(message);

            Utils.ShowToast(ToastTemplateType.ToastText02, "Background " + task.Name + " completed");
        }
    }
}
