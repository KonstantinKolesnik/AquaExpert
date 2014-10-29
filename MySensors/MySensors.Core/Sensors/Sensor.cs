using System.Collections.ObjectModel;

namespace MySensors.Core.Sensors
{
    public class Sensor : ObservableObject
    {
        #region Fields
        private byte nodeID;
        private byte id;
        private SensorType type;
        private string protocolVersion = "";
        private ObservableCollection<SensorValue> values = new ObservableCollection<SensorValue>();
        #endregion

        #region Properties
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
        public ObservableCollection<SensorValue> Values
        {
            get { return values; }
            internal set { values = value == null ? new ObservableCollection<SensorValue>() : value; }
        }
        #endregion

        #region Constructors
        public Sensor(byte nodeID, byte id)
        {
            NodeID = nodeID;
            ID = id;
        }
        #endregion
    }
}
