using SmartHub.Plugins.MySensors.Core;
using System;
using System.ComponentModel.Composition;

namespace SmartHub.Plugins.MySensors
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MySensorsMessageAttribute : ExportAttribute
    {
        public MySensorsMessageAttribute()
            : base("7CDDD153-64E0-4050-8533-C47C1BACBC6B", typeof(Action<SensorMessage>))
        {
        }
    }
}
