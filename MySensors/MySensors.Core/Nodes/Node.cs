
namespace MySensors.Core.Nodes
{
    public class Node : ObservableObject
    {
        public byte NodeID { get; private set; }
        public SensorType Type { get; internal set; } // ArduinoNode / ArduinoRelay
        public string ProtocolVersion { get; internal set; } // library version
        public string SketchName { get; internal set; }
        public string SketchVersion { get; internal set; }

        public bool IsRepeater { get { return Type == SensorType.ArduinoRelay; } }








        public Node(byte nodeID)
        {
            NodeID = nodeID;
        }
        public Node(byte nodeID, SensorType type)
        {
            NodeID = nodeID;
            Type = type;
        }
    }
}
