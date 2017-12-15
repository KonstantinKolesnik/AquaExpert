using SmartHub.Core.Plugins.Utils;
using System;

namespace SmartHub.Plugins.MeteoStation.Data
{
    public class MeteoStationSetting
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string SerializedValue { get; set; }

        public virtual dynamic GetValue(Type type)
        {
            var json = string.IsNullOrWhiteSpace(SerializedValue) ? "{}" : SerializedValue;
            return Extensions.FromJson(type, json);
        }
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
