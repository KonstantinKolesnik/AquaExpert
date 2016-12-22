using System;

namespace SmartHub.UWP.Plugins.UI.Attributes
{
    public class AppSectionItemAttribute : Attribute
    {
        public string Title { get; set; }
        public AppSectionType Type { get; set; }
        public string TypeFullName { get; set; }

        public string TileTypeFullName { get; set; }
        public int SortOrder { get; set; }

        public AppSectionItemAttribute(string title, AppSectionType sectionType, Type typeFullName)
        {
            Title = title;
            Type = sectionType;
            TypeFullName = typeFullName.ToString();
        }
    }
}
