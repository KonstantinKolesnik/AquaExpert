
namespace MySensors.Core.Nodes
{
    public class Sensor : ObservableObject
    {
        private byte nodeID;
        private byte id;
        private SensorType type;
        private string protocolVersion = "";

        public byte NodeID
        {
            get { return nodeID; }
            private set
            {
                if (nodeID != value)
                {
                    nodeID = value;
                    NotifyPropertyChanged("NodeID");
                }
            }
        }
        public byte ID
        {
            get { return id; }
            private set
            {
                if (id != value)
                {
                    id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }
        public SensorType Type
        {
            get { return type; }
            internal set
            {
                if (type != value)
                {
                    type = value;
                    NotifyPropertyChanged("Type");
                }
            }
        }
        public string ProtocolVersion // library version
        {
            get { return protocolVersion; }
            internal set
            {
                if (protocolVersion != value)
                {
                    protocolVersion = value;
                    NotifyPropertyChanged("ProtocolVersion");
                }
            }
        }

        public Sensor(byte nodeID, byte id)
        {
            NodeID = nodeID;
            ID = id;
        }
    }
}
