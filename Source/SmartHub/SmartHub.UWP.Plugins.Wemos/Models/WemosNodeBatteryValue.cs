using SQLite.Net.Attributes;
using System;

namespace SmartHub.UWP.Plugins.Wemos.Models
{
    public class WemosNodeBatteryValue
    {
        public int NodeID { get; set; }
        [NotNull]
        public DateTime TimeStamp { get; set; }
        [NotNull]
        public int Value { get; set; }
    }
}
