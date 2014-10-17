using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;


namespace SmartNetwork.Service
{
    public class ServiceLocator : ObservableObject
    {
        #region Fields
        private int port;
        private string key;
        private int receiveTimeout = 2000;
        private ObservableCollection<ServiceInformation> servers = new ObservableCollection<ServiceInformation>();
        #endregion

        #region Properties
        public ObservableCollection<ServiceInformation> Services
        {
            get { return servers; }
        }
        #endregion

        #region Events
        public event EventHandler ServerFound;
        public event EventHandler ServerLost;
        #endregion

        #region Constructor
        public ServiceLocator(int port, string key)
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

        //private void Request()
        //{
        //    string localIP = "";
        //    IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        //    foreach (IPAddress ip in host.AddressList)
        //    {
        //        if (ip.AddressFamily == AddressFamily.InterNetwork)
        //        {
        //            localIP = ip.ToString();
        //            break;
        //        }
        //    }
        //    localIP = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();


        //    IPEndPoint deviceEP = new IPEndPoint(IPAddress.Broadcast, port);

        //    IPEndPoint itemEP = new IPEndPoint(IPAddress.Any, port);

        //    byte[] request = Encoding.UTF8.GetBytes(key);
        //    string responseExpected = key + "OK";

        //    UdpClient client = new UdpClient();
        //    client.EnableBroadcast = true;
        //    client.Client.ReceiveTimeout = receiveTimeout;

        //    client.Send(request, request.Length, deviceEP);

        //    try
        //    {
        //        byte[] receiveBytes = client.Receive(ref itemEP);
        //        string response = Encoding.UTF8.GetString(receiveBytes, 0, receiveBytes.Length);
        //        if (String.Equals(response, responseExpected))
        //            SyncList(itemEP);
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    client.Close();
        //}
        //private void SyncList(IPEndPoint newServer)
        //{
        //    if (!Services.Any(server => server.IPAddress.Equals(newServer.Address.ToString())))
        //        Services.Add(new ServiceInformation(newServer.Address.ToString(), newServer.Port));
        //}



        private void Request()
        {
            DatagramSocket socket = new DatagramSocket();
            socket.Control.DontFragment = true;
            socket.MessageReceived += MessageReceived;

            try
            {
                // Connect to the server (in our case the listener we created in previous step).
                await socket.ConnectAsync(hostName, port.ToString());

                //rootPage.NotifyUser("Connected", NotifyType.StatusMessage);

                // Mark the socket as connected. Set the value to null, as we care only about the fact that the property is set.
                //CoreApplication.Properties.Add("connected", null);
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                //rootPage.NotifyUser("Connect failed with error: " + exception.Message, NotifyType.ErrorMessage);
            }


        }








        #endregion
    }
}
