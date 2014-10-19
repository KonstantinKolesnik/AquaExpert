
namespace SmartNetwork.Core.Service
{
    public class ServiceInformation : ObservableObject
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

        public ServiceInformation(string ipAddress, int port)
        {
            IPAddress = ipAddress;
            Port = port;
        }
    }
}
