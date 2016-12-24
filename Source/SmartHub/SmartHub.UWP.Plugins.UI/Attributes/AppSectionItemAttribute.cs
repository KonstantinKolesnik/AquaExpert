using System;

namespace SmartHub.UWP.Plugins.UI.Attributes
{
    public class AppSectionItemAttribute : Attribute
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public AppSectionType Type { get; set; }
        public Type TypeFullName { get; set; }

        //public string TileTypeFullName { get; set; }
        //public int SortOrder { get; set; }

        public AppSectionItemAttribute(string title, AppSectionType sectionType, Type typeFullName, string description = null)
        {
            Title = title;
            Type = sectionType;
            TypeFullName = typeFullName;
            Description = description;
        }
    }
}
