using System;

namespace SmartHub.UWP.Plugins.Wemos.Models
{
    public class WemosNodeBatteryValue
    {
        public int NodeID { get; set; }
        public DateTime TimeStamp { get; set; }
        public long Value { get; set; }
    }
}
