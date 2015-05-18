using SmartHub.Core.Plugins.Utils;
using System;

namespace SmartHub.Plugins.AquaController.Data
{
    public class Setting
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string SerializedValue { get; set; }

        public virtual dynamic GetValue()
        {
            var json = string.IsNullOrWhiteSpace(SerializedValue) ? "{}" : SerializedValue;
            return Extensions.FromJson(json);
        }
        public virtual void SetValue(object value)
        {
            SerializedValue = value.ToJson();
        }
    }
}
