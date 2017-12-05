using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.Scripts.Models;
using SmartHub.UWP.Plugins.Scripts.UI;
using SmartHub.UWP.Plugins.UI.Attributes;

namespace SmartHub.UWP.Plugins.Scripts
{
    [Plugin]
    [AppSectionItem("Scripts", AppSectionType.Applications, typeof(MainPage), "Scripts")]
    [AppSectionItem("Scripts", AppSectionType.System, typeof(SettingsPage), "Scripts")]
    public class ScriptsPlugin : PluginBase
    {

        #region Plugin ovverrides
        public override void InitDbModel()
        {
            //var db = Context.StorageOpen();
            //db.CreateTable<UserScript>();
            //db.CreateTable<ScriptEventHandler>();
        }
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
    }
}
