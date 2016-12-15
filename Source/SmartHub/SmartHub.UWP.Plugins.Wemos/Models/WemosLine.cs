using SmartHub.UWP.Plugins.Wemos.Core;
using SQLite.Net.Attributes;

namespace SmartHub.UWP.Plugins.Wemos.Models
{
    public class WemosLine
    {
        [NotNull]
        public int NodeID { get; set; }
        [NotNull]
        public int LineID { get; set; }
        public string Name { get; set; }
        [NotNull]
        public WemosLineType Type { get; set; }
        public float ProtocolVersion { get; set; }
    }
}
