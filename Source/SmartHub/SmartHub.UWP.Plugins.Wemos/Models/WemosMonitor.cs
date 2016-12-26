using Newtonsoft.Json;
using SQLite.Net.Attributes;
using System;

namespace SmartHub.UWP.Plugins.Wemos.Models
{
    public class WemosMonitor
    {
        [PrimaryKey, NotNull, AutoIncrement]
        public int ID { get; set; }

        [NotNull]
        public string Name { get; set; }
        public string NameForInformer { get; set; }
        [NotNull]
        public int LineID { get; set; }
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

    public class WemosMonitorDto : WemosMonitor
    {
        public string LineName { get; set; }

        public WemosMonitorDto(WemosMonitor monitor)
        {
            if (monitor != null)
            {
                ID = monitor.ID;
                Name = monitor.Name;
                NameForInformer = monitor.NameForInformer;
                LineID = monitor.LineID;
                Configuration = monitor.Configuration;
            }
        }
    }
}
