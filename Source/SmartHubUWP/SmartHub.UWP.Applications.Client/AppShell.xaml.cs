using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Xaml;
using SmartHub.UWP.Plugins.UI.Attributes;
using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SmartHub.UWP.Applications.Client
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
            get { return XamlUtils.FindFirstVisualChild<Grid>(this).RequestedTheme; }
            set { XamlUtils.FindFirstVisualChild<Grid>(this).RequestedTheme = value; }
        }
        #endregion

        #region Constructor
        public AppShell()
        {
            InitializeComponent();
            Current = this;

            AppFrame.CacheSize = 1; // TODO: change this value to a cache size that is appropriate for your application

            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
        }
        #endregion

        #region Public methods
        public void SetNavigationInfo(string title = null, string menuItemNameToSelect = null)
        {
            if (!string.IsNullOrEmpty(title))
                tbTitle.Text = XamlUtils.GetLocalizedString(AppManager.AppData.Language, title).ToUpper();

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
            if (sender is RadioButton rb && rb != checkedMenuItem)
                switch (rb.Name)
                {
                    case "menuDashboard": AppFrame.Navigate(typeof(DashboardPage)); break;
                    case "menuApplications": AppFrame.Navigate(typeof(AppSectionPage), AppSectionType.Applications); break;
                    case "menuSystem": AppFrame.Navigate(typeof(AppSectionPage), AppSectionType.System); break;
                    case "menuSettings": AppFrame.Navigate(typeof(SettingsPage)); break;
                    case "menuAbout": AppFrame.Navigate(typeof(AboutPage)); break;
                }
        }

        private void AppFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (menu.DisplayMode != SplitViewDisplayMode.CompactInline)
                menu.IsPaneOpen = false;

            // clear frame stack from previous duplicates to the end
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

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = ((Frame) sender).CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }
        private async void AppFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            await CoreUtils.MessageBox("Failed to load Page " + e.SourcePageType.FullName);
            e.Handled = true;
        }
        #endregion
    }
}
