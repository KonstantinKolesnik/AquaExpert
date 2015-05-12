using System;

namespace SmartHub.Plugins.AquaController.Data
{
    public class Setting
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Value { get; set; }
    }
}
