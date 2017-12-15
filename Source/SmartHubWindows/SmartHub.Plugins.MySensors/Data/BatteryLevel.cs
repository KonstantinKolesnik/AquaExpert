using System;

namespace SmartHub.Plugins.MySensors.Data
{
    public class BatteryLevel
    {
        public virtual Guid Id { get; set; }
        public virtual byte NodeNo { get; set; }
        public virtual DateTime TimeStamp { get; set; }
        public virtual byte Level { get; set; }
    }
}
