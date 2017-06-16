using Newtonsoft.Json;
using SQLite.Net.Attributes;

namespace SmartHub.UWP.Plugins.Wemos.Monitors.Models
{
    public class WemosMonitor
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
        public string NameForInformer
        {
            get; set;
        }
        [NotNull]
        public int LineID
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
    }
}
