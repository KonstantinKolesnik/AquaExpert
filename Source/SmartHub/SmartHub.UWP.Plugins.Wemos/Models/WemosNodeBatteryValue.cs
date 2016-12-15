using SQLite.Net.Attributes;
using System;

namespace SmartHub.UWP.Plugins.Wemos.Models
{
    public class WemosNodeBatteryValue
    {
        [PrimaryKey, NotNull]
        public int NodeID { get; set; }
        [NotNull]
        public DateTime TimeStamp { get; set; }
        [NotNull]
        public long Value { get; set; }
    }
}
