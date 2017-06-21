using SmartHub.UWP.Core;
using SmartHub.UWP.Core.StringResources;
using SmartHub.UWP.Core.Xaml;
using SmartHub.UWP.Plugins.Storage.Models;
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

        public static readonly DependencyProperty DateModifiedProperty = DependencyProperty.Register("DateModified", typeof(string), typeof(SettingsPage), null);
        public string DateModified
        {
            get { return (string) GetValue(DateModifiedProperty); }
            set { SetValue(DateModifiedProperty, value); }
        }

        public static readonly DependencyProperty ItemDateProperty = DependencyProperty.Register("ItemDate", typeof(string), typeof(SettingsPage), null);
        public string ItemDate
        {
            get { return (string) GetValue(ItemDateProperty); }
            set { SetValue(ItemDateProperty, value); }
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
        private async Task UpdateStorageInfo()
        {
            biRequest.IsActive = true;

            var info = await CoreUtils.RequestAsync<StorageInfo>("/api/storage/size");

            StorageSize = info != null ? $"{info?.Size / 1024} kB" : Labels.NoData;
            DateModified = info != null ? info?.DateModified.ToString("dd.MM.yy HH:mm:ss") : Labels.NoData;
            ItemDate = info != null ? info?.ItemDate.ToString("dd.MM.yy HH:mm:ss") : Labels.NoData;

            biRequest.IsActive = false;
        }
        #endregion

        #region Event handlers
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateStorageInfo();
        }
        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            await UpdateStorageInfo();
        }
        #endregion
    }
}
