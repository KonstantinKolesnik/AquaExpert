using SmartHub.UWP.Applications.Server.BackgroundTask;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using SmartHub.UWP.Core;
using Windows.UI.Notifications;

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
            // check if task is already registered:
            task = BackgroundTaskRegistration.AllTasks.Values.FirstOrDefault(t => t.Name == taskName);

            // if not, register a new task:
            if (task == null)
            {
                //task.Unregister(true);

                var access = await BackgroundExecutionManager.RequestAccessAsync();

                var taskBuilder = new BackgroundTaskBuilder()
                {
                    Name = taskName,
                    TaskEntryPoint = typeof(SmartHubServerBackgroundTask).ToString()
                };

                var trigger = new ApplicationTrigger();
                //trigger = new SocketActivityTrigger();
                //trigger = new SystemTrigger(SystemTriggerType.TimeZoneChange, false);
                //trigger = new SystemTrigger(SystemTriggerType.PowerStateChange, true);
                //trigger = new SystemTrigger(SystemTriggerType.SessionConnected, true);
                taskBuilder.SetTrigger(trigger);

                //taskBuilder.AddCondition(new SystemCondition(SystemConditionType.UserPresent));
                //taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));

                task = taskBuilder.Register();

                if (task != null)
                    await (trigger as ApplicationTrigger).RequestAsync();
            }

            if (task != null)
                task.Completed += new BackgroundTaskCompletedEventHandler(Task_Completed);
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
