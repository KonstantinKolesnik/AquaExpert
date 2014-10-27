using System;

namespace MySensors.Core.Nodes
{
    public class BatteryLevel : ObservableObject
    {
        private DateTime time;
        private byte percent;

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
    }
}
