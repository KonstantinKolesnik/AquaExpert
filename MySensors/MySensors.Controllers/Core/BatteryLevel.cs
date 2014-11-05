using System;

namespace MySensors.Controllers.Core
{
    public class BatteryLevel : ObservableObject
    {
        #region Fields
        private byte nodeID;
        private DateTime time;
        private byte percent;
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
        public byte Percent
        {
            get { return percent; }
            private set
            {
                if (percent != value)
                {
                    percent = value;
                    NotifyPropertyChanged("Percent");
                }
            }
        }
        #endregion

        #region Constructors
        public BatteryLevel(byte nodeID, DateTime time, byte percent)
        {
            NodeID = nodeID;
            Time = time;
            Percent = percent;
        }
        #endregion
    }
}
