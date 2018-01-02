using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.ApiListener;
using SmartHub.UWP.Plugins.ApiListener.Attributes;
using SmartHub.UWP.Plugins.Storage.Models;
using SmartHub.UWP.Plugins.Storage.UI;
using SmartHub.UWP.Plugins.UI.Attributes;

namespace SmartHub.UWP.Plugins.Storage
{
    [Plugin]
    [AppSectionItem("Storage", AppSectionType.System, typeof(SettingsPage), "Data storage")]
    public class StoragePlugin : PluginBase
    {
        #region Remote API
        [ApiMethod("/api/storage/size")]
        public ApiMethod apiGetStorageSize => (args =>
        {
            var task = CoreUtils.GetFileBasicPropertiesAsync(Context.StoragePath);
            task.Wait();
            var result = task.Result;

            return new StorageInfo
            {
                Size = result.Size,
                DateModified = result.DateModified,
                ItemDate = result.ItemDate,
            };
        });
        #endregion
    }
}
