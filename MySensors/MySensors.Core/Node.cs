using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

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
        }
        public ObservableCollection<Sensor> Sensors
        {
            get { return sensors; }
        }

        public BatteryLevel BatteryLevel
        {
            get { return batteryLevels != null && batteryLevels.Count != 0 ? batteryLevels.Where(v => v.Time == batteryLevels.Select(vv => vv.Time).Max()).FirstOrDefault() : null; }
        }
        #endregion

        #region Constructors
        public Node(byte id)
        {
            ID = id;

            batteryLevels.CollectionChanged += batteryLevels_CollectionChanged;
        }
        public Node(byte id, SensorType type, string protocolVersion, string sketchName, string sketchVersion)
        {
            ID = id;
            Type = type;
            ProtocolVersion = protocolVersion;
            SketchName = sketchName;
            SketchVersion = sketchVersion;

            batteryLevels.CollectionChanged += batteryLevels_CollectionChanged;
        }
        #endregion

        #region Event handlers
        private void batteryLevels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("BatteryLevel");
        }
        #endregion
    }
}
