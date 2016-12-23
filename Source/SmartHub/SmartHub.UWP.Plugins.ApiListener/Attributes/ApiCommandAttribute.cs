using System;
using System.Composition;

namespace SmartHub.UWP.Plugins.ApiListener.Attributes
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ApiCommandAttribute : Attribute
    {
        public string CommandName
        {
            get; set;
        }
    }
}
