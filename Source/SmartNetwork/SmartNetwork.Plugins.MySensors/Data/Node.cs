using SmartNetwork.Plugins.MySensors.Core;

namespace SmartNetwork.Plugins.MySensors.Data
{
    public class Node
    {
        public virtual byte Id { get; set; }
        public virtual SensorType Type { get; set; }
        public virtual string ProtocolVersion { get; set; }
        public virtual string SketchName { get; set; }
        public virtual string SketchVersion { get; set; }
    }
}
