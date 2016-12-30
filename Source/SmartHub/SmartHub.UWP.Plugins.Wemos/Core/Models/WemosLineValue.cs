using SQLite.Net.Attributes;
using System;

namespace SmartHub.UWP.Plugins.Wemos.Core.Models
{
    public class WemosLineValue
    {
        [NotNull]
        public int NodeID { get; set; }
        [NotNull]
        public int LineID { get; set; }
        [NotNull]
        public DateTime TimeStamp { get; set; }
        [NotNull]
        public WemosLineType Type { get; set; }
        [NotNull, Default()]
        public float Value { get; set; }
    }
}
