using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SmartHub.UWP.Applications.Client
{
    public sealed partial class AboutPage : Page
    {
        #region Constructor
        public AboutPage()
        {
            InitializeComponent();
        }
        #endregion

        #region Navigation
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            AppShell.Current.SetNavigationInfo("About", "menuAbout");
        }
        #endregion
    }
}
