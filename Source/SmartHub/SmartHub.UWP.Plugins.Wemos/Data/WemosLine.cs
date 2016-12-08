using SmartHub.UWP.Plugins.Wemos.Core;

namespace SmartHub.UWP.Plugins.Wemos.Data
{
    public class WemosLine
    {
        public int NodeID { get; set; }
        public int LineID { get; set; }
        public string Name { get; set; }
        public WemosLineType Type { get; set; }
        public string ProtocolVersion { get; set; }

        public string TypeName
        {
            get { return Type.ToString(); }
        }
    }
}
