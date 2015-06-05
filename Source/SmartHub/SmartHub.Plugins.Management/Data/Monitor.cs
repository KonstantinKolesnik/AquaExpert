using System;

namespace SmartHub.Plugins.Management.Data
{
    public class Monitor
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual Guid SensorId { get; set; }
        //public virtual MonitorType Type { get; set; }
        //public virtual string Configuration { get; set; }
    }
}
