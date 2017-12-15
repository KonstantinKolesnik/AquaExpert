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
}
