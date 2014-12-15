using SmartNetwork.Plugins.MySensors.Core;
using System;

namespace SmartNetwork.Plugins.MySensors.Data
{
    public class SensorValue
    {
        public virtual byte NodeId { get; set; }
        public virtual byte SensorId { get; set; }
        public virtual DateTime TimeStamp { get; set; }
        public virtual SensorValueType Type { get; set; }
        public virtual float Value { get; set; }
    }
}
