using System;

namespace SmartHub.Plugins.MySensors.Data
{
    public class MySensorsSetting
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Value { get; set; }
    }
}
