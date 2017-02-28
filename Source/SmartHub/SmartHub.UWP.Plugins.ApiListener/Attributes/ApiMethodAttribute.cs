using System;
using System.Composition;

namespace SmartHub.UWP.Plugins.ApiListener.Attributes
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ApiMethodAttribute : Attribute
    {
        public string MethodName
        {
            get; set;
        }
    }
}
