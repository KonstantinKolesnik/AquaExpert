﻿
namespace SmartHub.Plugins.WebUI.Attributes
{
    public class AppSectionAttribute : JavaScriptResourceAttribute
    {
        public string Title { get; set; }
        public SectionType Type { get; set; }
        public int SortOrder { get; set; }
        public string TileTypeFullName { get; set; }

        public AppSectionAttribute(string title, SectionType sectionType, string url, string resourcePath)
            : base(url, resourcePath)
        {
            Title = title;
            Type = sectionType;
        }
    }
}
