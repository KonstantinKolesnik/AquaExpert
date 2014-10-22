using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MySensors.Controller.Core.Locators
{
    public class EthernetGatewayLocator// : INotifyPropertyChanged
    {
        #region Fields
        private int port;
        private string key;
        private int receiveTimeout = 2000;
        //private ObservableCollection<ServerInformation> servers = new ObservableCollection<ServerInformation>();
        #endregion

        #region Properties
        //public ObservableCollection<ServerInformation> Servers
        //{
        //    get { return servers; }
        //}
        #endregion

        #region Events
        //public event EventHandler ServerFound;
        //public event EventHandler ServerLost;

        //public event PropertyChangedEventHandler PropertyChanged;
        //private void OnPropertyChanged(PropertyChangedEventArgs e)
        //{
        //    if (PropertyChanged != null)
        //        PropertyChanged(this, e);
        //}
        #endregion

        #region Constructor
        public EthernetGatewayLocator(int port, string key)
        {
            this.port = port;
            this.key = key;
        }
        #endregion

        #region Public methods
        public void Refresh()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return;

            new Thread(Request).Start();
        }

        private void Request()
        {
            //string localIP = "";
            //IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            //foreach (IPAddress ip in host.AddressList)
            //{
            //    if (ip.AddressFamily == AddressFamily.InterNetwork)
            //    {
            //        localIP = ip.ToString();
            //        break;
            //    }
            //}
            //localIP = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();


            IPEndPoint deviceEP = new IPEndPoint(IPAddress.Broadcast, port);

            IPEndPoint itemEP = new IPEndPoint(IPAddress.Any, port);

            byte[] request = Encoding.UTF8.GetBytes(key);
            string responseExpected = key + "OK";

            UdpClient client = new UdpClient();
            client.EnableBroadcast = true;
            client.Client.ReceiveTimeout = receiveTimeout;

            client.Send(request, request.Length, deviceEP);

            try
            {
                byte[] receiveBytes = client.Receive(ref itemEP);
                string response = Encoding.UTF8.GetString(receiveBytes);
                if (String.Equals(response, responseExpected))
                    SyncList(itemEP);
            }
            catch (Exception ex)
            {
            }

            client.Close();
        }
        private void SyncList(IPEndPoint newServer)
        {
            //if (!Servers.Any(server => server.IPAddress.Equals(newServer.Address.ToString())))
            //Servers.Add(new ServerInformation(newServer.Address.ToString(), newServer.Port));
        }
        #endregion
    }

}
