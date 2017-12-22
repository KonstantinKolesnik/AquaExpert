﻿using SmartHub.UWP.Core;
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
        #region Plugin ovverrides
        public override void InitPlugin()
        {
        }
        public override void StartPlugin()
        {
        }
        public override void StopPlugin()
        {
        }
        #endregion

        #region Remote API
        [ApiMethod("/api/storage/size")]
        public ApiMethod apiGetStorageSize => ((parameters) =>
        {
            var task = CoreUtils.GetFileBasicPropertiesAsync(Context.StoragePath);
            task.Wait();

            return new StorageInfo
            {
                Size = task.Result.Size,
                DateModified = task.Result.DateModified,
                ItemDate = task.Result.ItemDate,
            };
        });
        #endregion
    }
}
