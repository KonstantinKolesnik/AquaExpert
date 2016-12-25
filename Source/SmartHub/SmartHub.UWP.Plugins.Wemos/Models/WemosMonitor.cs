using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.UWP.Plugins.Wemos.Models
{
    public class WemosMonitor
    {
        [PrimaryKey, NotNull, AutoIncrement]
        public int ID { get; set; }

        [NotNull]
        public string Name { get; set; }
        //[NotNull]
        public string NameForInformer { get; set; }
        [NotNull, Unique]
        public int LineID { get; set; }
        [NotNull]
        public string Configuration { get; set; }

        //public virtual dynamic GetConfiguration(Type type)
        //{
        //    var json = string.IsNullOrWhiteSpace(Configuration) ? "{}" : Configuration;
        //    return Extensions.FromJson(type, json);
        //}
        //public virtual void SetConfiguration(object value)
        //{
        //    Configuration = value.ToJson("{}");
        //}
    }
}
