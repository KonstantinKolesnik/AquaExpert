using SmartHub.UWP.Plugins.UI.Attributes;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SmartHub.UWP.Applications.Server
{
    public sealed partial class AppSectionPage : Page
    {
        private bool isAppsSection;

        private string ItemsKey
        {
            get { return isAppsSection ? "ApplicationItems" : "SystemItems"; }
        }
        private string ApiCommandName
        {
            get { return isAppsSection ? "/api/ui/sections/apps" : "/api/ui/sections/system"; }
        }

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

            AppShell.Current.ApiClient.Received += ApiClient_Received;

            if (!CoreApplication.Properties.ContainsKey(ItemsKey))
            {
                // request items
                await AppShell.Current.ApiClient.SendAsync(ApiCommandName, 1, 2, 3);
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            AppShell.Current.ApiClient.Received -= ApiClient_Received;
        }


        private void ApiClient_Received(object sender, Core.Communication.Stream.StringEventArgs e)
        {
            int a = 0;
            int b = a;
        }
        #endregion
    }
}
