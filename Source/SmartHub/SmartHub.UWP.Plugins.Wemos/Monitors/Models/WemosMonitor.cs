using Newtonsoft.Json;
using SQLite.Net.Attributes;
using System;

namespace SmartHub.UWP.Plugins.Wemos.Monitors.Models
{
    public class WemosMonitor
    {
        [PrimaryKey, NotNull]//, AutoIncrement]
        public string ID
        {
            get; set;
        } = Guid.NewGuid().ToString();
        [NotNull]
        public int LineID
        {
            get; set;
        }
        [NotNull]
        public float Min
        {
            get; set;
        }
        [NotNull]
        public float Max
        {
            get; set;
        }







        //[NotNull]
        //public string Configuration
        //{
        //    get; set;
        //}

        //public T DeserializeConfiguration<T>()
        //{
        //    if (string.IsNullOrWhiteSpace(Configuration))
        //        Configuration = "{}";

        //    return JsonConvert.DeserializeObject<T>(Configuration);
        //}
        //public void SerializeConfiguration(object value)
        //{
        //    Configuration = JsonConvert.SerializeObject(value);
        //}
    }
}
