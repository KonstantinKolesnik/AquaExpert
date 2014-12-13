using System;

namespace SmartNetwork.Plugins.MySensors.Data
{
    public class BatteryLevel
    {
        public virtual Guid Id { get; set; }

        public virtual Node Node { get; set; } // будет связано с полем "NodeId"
        public virtual DateTime TimeStamp { get; set; }
        public virtual byte Percent { get; set; }
    }
}
