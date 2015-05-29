using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.Plugins.AquaController.Data
{
    public class Monitor
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual Guid SensorId { get; set; }
    }
}
