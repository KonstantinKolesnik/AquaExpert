using SmartHub.UWP.Core;
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
        #endregion

        #region Properties
        public static AppShell Current
        {
            get; private set;
        }
        public Frame AppFrame => appShellFrame;

        new public ElementTheme RequestedTheme
        {
            get { return Utils.FindFirstVisualChild<Grid>(this).RequestedTheme; }
            set { Utils.FindFirstVisualChild<Grid>(this).RequestedTheme = value; }
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
        private void AppFrame_Navigated(object sender, NavigationEventArgs e)
        {
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
