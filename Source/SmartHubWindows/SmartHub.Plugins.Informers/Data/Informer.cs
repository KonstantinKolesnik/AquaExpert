using SmartHub.Core.Plugins.Utils;
using System;
using System.Collections.Generic;

namespace SmartHub.Plugins.Informers.Data
{
    public class Informer
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual Guid SensorDisplayId { get; set; }
        public virtual string MonitorsIds { get; set; }

        public virtual List<Guid> GetMonitorsIds()
        {
            var json = string.IsNullOrWhiteSpace(MonitorsIds) ? "{}" : MonitorsIds;
            return (List<Guid>)Extensions.FromJson(typeof(List<Guid>), json);
        }
        public virtual void SetMonitorsIds(List<Guid> value)
        {
            MonitorsIds = value.ToJson("{}");
        }
    }
}
