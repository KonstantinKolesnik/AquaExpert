using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.UI.Attributes;
using System.Collections.Generic;

namespace SmartHub.UWP.Plugins.UI
{
    public class UIPlugin : PluginBase
    {
        #region Fields
        private readonly List<AppSectionItemAttribute> sectionItems = new List<AppSectionItemAttribute>();
        //private readonly HashSet<string> cssFiles = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        #endregion

        #region Plugin ovverrides
        public override void InitPlugin()
        {
            foreach (var plugin in Context.GetAllPlugins())
            {
                var type = plugin.GetType();

                // элементы разделов
                //sectionItems.AddRange(type.GetCustomAttributes<AppSectionItemAttribute>());
                // стили
                //cssFiles.UnionWith(type.GetCustomAttributes<CssResourceAttribute>().Where(attr => attr.AutoLoad).Select(attr => attr.Url));
            }
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
        #endregion

        //#region Web API
        //[HttpCommand("/api/webui/sections/common")]
        //private object GetCommonSectionItems(HttpRequestParams request)
        //{
        //    return GetSectionItems(SectionType.Common);
        //}
        //[HttpCommand("/api/webui/sections/system")]
        //private object GetSystemSectionItems(HttpRequestParams request)
        //{
        //    return GetSectionItems(SectionType.System);
        //}
        //[HttpCommand("/api/webui/styles.json")]
        //private object GetStylesBundle(HttpRequestParams request)
        //{
        //    return cssFiles;
        //}
        //#endregion
    }
}
