using SmartHub.Plugins.MySensors.Core;
using System;

namespace SmartHub.Plugins.MySensors.Data
{
    public class Node
    {
        public virtual Guid Id { get; set; }
        public virtual byte NodeNo { get; set; }
        public virtual SensorType Type { get; set; }
        public virtual string ProtocolVersion { get; set; }
        public virtual string SketchName { get; set; }
        public virtual string SketchVersion { get; set; }
        public virtual string Name { get; set; }

        public virtual string TypeName
        {
            get { return Type.ToString(); }
        }
    }
}
