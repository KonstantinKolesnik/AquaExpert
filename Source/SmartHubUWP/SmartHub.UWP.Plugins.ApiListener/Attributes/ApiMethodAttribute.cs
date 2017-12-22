using System;
using System.Composition;

namespace SmartHub.UWP.Plugins.ApiListener.Attributes
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ApiMethodAttribute : ExportAttribute
    {
        public string MethodName
        {
            get; set;
        }

        public ApiMethodAttribute()
            : base(typeof(ApiMethod))
        {
        }
        public ApiMethodAttribute(string methodName)
            : base(typeof(ApiMethod))
        {
            MethodName = methodName;
        }
    }
}
