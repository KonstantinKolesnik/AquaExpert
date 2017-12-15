using System;
using System.ComponentModel.Composition;

namespace SmartHub.Plugins.MySensors.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MySensorsConnectedAttribute : ExportAttribute
    {
        public MySensorsConnectedAttribute()
            : base("46CD89C9-08A2-4E3C-84EE-826DA51127CF", typeof(Action))
        {
        }
    }
}
