using SmartHub.UWP.Plugins.Wemos.Core;

namespace SmartHub.UWP.Plugins.Wemos.Data
{
    public class WemosNode
    {
        public int NodeID { get; set; }
        public string Name { get; set; }
        public WemosLineType Type { get; set; }
        public float ProtocolVersion { get; set; }
        public string FirmwareName { get; set; }
        public float FirmwareVersion { get; set; }
        public bool NeedsReboot { get; set; }

        public string TypeName
        {
            get { return Type.ToString(); }
        }
    }
}
