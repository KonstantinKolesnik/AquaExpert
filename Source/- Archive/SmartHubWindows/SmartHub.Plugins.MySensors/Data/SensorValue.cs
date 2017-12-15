using SmartHub.Plugins.MySensors.Core;
using System;

namespace SmartHub.Plugins.MySensors.Data
{
    public class SensorValue
    {
        public virtual Guid Id { get; set; }
        public virtual byte NodeNo { get; set; }
        public virtual byte SensorNo { get; set; }
        public virtual SensorValueType Type { get; set; }
        public virtual DateTime TimeStamp { get; set; }
        public virtual float Value { get; set; }

        public virtual string TypeName
        {
            get { return Type.ToString(); }
        }
    }
}
