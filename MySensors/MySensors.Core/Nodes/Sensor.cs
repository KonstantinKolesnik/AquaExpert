
namespace MySensors.Core.Nodes
{
    public class Sensor : ObservableObject
    {
        public byte NodeID { get; private set; }
        public byte SensorID { get; private set; }
        public SensorType Type { get; internal set; }
        public string ProtocolVersion { get; internal set; } // library version




        public Sensor(byte nodeID, byte sensorID)
        {
            NodeID = nodeID;
            SensorID = sensorID;
        }
    }
}
