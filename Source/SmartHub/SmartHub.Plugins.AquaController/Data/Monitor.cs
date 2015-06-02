using System;

namespace SmartHub.Plugins.AquaController.Data
{
    public class Monitor
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual Guid SensorId { get; set; }
        public virtual bool IsVisible { get; set; }
    }
}
