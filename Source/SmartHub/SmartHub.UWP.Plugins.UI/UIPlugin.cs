using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.ApiListener.Attributes;
using SmartHub.UWP.Plugins.Timer.Attributes;
using SmartHub.UWP.Plugins.UI.Attributes;
using System;
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
            sectionItems.AddRange(Context.GetAllPlugins().SelectMany(p => p.GetType().GetTypeInfo().GetCustomAttributes<AppSectionItemAttribute>()));
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

        [RunPeriodically(10)]
        public void aaa(DateTime dt)
        {

        }

        #region Remote API
        [ApiCommand("/api/ui/sections/apps")]
        public object GetCommonSectionItems(object parameters)
        {
            return GetSectionItems(AppSectionType.Applications);
        }
        //public object GetCommonSectionItems(params object[] parameters)
        //{
        //    return GetSectionItems(AppSectionType.Applications);
        //}

        //[ApiCommand("/api/ui/sections/system")]
        //public object GetSystemSectionItems(params object[] parameters)
        //{
        //    return GetSectionItems(AppSectionType.System);
        //}
        #endregion
    }
}
