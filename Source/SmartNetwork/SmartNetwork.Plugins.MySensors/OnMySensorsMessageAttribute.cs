using SmartNetwork.Plugins.MySensors.Core;
using System;
using System.ComponentModel.Composition;

namespace SmartNetwork.Plugins.MySensors
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class OnMySensorsMessageAttribute : ExportAttribute
    {
        public OnMySensorsMessageAttribute()
            : base("7CDDD153-64E0-4050-8533-C47C1BACBC6B", typeof(Action<SensorMessage>))
        {
        }
    }
}
