using SmartNetwork.Plugins.MySensors.Core;
using System;

namespace SmartNetwork.Plugins.MySensors.Data
{
    public class Node
    {
        public virtual Guid Id { get; set; }

        public virtual byte ID { get; set; }
        public virtual SensorType Type { get; set; }
        public virtual string ProtocolVersion { get; set; }
        public virtual string SketchName { get; set; }
        public virtual string SketchVersion { get; set; }
    }
}
