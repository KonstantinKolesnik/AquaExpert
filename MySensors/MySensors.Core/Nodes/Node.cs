using System.Collections.ObjectModel;

namespace MySensors.Core.Nodes
{
    public class Node : ObservableObject
    {
        #region Fields
        private byte id;
        private SensorType type;
        private string protocolVersion = "";
        private string sketchName = "";
        private string sketchVersion = "";
        private ObservableCollection<BatteryLevel> batteryLevels = new ObservableCollection<BatteryLevel>();
        #endregion

        #region Properties
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
        public SensorType Type // ArduinoNode / ArduinoRelay
        {
            get { return type; }
            internal set
            {
                if (type != value)
                {
                    type = value;
                    NotifyPropertyChanged("Type");
                    NotifyPropertyChanged("IsRepeater");
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
        public string SketchName
        {
            get { return sketchName; }
            internal set
            {
                if (sketchName != value)
                {
                    sketchName = value;
                    NotifyPropertyChanged("SketchName");
                }
            }
        }
        public string SketchVersion
        {
            get { return sketchVersion; }
            internal set
            {
                if (sketchVersion != value)
                {
                    sketchVersion = value;
                    NotifyPropertyChanged("SketchVersion");
                }
            }
        }
        public ObservableCollection<BatteryLevel> BatteryLevels
        {
            get { return batteryLevels; }
        }
        
        public bool IsRepeater
        {
            get { return Type == SensorType.ArduinoRelay; }
        }
        #endregion

        #region Constructors
        public Node(byte id)
        {
            ID = id;
        }
        #endregion
    }
}
