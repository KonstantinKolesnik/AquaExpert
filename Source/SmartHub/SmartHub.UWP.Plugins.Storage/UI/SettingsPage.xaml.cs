using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Services.OneDrive;
using SmartHub.UWP.Core;
using SmartHub.UWP.Core.StringResources;
using SmartHub.UWP.Core.Xaml;
using SmartHub.UWP.Plugins.Storage.Models;
using System;
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
        private async Task InitCloud()
        {
            // First get the root of your OneDrive.
            // By default the service silently connects the current Windows user if Windows is associated with a Microsoft Account.
            //var folder = await OneDriveService.Instance.RootFolderAsync();
            // If Windows is not associated with a Microsoft Account, you need:
            // 1) Initialize the service using an authentication provider AccountProviderType.Msa or AccountProviderType.Adal
            //OneDriveService.Instance.Initialize(appClientId, AccountProviderType.Msa, OneDriveScopes.OfflineAccess | OneDriveScopes.ReadWrite);
            // 2) Sign in
            //if (!await OneDriveService.Instance.LoginAsync())
            //    throw new Exception("Unable to sign in");


            // Once you have a reference to the Root Folder you can get a list of all items.
            // List the Items from the current folder.
            //var OneDriveItems = await folder.GetItemsAsync();
            //do
            //{
            //    //Get the next page of items
            //    OneDriveItems = await folder.NextItemsAsync();
            //}
            //while (OneDriveItems != null);


            // Then from there you can play with folders and files. Create Folder.
            //var level1Folder = await rootFolder.CreateFolderAsync("Level1");
            //var level2Folder = await level1Folder.CreateFolderAsync("Level2");
            //var level3Folder = await level2Folder.CreateFolderAsync("Level3");
            // You can get a sub folder by path
            //var level3Folder = await rootFolder.GetFolderAsync("Level1/Level2/Level3");

            // Move Folder:
            //var result = await level3Folder.MoveAsync(rootFolder);
            // Copy Folder:
            //var result = level3Folder.CopyAsync(destFolder)

            // Rename Folder:
            //await level3Folder.RenameAsync("NewLevel3");

            // Create new files:
            //var selectedFile = await OpenLocalFileAsync();
            //if (selectedFile != null)
            //{
            //    using (var localStream = await selectedFile.OpenReadAsync())
            //    {
            //        var fileCreated = await level3Folder.CreateFileAsync(selectedFile.Name, CreationCollisionOption.GenerateUniqueName, localStream);
            //    }
            //}

            // If the file exceed the Maximum size (ie 4MB) use the UploadFileAsync method instead:
            //var largeFileCreated = await folder.UploadFileAsync(selectedFile.Name, localStream, CreationCollisionOption.GenerateUniqueName, 320 * 1024);

            // You can also Move, Copy or Rename a file:
            //await fileCreated.MoveAsync(destFolder);
            //await fileCreated.CopyAsync(destFolder);
            //await fileCreated.RenameAsync("newName");

            // Download a file and save the content in a local file:
            //var remoteFile = await level3Folder.GetFile("NewFile.docx");
            //using (var remoteStream = await remoteFile.OpenAsync())
            //{
            //    byte[] buffer = new byte[remoteStream.Size];
            //    var localBuffer = await remoteStream.ReadAsync(buffer.AsBuffer(), (uint) remoteStream.Size, InputStreamOptions.ReadAhead);
            //    var localFolder = ApplicationData.Current.LocalFolder;
            //    var myLocalFile = await localFolder.CreateFileAsync($"{oneDriveFile.Name}", CreationCollisionOption.GenerateUniqueName);
            //    using (var localStream = await myLocalFile.OpenAsync(FileAccessMode.ReadWrite))
            //    {
            //        await localStream.WriteAsync(localBuffer);
            //        await localStream.FlushAsync();
            //    }
            //}

            // At last you can get the thumbnail of a file:
            //var stream = await file.GetThumbnailAsync(ThumbnailSize.Large)
            // Windows.UI.Xaml.Controls.Image thumbnail = new Windows.UI.Xaml.Controls.Image();
            //BitmapImage bmp = new BitmapImage();
            //await bmp.SetSourceAsync(streamTodDisplay);
            //thumbnail.Source = bmp;

            //-------------------------------------------------------------------------------------------------------------------------------------------

            if (NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
            {
                //Live SDK ID: 00000000401DF53E

                var folder = await OneDriveService.Instance.DocumentsFolderAsync();
                //var remoteFile = await folder.GetFileAsync("NewFile.docx");


            }
        }
        #endregion

        #region Event handlers
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateStorageInfo();
            await InitCloud();
        }
        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            await UpdateStorageInfo();
        }
        #endregion
    }
}
