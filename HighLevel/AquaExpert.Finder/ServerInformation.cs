using System.ComponentModel;

namespace AquaExpert.Finder
{
    public class ServerInformation : INotifyPropertyChanged
    {
        private int port;
        private string ipAddress;

        public int Port
        {
            get { return port; }
            set
            {
                if (port != value)
                {
                    port = value;
                    NotifyPropertyChanged("Port");
                }
            }
        }
        public string IPAddress
        {
            get { return ipAddress; }
            set
            {
                if (ipAddress != value)
                {
                    ipAddress = value;
                    NotifyPropertyChanged("IPAddress");
                }
            }
        }

        public ServerInformation(string ipAddress, int port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
        }

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
