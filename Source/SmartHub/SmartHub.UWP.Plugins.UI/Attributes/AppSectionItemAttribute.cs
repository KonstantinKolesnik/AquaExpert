using System;
using System.Composition;

namespace SmartHub.UWP.Plugins.UI.Attributes
{
    //public class AppSectionItemAttribute : Attribute
    //{
    //    public string Title { get; set; }
    //    public AppSectionType Type { get; set; }
    //    public int SortOrder { get; set; }
    //    public string TileTypeFullName { get; set; }

    //    public AppSectionItemAttribute(string title, AppSectionType sectionType)//, string url, string resourcePath)
    //        //: base(url, resourcePath)
    //    {
    //        Title = title;
    //        Type = sectionType;
    //    }
    //}


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AppSectionItemAttribute : ExportAttribute
    {
        public const string ContractID = nameof(AppSectionItemAttribute);

        public AppSectionItemAttribute()
            : base(ContractID)
        {
        }
    }
}
