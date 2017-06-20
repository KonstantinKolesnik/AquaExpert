using SmartHub.UWP.Core.Communication.Stream;
using SmartHub.UWP.Core.Infrastructure;
using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.ApiListener;
using SmartHub.UWP.Plugins.ApiListener.Attributes;
using SmartHub.UWP.Plugins.UI.Attributes;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Windows.Networking;

namespace SmartHub.UWP.Plugins.Storage
{
    [Plugin]
    //[AppSectionItem("Wemos", AppSectionType.System, typeof(Settings), "Wemos modules")]
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
            //ServiceContext.DbPath
        }
        public override void StartPlugin()
        {
        }
        public override void StopPlugin()
        {
        }
        #endregion




    }
}
