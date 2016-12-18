using SmartHub.UWP.Plugins.UI.Attributes;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SmartHub.UWP.Applications.Server
{
    public sealed partial class AppSectionPage : Page
    {
        public AppSectionPage()
        {
            InitializeComponent();
        }

        #region Navigation
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var isAppsSection = (AppSectionType) e.Parameter == AppSectionType.Applications;
            CoreApplication.Properties["IsAppsSection"] = isAppsSection;
            AppShell.Current.SetNavigationInfo(isAppsSection ? "Applications" : "System", isAppsSection ? "menuApplications" : "menuSystem");
        }
        #endregion

    }
}
