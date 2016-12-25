using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.ApiListener;
using SmartHub.UWP.Plugins.ApiListener.Attributes;
using SmartHub.UWP.Plugins.UI.Attributes;
using System.Collections.Generic;
using System.Composition;
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
        //        .Where(sectionItem => sectionItem.Type == sectionType)
        //        .OrderBy(sectionItem => sectionItem.Title)
        //        .Select(sectionItem => new
        //        {
        //            id = Guid.NewGuid(),
        //            name = sectionItem.Title,
        //            //path = sectionItem.GetModulePath(),
        //            sortOrder = sectionItem.SortOrder,
        //            tileTypeFullName = sectionItem.TileTypeFullName
        //        }).ToArray();
        //}
        private List<AppSectionItemAttribute> GetSectionItems(AppSectionType sectionType)
        {
            return sectionItems
                .Where(item => item.Type == sectionType)
                .OrderBy(item => item.Title)
                .ToList();
        }
        #endregion

        #region Remote API
        [ApiCommand(CommandName = "/api/ui/sections/apps"), Export(typeof(ApiCommand))]
        public ApiCommand apiGetApplicationsSectionItems => ((parameters) =>
        {
            return Context.GetPlugin<UIPlugin>().GetSectionItems(AppSectionType.Applications);
        });

        [ApiCommand(CommandName = "/api/ui/sections/system"), Export(typeof(ApiCommand))]
        public ApiCommand apiGetSystemSectionItems => ((parameters) =>
        {
            return Context.GetPlugin<UIPlugin>().GetSectionItems(AppSectionType.System);
        });
        #endregion
    }
}
