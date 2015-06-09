using SmartHub.Core.Plugins.Utils;
using System;

namespace SmartHub.Plugins.Monitors.Data
{
    public class Monitor
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual Guid SensorId { get; set; }
        public virtual string Configuration { get; set; }

        public virtual dynamic GetConfiguration(Type type)
        {
            var json = string.IsNullOrWhiteSpace(Configuration) ? "{}" : Configuration;
            return Extensions.FromJson(type, json);
        }
        public virtual void SetConfiguration(object value)
        {
            Configuration = value.ToJson("{}");
        }
    }
}
