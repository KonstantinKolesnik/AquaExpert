using System;
using System.ComponentModel.Composition;

namespace SmartHub.Plugins.MySensors.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MySensorsDisconnectedAttribute : ExportAttribute
    {
        public MySensorsDisconnectedAttribute()
            : base("8E26E0FB-657F-4DEA-BF9D-A9E93A5632DC", typeof(Action))
        {
        }
    }
}
