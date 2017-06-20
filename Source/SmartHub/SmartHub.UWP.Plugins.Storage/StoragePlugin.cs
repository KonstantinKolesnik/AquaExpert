using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.ApiListener;
using SmartHub.UWP.Plugins.ApiListener.Attributes;
using SmartHub.UWP.Plugins.Storage.UI;
using SmartHub.UWP.Plugins.UI.Attributes;
using System.Composition;

namespace SmartHub.UWP.Plugins.Storage
{
    [Plugin]
    [AppSectionItem("Storage", AppSectionType.System, typeof(SettingsPage), "Data storage")]
    public class StoragePlugin : PluginBase
    {

        #region Plugin ovverrides
        public override void InitDbModel()
        {
            //using (var db = Context.OpenConnection())
            //{
            //    //db.CreateTable<WemosSetting>();
            //}
        }
        public override void InitPlugin()
        {
            //Context.StoragePath
        }
        public override void StartPlugin()
        {
        }
        public override void StopPlugin()
        {
        }
        #endregion


        #region Remote API
        [ApiMethod(MethodName = "/api/storage/size"), Export(typeof(ApiMethod))]
        public ApiMethod apiGetStorageSize => ((parameters) =>
        {
            var task = CoreUtils.GetFileSizeAsync(Context.StoragePath);
            task.Wait();
            return task.Result;
        });
        #endregion
    }
}
