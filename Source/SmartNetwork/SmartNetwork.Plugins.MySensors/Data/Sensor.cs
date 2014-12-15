using SmartNetwork.Plugins.MySensors.Core;

namespace SmartNetwork.Plugins.MySensors.Data
{
    public class Sensor
    {
        public virtual byte NodeId { get; set; }
        public virtual byte Id { get; set; }
        public virtual SensorType Type { get; set; }
        public virtual string ProtocolVersion { get; set; }
    }
}
