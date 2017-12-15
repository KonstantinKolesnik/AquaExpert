using SmartHub.Plugins.MySensors.Core;
using System;
using System.ComponentModel.Composition;

namespace SmartHub.Plugins.MySensors.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MySensorsMessageCalibrationAttribute : ExportAttribute
    {
        public MySensorsMessageCalibrationAttribute()
            : base("6536BBC2-3B62-4D01-B786-E5A4A2D7A095", typeof(Action<SensorMessage>))
        {
        }
    }
}
