using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Xaml;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Storage.UI
{
    public sealed partial class SettingsPage : UserControl
    {
        #region Properties
        public static readonly DependencyProperty StorageSizeProperty = DependencyProperty.Register("StorageSize", typeof(string), typeof(SettingsPage), null);
        public string StorageSize
        {
            get { return (string) GetValue(StorageSizeProperty); }
            set { SetValue(StorageSizeProperty, value); }
        }
        #endregion

        #region Constructor
        public SettingsPage()
        {
            InitializeComponent();
            XamlUtils.FindFirstVisualChild<Grid>(this).DataContext = this;
        }
        #endregion

        #region Private methods
        private async Task UpdateStorageSize()
        {
            biRequest.IsActive = true;

            var size = await CoreUtils.RequestAsync<ulong>("/api/storage/size");
            StorageSize = $"{size} Bytes";

            biRequest.IsActive = false;
        }
        #endregion

        #region Event handlers
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateStorageSize();
        }
        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            await UpdateStorageSize();
        }
        #endregion
    }
}
