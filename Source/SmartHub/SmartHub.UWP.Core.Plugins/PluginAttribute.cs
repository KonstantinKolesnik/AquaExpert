using System;
using System.Composition;

namespace SmartHub.UWP.Core.Plugins
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PluginAttribute : ExportAttribute
    {
        public PluginAttribute()
            : base(typeof(PluginBase))
        {
        }
    }


    //[MetadataAttribute]
    //[AttributeUsage(AttributeTargets.Class)]
    //public class PluginExtensionMetadataAttribute : Attribute
    //{
    //    public string Title { get; set; }
    //    public string Description { get; set; }
    //    public string ImageUri { get; set; }
    //}
}
