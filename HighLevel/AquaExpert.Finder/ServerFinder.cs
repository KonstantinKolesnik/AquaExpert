using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using System.Windows;

namespace AquaExpert.Finder
{
    public class ServerFinder : INotifyPropertyChanged
    {
        #region Fields
        private int port;
        private string key;
        private int receiveTimeout = 2000;
        //private ObservableCollection<IPEndPoint> servers = new ObservableCollection<IPEndPoint>();
        #endregion

        #region Properties
        //public ObservableCollection<IPEndPoint> Servers
        //{
        //    get { return servers; }
        //}

        public static DependencyProperty ServersProperty = DependencyProperty.Register("Servers", typeof(ObservableCollection<IPEndPoint>), typeof(ServerFinder), new PropertyMetadata(new ObservableCollection<IPEndPoint>()));
        public ObservableCollection<IPEndPoint> Servers
        {
            get { return (ObservableCollection<IPEndPoint>)GetValue(ServersProperty); }
            set { SetValue(ServersProperty, value); }
        }
        #endregion

        #region Events
        public event EventHandler ServerFound;
        public event EventHandler ServerLost;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        #endregion

        #region Constructor
        public ServerFinder(int port, string key)
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




            //IPEndPoint deviceEP = new IPEndPoint(IPAddress.Parse("192.168.1.84"), serverPort);
            //IPEndPoint deviceEP = new IPEndPoint(IPAddress.Parse("192.168.255.255"), serverPort);
            IPEndPoint deviceEP = new IPEndPoint(IPAddress.Broadcast, port);
            
            IPEndPoint itemEP = new IPEndPoint(IPAddress.Any, port);

            byte[] request = Encoding.UTF8.GetBytes(key);
            string responseExpected = key + "OK";

            UdpClient client = new UdpClient();
            client.EnableBroadcast = true;
            client.Client.ReceiveTimeout = receiveTimeout;
            //client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            //client.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 255);
            
            client.Send(request, request.Length, deviceEP);

            try
            {
                byte[] receiveBytes = client.Receive(ref itemEP);
                string response = Encoding.UTF8.GetString(receiveBytes);
                if (String.Equals(response, responseExpected))
                {
                    SyncList(itemEP);
                }
            }
            catch (Exception) {}

            client.Close();
        }
        private void SyncList(IPEndPoint newServer)
        {
            if (!Servers.Any(server => server.Address.Equals(newServer.Address)))
                Servers.Add(newServer);
        }
        #endregion
    }

    public class DiscoveryListener
    {
        #region Fields
        private Socket socket;
        private int port;
        private string key;
        private bool isStarted = false;
        #endregion

        #region Constructor
        public DiscoveryListener(int port, string key)
        {
            this.port = port;
            this.key = key;
        }
        #endregion

        #region Public methods
        public void Start()
        {
            if (!isStarted)
            {
                isStarted = true;

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
                socket.Bind(new IPEndPoint(IPAddress.Any, port));

                new Thread(Listen).Start();
            }
        }
        public void Stop()
        {
            isStarted = false;
        }
        #endregion

        #region Private methods
        private void Listen()
        {
            byte[] response = Encoding.UTF8.GetBytes(key + "OK");

            using (socket)
            {
                while (isStarted)
                {
                    try
                    {
                        if (socket.Poll(100, SelectMode.SelectRead))
                        {
                            if (socket.Available > 0)
                            {
                                EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                                byte[] buffer = new byte[socket.Available];

                                int bytesRead = socket.ReceiveFrom(buffer, ref remoteEP);
                                if (bytesRead > 0)
                                {
                                    string request = new string(Encoding.UTF8.GetChars(buffer, 0, bytesRead));
                                    if (String.Compare(request, key) == 0)
                                    {
                                        try
                                        {
                                            socket.SendTo(response, remoteEP);
                                        }
                                        catch (Exception e) { }
                                    }
                                }
                            }
                        }
                        else
                            Thread.Sleep(10);
                    }
                    catch (Exception e)
                    {
                        break;
                    }
                }
            }

            socket.Close();
            socket = null;

            isStarted = false;
        }
        #endregion
    }
}
