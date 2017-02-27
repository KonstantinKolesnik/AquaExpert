using SmartHub.UWP.Core.Communication.AppService;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Applications.Client
{
    public sealed partial class MainPage : Page
    {
        private AppServiceClientLocal client;

        public MainPage()
        {
            InitializeComponent();

            client = new AppServiceClientLocal("smarthubuwp.server", "07a5a158-235a-40cb-bf90-3be263fed556_g9kck1pvk5wcy");
        }

        private async void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            await client.Connect();
        }
        private void ButtonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            client.Disconnect();
        }
        private async void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            //client.Send();
        }
    }
}
