using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace MySensors.Controllers.Core
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
        public ObservableCollection<Sensor> Sensors
        {
            get { return sensors; }
        }

        public BatteryLevel LastBatteryLevel
        {
            get { return batteryLevels.Where(v => v.Time == batteryLevels.Select(vv => vv.Time).Max()).FirstOrDefault(); }
        }
        #endregion

        #region Constructors
        internal Node(byte id)
        {
            ID = id;

            batteryLevels.CollectionChanged += batteryLevels_CollectionChanged;
        }
        internal Node(byte id, SensorType type, string protocolVersion)
        {
            ID = id;
            Type = type;
            ProtocolVersion = protocolVersion;

            batteryLevels.CollectionChanged += batteryLevels_CollectionChanged;
        }

        internal Node(byte id, SensorType type, string protocolVersion, string sketchName, string sketchVersion)
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
            NotifyPropertyChanged("LastBatteryLevel");
        }
        #endregion
    }
}
