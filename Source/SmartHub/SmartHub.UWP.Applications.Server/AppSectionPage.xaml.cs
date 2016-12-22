using SmartHub.UWP.Core.Communication.Stream;
using SmartHub.UWP.Plugins.ApiListener;
using SmartHub.UWP.Plugins.UI.Attributes;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SmartHub.UWP.Applications.Server
{
    public sealed partial class AppSectionPage : Page
    {
        private bool isAppsSection;
        private StreamClient apiClient = new StreamClient();

        public AppSectionPage()
        {
            InitializeComponent();
        }

        #region Navigation
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            isAppsSection = (AppSectionType) (e.Parameter != null ? e.Parameter : CoreApplication.Properties[nameof(isAppsSection)]) == AppSectionType.Applications;
            CoreApplication.Properties[nameof(isAppsSection)] = isAppsSection;
            AppShell.Current.SetNavigationInfo(isAppsSection ? "Applications" : "System", isAppsSection ? "menuApplications" : "menuSystem");

            if (!CoreApplication.Properties.ContainsKey(isAppsSection ? "ApplicationItems" : "SystemItems"))
            {
                // request items
                await apiClient.Start("localhost", ApiListenerPlugin.ServiceName);
                await apiClient.Send("/api/ui/sections/apps", 1, 2, 3);
                apiClient.Stop();
            }
        }
        #endregion
    }
}
