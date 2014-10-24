
namespace MySensors.Core.Nodes
{
    public class Node
    {
        public byte NodeID { get; private set; }
        public SensorType Type { get; internal set; } // ArduinoNode / ArduinoRelay
        public string Version { get; internal set; } // library version
        public bool IsAckNeeded { get; internal set; }
        public string SketchName { get; internal set; }
        public string SketchVersion { get; internal set; }

        public bool IsGateway { get { return NodeID == 0; } }
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

        //public Node(byte nodeID, string name, string version, bool isAckNeeded)
        //{
        //    NodeID = nodeID;
        //    Name = name;
        //    Version = version;
        //    IsAckNeeded = isAckNeeded;
        //}
    }
}
