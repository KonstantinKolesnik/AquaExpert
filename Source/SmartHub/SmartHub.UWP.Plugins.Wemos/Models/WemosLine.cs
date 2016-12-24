using SQLite.Net.Attributes;
using System;

namespace SmartHub.UWP.Plugins.Wemos.Models
{
    public class WemosLine
    {
        [PrimaryKey, NotNull, AutoIncrement]
        public int ID { get; set; }
        [NotNull]
        public int NodeID { get; set; }
        [NotNull]
        public int LineID { get; set; }

        public string Name { get; set; }
        [NotNull]
        public WemosLineType Type { get; set; }
        public float ProtocolVersion { get; set; }

        public DateTime LastTimeStamp { get; set; }
        public float LastValue { get; set; }
    }
}
