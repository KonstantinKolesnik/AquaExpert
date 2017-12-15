using System;

namespace MySensors.Controllers.Core
{
    public class SensorValue : ObservableObject
    {
        #region Fields
        private byte nodeID;
        private byte id;
        private DateTime time;
        private SensorValueType type;
        private float value;
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
        public DateTime Time
        {
            get { return time; }
            private set
            {
                if (time != value)
                {
                    time = value;
                    NotifyPropertyChanged("Time");
                }
            }
        }
        public SensorValueType Type
        {
            get { return type; }
            private set
            {
                if (type != value)
                {
                    type = value;
                    NotifyPropertyChanged("Type");
                }
            }
        }
        public float Value
        {
            get { return value; }
            private set
            {
                if (this.value != value)
                {
                    this.value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }
        #endregion

        #region Constructors
        internal SensorValue(byte nodeID, byte id, DateTime time, SensorValueType type, float value)
        {
            NodeID = nodeID;
            ID = id;
            Time = time;
            Type = type;
            Value = value;
        }
        #endregion
    }
}
