using SQLite.Net.Attributes;
using System;

namespace SmartHub.UWP.Plugins.Wemos.Core.Models
{
    public class WemosLine
    {
        [PrimaryKey, NotNull]
        public string ID
        {
            get; set;
        } = Guid.NewGuid().ToString();

        [NotNull]
        public int NodeID
        {
            get; set;
        }
        [NotNull]
        public int LineID
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        [NotNull]
        public WemosLineType Type
        {
            get; set;
        }
        public float ProtocolVersion
        {
            get; set;
        }

        // value = value * factor + offset
        [NotNull]
        public float Factor
        {
            get; set;
        } = 1;
        [NotNull, Default()]
        public float Offset
        {
            get; set;
        } = 0;

        public DateTime LastTimeStamp
        {
            get; set;
        }
        public float LastValue
        {
            get; set;
        }
    }
}
