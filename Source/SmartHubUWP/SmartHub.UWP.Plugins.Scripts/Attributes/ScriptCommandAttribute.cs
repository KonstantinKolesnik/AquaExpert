using System;
using System.Composition;

namespace SmartHub.UWP.Plugins.Scripts.Attributes
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ScriptCommandAttribute : ExportAttribute, IScriptCommandAttribute
    {
        public string MethodName
        {
            get; set;
        }

        public ScriptCommandAttribute()
            : base(typeof(ScriptCommand))
        {
        }
        public ScriptCommandAttribute(string methodName)
            : base(typeof(ScriptCommand))
        {
            MethodName = methodName;
        }
    }
}
