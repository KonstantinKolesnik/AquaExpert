using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Communication.Stream;
using SmartHub.UWP.Plugins.UI.Attributes;
using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SmartHub.UWP.Applications.Server
{
    public sealed partial class AppShell : Page
    {
        #region Fields
        private RadioButton checkedMenuItem = null;
        private EventHandler<BackRequestedEventArgs> primaryBackRequestHandler = null;
        private StreamClient apiClient = new StreamClient();
        #endregion

        #region Properties
        public static AppShell Current
        {
            get; private set;
        }
        public Frame AppFrame => appShellFrame;
        public StreamClient ApiClient => apiClient;

        public AppSectionItemAttribute SelectedAppSectionItem
        {
            get; set;
        }
        #endregion

        #region Constructor
        public AppShell()
        {
            InitializeComponent();
            Current = this;

            AppFrame.CacheSize = 1; // TODO: change this value to a cache size that is appropriate for your application

            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            //DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
        }
        #endregion

        #region Public methods
        public void SetNavigationInfo(string title = null, string menuItemNameToSelect = null)
        {
            if (!string.IsNullOrEmpty(title))
                tbTitle.Text = Utils.GetLabelValue(title).ToUpper();

            if (!string.IsNullOrEmpty(menuItemNameToSelect))
            {
                var ctrl = FindName(menuItemNameToSelect);
                if (ctrl != null)
                {
                    var rb = ctrl as RadioButton;
                    if (rb != null)
                    {
                        rb.IsChecked = true;
                        checkedMenuItem = rb;
                    }
                }
            }
        }
        public void SetPrimaryBackRequestHandler(EventHandler<BackRequestedEventArgs> handler)
        {
            primaryBackRequestHandler = handler;
        }
        #endregion

        #region Event handlers
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            primaryBackRequestHandler?.Invoke(sender, e);

            bool handled = e.Handled;

            if (AppFrame != null)
            {
                // Check to see if this is the top-most page on the app back stack.
                if (AppFrame.CanGoBack)
                {
                    if (!handled)
                    {
                        // If not, set the event to handled and go back to the previous page in the app.
                        handled = true;
                        AppFrame.GoBack();
                    }
                }
            }

            e.Handled = handled;
        }
        //private async void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        //{
        //    if (profilesForSharing.Count > 0)
        //    {
        //        var deferral = e.Request.GetDeferral();

        //        var file = await StorageFile.CreateStreamedFileAsync("EcosCab.ecoscabprofiles", StreamedFileDataRequestedHandler, null);
        //        List<StorageFile> files = new List<StorageFile>() { file };

        //        e.Request.Data.Properties.Title = $"{AppManager.AppName} {Labels.Profiles}";
        //        //e.Request.Data.Properties.Description = "A demonstration on how to share";
        //        e.Request.Data.Properties.ApplicationName = AppManager.AppName;
        //        //e.Request.Data.Properties.ContentSourceApplicationLink = new Uri("ms-sdk-sharesourcecs:navigate?page=" + GetType().Name);
        //        e.Request.Data.SetStorageItems(files);

        //        deferral.Complete();
        //    }
        //    else
        //        e.Request.FailWithDisplayText(Labels.NoData);
        //}

        //private void StreamedFileDataRequestedHandler(StreamedFileDataRequest request)
        //{
        //    try
        //    {
        //        using (var stream = request.AsStreamForWrite())
        //        using (var streamWriter = new StreamWriter(stream))
        //            streamWriter.Write(profilesForSharing.ToJson());

        //        request.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        request.FailAndClose(StreamedFileFailureMode.Incomplete);
        //    }
        //}

        private void btnHamburger_Click(object sender, RoutedEventArgs e)
        {
            menu.IsPaneOpen = !menu.IsPaneOpen;
        }
        private void menuItem_Click(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;

            if (rb != null && rb != checkedMenuItem)
            {
                switch (rb.Name)
                {
                    case "menuDashboard": AppFrame.Navigate(typeof(DashboardPage)); break;
                    case "menuApplications": AppFrame.Navigate(typeof(AppSectionPage), AppSectionType.Applications); break;
                    case "menuSystem": AppFrame.Navigate(typeof(AppSectionPage), AppSectionType.System); break;
                    case "menuSettings": AppFrame.Navigate(typeof(SettingsPage)); break;
                    case "menuAbout": AppFrame.Navigate(typeof(AboutPage)); break;
                }
            }
        }

        //private bool isBackFromDialog = false;
        //private async void AppFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        //{
        //    if (e.SourcePageType == typeof(ProfilesPage) && AppManager.EcosApp.ActiveProfile != null)
        //    {
        //        if (!isBackFromDialog)
        //        {
        //            e.Cancel = true;
        //            await Utils.MessageBoxYesNo(Labels.confirmTerminateSession,
        //                (onYes) =>
        //                {
        //                    isBackFromDialog = true;
        //                    AppFrame.Navigate(typeof(ProfilesPage));
        //                },
        //                (onNo) =>
        //                {
        //                    if (checkedMenuItem != null)
        //                        checkedMenuItem.IsChecked = true;
        //                }
        //            );
        //        }
        //        else
        //        {
        //            isBackFromDialog = false;
        //            e.Cancel = false;
        //        }
        //    }
        //}
        private async void AppFrame_Navigated(object sender, NavigationEventArgs e)
        {
            await apiClient.StartAsync(AppManager.RemoteUrl, AppManager.RemoteServiceName);

            if (menu.DisplayMode != SplitViewDisplayMode.CompactInline)
                menu.IsPaneOpen = false;

            // clear frame stack from previous duplicates to the end
            //if (e.NavigationMode != NavigationMode.Back)
            {
                int idx = -1;

                foreach (var pse in AppFrame.BackStack)
                    if (pse.SourcePageType == e.SourcePageType)
                    {
                        idx = AppFrame.BackStack.IndexOf(pse);
                        break;
                    }

                if (idx != -1)
                {
                    int n = AppFrame.BackStack.Count - idx;
                    for (int i = 0; i < n; i++)
                        AppFrame.BackStack.RemoveAt(idx);
                }
            }

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = ((Frame) sender).CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }
        private async void AppFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            await Utils.MessageBox("Failed to load Page " + e.SourcePageType.FullName);
            e.Handled = true;
        }
        #endregion
    }
}
