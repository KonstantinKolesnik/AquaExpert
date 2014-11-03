using System.Collections.ObjectModel;

namespace MySensors.Core
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
        private ObservableCollection<Sensor> sensors = new ObservableCollection<Sensor>();
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
            set
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
            set
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
            set
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
            set
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
            //set
            //{
            //    batteryLevels = value == null ? new ObservableCollection<BatteryLevel>() : value;
            //    NotifyPropertyChanged("BatteryLevels");
            //}
        }
        public ObservableCollection<Sensor> Sensors
        {
            get { return sensors; }
            //set
            //{
            //    sensors = value == null ? new ObservableCollection<Sensor>() : value;
            //    NotifyPropertyChanged("Sensors");
            //}
        }
        #endregion

        #region Constructors
        public Node(byte id)
        {
            ID = id;
        }
        public Node(byte id, SensorType type, string protocolVersion, string sketchName, string sketchVersion)
        {
            ID = id;
            Type = type;
            ProtocolVersion = protocolVersion;
            SketchName = sketchName;
            SketchVersion = sketchVersion;
        }
        #endregion
    }
}
