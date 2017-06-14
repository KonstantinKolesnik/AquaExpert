using SmartHub.UWP.Applications.Server.BackgroundTask;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml;
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

            //foreach (var task in BackgroundTaskRegistration.AllTasks)
            //{
            //    if (task.Value.Name == BackgroundTaskSample.ApplicationTriggerTaskName)
            //    {
            //        AttachProgressAndCompletedHandlers(task.Value);
            //        BackgroundTaskSample.UpdateBackgroundTaskRegistrationStatus(BackgroundTaskSample.ApplicationTriggerTaskName, true);
            //        break;
            //    }
            //}

            //trigger = new ApplicationTrigger();
            //UpdateUI();
        }

        private async Task CheckBackgroundTask()
        {
            var access = await BackgroundExecutionManager.RequestAccessAsync();
            //switch (access)
            //{
            //    case BackgroundAccessStatus.Unspecified:
            //        break;
            //    case BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity:
            //        break;
            //    case BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity:
            //        break;
            //    case BackgroundAccessStatus.Denied:
            //        break;
            //    default:
            //        break;
            //}
            //var b = access;


            return;


            // check if task is already registered:
            foreach (var t in BackgroundTaskRegistration.AllTasks)
                if (t.Value.Name == taskName)
                {
                    task = t.Value;
                    break;
                }

            // if not, register a new task:
            if (task == null)
            {
                var taskBuilder = new BackgroundTaskBuilder()
                {
                    Name = taskName,
                    TaskEntryPoint = typeof(SmartHubServerBackgroundTask).ToString()
                };

                //var trigger = new ApplicationTrigger();
                //var trigger = new SocketActivityTrigger();
                //var trigger = new SystemTrigger(SystemTriggerType.TimeZoneChange, false);
                //taskBuilder.SetTrigger(trigger);

                taskBuilder.AddCondition(new SystemCondition(SystemConditionType.UserPresent));
                //taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));

                task = taskBuilder.Register();
                task.Completed += new BackgroundTaskCompletedEventHandler(Task_Completed);

                //await trigger.RequestAsync(); // if ApplicationTrigger
            }
        }
        private void Task_Completed(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            //var settings = ApplicationData.Current.LocalSettings;
            //var key = task.TaskId.ToString();
            //var message = settings.Values[key].ToString();
            //UpdateUI(message);
        }
    }
}
