using System;
using System.Composition;

namespace SmartHub.UWP.Plugins.Scripts.Attributes
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ScriptCommandAttribute : Attribute, IScriptCommandAttribute
    {
        public string MethodName
        {
            get; set;
        }
    }
}
