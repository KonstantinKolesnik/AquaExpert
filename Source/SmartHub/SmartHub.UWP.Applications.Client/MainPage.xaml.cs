using SmartHub.UWP.Core;
using Windows.ApplicationModel.AppService;
using Windows.UI.Xaml.Controls;

//using SDKTemplate;
using System;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Navigation;
using SmartHub.UWP.Plugins.UI.UI;
using Windows.UI.Xaml;

namespace SmartHub.UWP.Applications.Client
{
    public sealed partial class MainPage : Page
    {
        private AppServiceConnection connection;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            holder.Content = new ucMainUI();
        }


        private async void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            //Is a connection already open?
            if (connection != null)
            {
                await Utils.MessageBox("A connection already exists");
                return;
            }

            //Set up a new app service connection
            connection = new AppServiceConnection();
            connection.AppServiceName = "smarthubuwp.server";
            connection.PackageFamilyName = "07a5a158-235a-40cb-bf90-3be263fed556_g9kck1pvk5wcy";
            connection.ServiceClosed += Connection_ServiceClosed;

            var status = await connection.OpenAsync();

            //If the new connection opened successfully we're done here
            if (status == AppServiceConnectionStatus.Success)
            {
                await Utils.MessageBox("Connection is open");
            }
            else
            {
                //Something went wrong. Lets figure out what it was and show the 
                //user a meaningful message
                switch (status)
                {
                    case AppServiceConnectionStatus.AppNotInstalled:
                        await Utils.MessageBox("The app AppServicesProvider is not installed. Deploy AppServicesProvider to this device and try again.");
                        break;

                    case AppServiceConnectionStatus.AppUnavailable:
                        await Utils.MessageBox("The app AppServicesProvider is not available. This could be because it is currently being updated or was installed to a removable device that is no longer available.");
                        break;

                    case AppServiceConnectionStatus.AppServiceUnavailable:
                        await Utils.MessageBox(string.Format("The app AppServicesProvider is installed but it does not provide the app service {0}.", connection.AppServiceName));
                        break;

                    case AppServiceConnectionStatus.Unknown:
                        await Utils.MessageBox("An unkown error occurred while we were trying to open an AppServiceConnection.");
                        break;
                }

                //Clean up before we go
                connection.Dispose();
                connection = null;
            }
        }
        private async void ButtonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            //Is there an open connection?
            if (connection == null)
            {
                await Utils.MessageBox("There's no open connection to close");
                return;
            }

            //Close the open connection
            connection.Dispose();
            connection = null;

            //Let the user know we closed the connection
            await Utils.MessageBox("Connection is closed");
        }

        private async void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                //Dispose the connection reference we're holding
                if (connection != null)
                {
                    connection.Dispose();
                    connection = null;
                }
            });
        }

        private async void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            //Is there an open connection?
            if (connection == null)
            {
                await Utils.MessageBox("You need to open a connection before trying to generate a random number.");
                return;
            }

            //Send a message to the app service
            var inputs = new ValueSet();
            inputs.Add("GetRootUI", null);
            AppServiceResponse response = await connection.SendMessageAsync(inputs);

            //If the service responded display the message. We're done!
            if (response.Status == AppServiceResponseStatus.Success)
            {
                //if (!response.Message.ContainsKey("result"))
                //{
                //    rootPage.NotifyUser("The app service response message does not contain a key called \"result\"", NotifyType.StatusMessage);
                //    return;
                //}

                //var resultText = response.Message["result"].ToString();
                //if (!string.IsNullOrEmpty(resultText))
                //{
                //    Result.Text = resultText;
                //    rootPage.NotifyUser("App service responded with a result", NotifyType.StatusMessage);
                //}
                //else
                //{
                //    rootPage.NotifyUser("App service did not respond with a result", NotifyType.ErrorMessage);
                //}
            }
            else
            {
                //Something went wrong. Show the user a meaningful
                //message depending upon the status
                switch (response.Status)
                {
                    case AppServiceResponseStatus.Failure:
                        await Utils.MessageBox("The service failed to acknowledge the message we sent it. It may have been terminated because the client was suspended.");
                        break;

                    case AppServiceResponseStatus.ResourceLimitsExceeded:
                        await Utils.MessageBox("The service exceeded the resources allocated to it and had to be terminated.");
                        break;

                    case AppServiceResponseStatus.Unknown:
                    default:
                        await Utils.MessageBox("An unkown error occurred while we were trying to send a message to the service.");
                        break;
                }
            }
        }
    }
}
