using System;

namespace SmartHub.UWP.Plugins.Wemos.Data
{
    public class WemosNodeBatteryValue
    {
        public int NodeID { get; set; }
        public DateTime TimeStamp { get; set; }
        public int Value { get; set; }
    }
}
