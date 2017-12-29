using System;

namespace SmartHub.UWP.Plugins.UI.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AppSectionItemAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public AppSectionType Type { get; set; }
        public Type UIModuleType { get; set; }

        //public string TileTypeFullName { get; set; }
        //public int SortOrder { get; set; }

        public string Url { get; set; }

        public AppSectionItemAttribute(string name, AppSectionType sectionType, Type uiModuleType, string description = null, string url = null)
        {
            Name = name;
            Type = sectionType;
            UIModuleType = uiModuleType;
            Description = description;
            Url = url;
        }
    }
}
