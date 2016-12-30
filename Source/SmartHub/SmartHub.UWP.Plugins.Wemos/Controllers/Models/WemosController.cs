using Newtonsoft.Json;
using SQLite.Net.Attributes;
using System;

namespace SmartHub.UWP.Plugins.Wemos.Controllers.Models
{
    public class WemosController
    {
        [PrimaryKey, NotNull, AutoIncrement]
        public int ID { get; set; }

        [NotNull]
        public string Name { get; set; }
        [NotNull]
        public WemosControllerType Type { get; set; }
        [NotNull, Default]
        public bool IsAutoMode { get; set; }
        [NotNull]
        public string Configuration { get; set; }

        public dynamic GetConfiguration(Type type)
        {
            if (string.IsNullOrWhiteSpace(Configuration))
                Configuration = "{}";

            return JsonConvert.DeserializeObject(Configuration, type);
        }
        public void SetConfiguration(object value)
        {
            Configuration = JsonConvert.SerializeObject(value);
        }
    }
}
