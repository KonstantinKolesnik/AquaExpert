using Newtonsoft.Json;
using SQLite.Net.Attributes;

namespace SmartHub.UWP.Plugins.Wemos.Controllers.Models
{
    public class WemosController
    {
        [PrimaryKey, NotNull, AutoIncrement]
        public int ID
        {
            get; set;
        }

        [NotNull]
        public string Name
        {
            get; set;
        }
        [NotNull]
        public WemosControllerType Type
        {
            get; set;
        }
        [NotNull, Default]
        public bool IsAutoMode
        {
            get; set;
        }
        [NotNull]
        public string Configuration
        {
            get; set;
        }

        public T DeserializeConfiguration<T>()
        {
            if (string.IsNullOrWhiteSpace(Configuration))
                Configuration = "{}";

            return JsonConvert.DeserializeObject<T>(Configuration);
        }
        public void SerializeConfiguration(object value)
        {
            Configuration = JsonConvert.SerializeObject(value);
        }

        [Ignore]
        public object Config
        {
            get { return string.IsNullOrWhiteSpace(Configuration) ? null : JsonConvert.DeserializeObject(Configuration); }
            set { Configuration = JsonConvert.SerializeObject(value); }
        }
    }
}
