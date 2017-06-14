using Microsoft.ApplicationInsights;
using SmartHub.UWP.Applications.Client.Common;
using SmartHub.UWP.Core;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace SmartHub.UWP.Applications.Client
{
    sealed partial class App : Application
    {
        public App()
        {
            WindowsAppInitializer.InitializeAsync();

            InitializeComponent();
            Suspending += OnSuspending;
            UnhandledException += App_UnhandledException;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            AppManager.Init();

            var shell = Window.Current.Content as AppShell;
            if (shell == null)
            {
                shell = new AppShell();

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                Window.Current.Content = shell;
            }

            if (!e.PrelaunchActivated)
            {
                if (shell.AppFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    shell.AppFrame.Navigate(typeof(DashboardPage), e.Arguments);
                }

                Window.Current.Activate();
            }

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(320, 200));
            LocalUtils.SetAppTheme();
        }
        //protected override void OnActivated(IActivatedEventArgs args)
        //{
        //    base.OnActivated(args);

        //    AppManager.Init();

        //    var rootFrame = Window.Current.Content as Frame;

        //    if (rootFrame == null)
        //    {
        //        rootFrame = new Frame();
        //        rootFrame.NavigationFailed += OnNavigationFailed;
        //        Window.Current.Content = rootFrame;
        //    }

        //    if (rootFrame.Content == null)
        //        rootFrame.Navigate(typeof(MainPage));

        //    Window.Current.Activate();
        //}

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);

            var taskInstance = args.TaskInstance;
            var appServiceDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += (s, e) => { appServiceDeferral.Complete(); };

            var appService = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            var appServiceConnection = appService.AppServiceConnection;
            appServiceConnection.RequestReceived += OnAppServiceRequestReceived;
            appServiceConnection.ServiceClosed += (s, e) => { appServiceDeferral.Complete(); };
        }
        private async void OnAppServiceRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            AppServiceDeferral messageDeferral = args.GetDeferral();

            ValueSet message = args.Request.Message;
            //string text = message["GetRootUI"] as string;
            //if (text == "Value")
            //{
            //    var returnMessage = new ValueSet();
            //    returnMessage.Add("Response", "True");

            //    await args.Request.SendResponseAsync(returnMessage);
            //}

            //object ui = null;
            //await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            //() =>
            //{
            //    ui =  new ucMainUI().ToJson();// AppManager.Hub.Context.GetPlugin<UIPlugin>().GetUI();
            //});


            //var returnMessage = new ValueSet();
            //returnMessage.Add("Response", ui);
            //await args.Request.SendResponseAsync(returnMessage);

            messageDeferral.Complete();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            //TODO: Save application state and stop any background activity
            AppManager.OnSuspending(deferral);

            deferral.Complete();
        }
        private async void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            await Utils.MessageBox(e.Message);
            e.Handled = true;
        }
    }
}
