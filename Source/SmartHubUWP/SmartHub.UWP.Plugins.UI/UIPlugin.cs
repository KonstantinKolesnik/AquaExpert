using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.ApiListener;
using SmartHub.UWP.Plugins.ApiListener.Attributes;
using SmartHub.UWP.Plugins.UI.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SmartHub.UWP.Plugins.UI
{
    [Plugin]
    public class UIPlugin : PluginBase
    {
        #region Fields
        private readonly List<AppSectionItemAttribute> sectionItems = new List<AppSectionItemAttribute>();
        #endregion

        #region Plugin ovverrides
        public override void InitPlugin()
        {
            var attrs = Context.GetAllPlugins().SelectMany(p => p.GetType().GetTypeInfo().GetCustomAttributes<AppSectionItemAttribute>());
            sectionItems.AddRange(attrs);
        }
        #endregion

        #region Private methods
        //private object GetSectionItems(AppSectionType sectionType)
        //{
        //    return sectionItems
        //        .Where(item => item.Type == sectionType)
        //        .OrderBy(item => item.Name)
        //        .Select(item => new
        //        {
        //            id = Guid.NewGuid(),
        //            name = item.Name,
        //            //path = item.GetModulePath(),
        //            sortOrder = item.SortOrder,
        //            tileTypeFullName = item.TileTypeFullName
        //        }).ToArray();
        //}
        private List<AppSectionItemAttribute> GetSectionItems(AppSectionType sectionType)
        {
            return sectionItems
                .Where(item => item.Type == sectionType)
                .OrderBy(item => item.Name)
                .ToList();
        }
        #endregion

        #region Remote API
        [ApiMethod("/api/ui/sections/apps")]
        public ApiMethod apiGetApplicationsSectionItems => (args =>
        {
            return Context.GetPlugin<UIPlugin>().GetSectionItems(AppSectionType.Applications);
        });

        [ApiMethod("/api/ui/sections/system")]
        public ApiMethod apiGetSystemSectionItems => (args =>
        {
            return Context.GetPlugin<UIPlugin>().GetSectionItems(AppSectionType.System);
        });
        #endregion
    }
}
