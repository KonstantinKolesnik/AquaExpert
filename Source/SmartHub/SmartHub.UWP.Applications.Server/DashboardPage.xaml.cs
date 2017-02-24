using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SmartHub.UWP.Applications.Server
{
    public sealed partial class DashboardPage : Page
    {
        #region Constructor
        public DashboardPage()
        {
            InitializeComponent();
        }
        #endregion

        #region Navigation
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            AppShell.Current.SetNavigationInfo("Dashboard", "menuDashboard");
        }
        #endregion
    }
}
