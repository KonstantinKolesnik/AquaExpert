using SmartHub.Plugins.MySensors.Core;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SmartHub.Plugins.MySensors.GatewayProxies
{
    class EthernetGatewayProxy : IGatewayProxy
    {
        #region Fields
        private int port;
        private string key;
        private int receiveTimeout = 2000;
        private IPEndPoint remoteEP;
        private bool isConnected = false;
        #endregion

        #region Properties
        public bool IsStarted
        {
            get { return isConnected; }
        }
        #endregion

        #region Events
        public event SensorMessageEventHandler MessageReceived;
        #endregion

        #region Constructor
        public EthernetGatewayProxy(int port, string key)
        {
            this.port = port;
            this.key = key;
        }
        public EthernetGatewayProxy()
            : this(8888, "EGW")
        {
        }
        #endregion

        #region Public methods
        public void Start()
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

            remoteEP = new IPEndPoint(IPAddress.Any, port);

            byte[] request = Encoding.ASCII.GetBytes(key);
            string responseExpected = key + "OK";

            UdpClient client = new UdpClient();
            client.EnableBroadcast = true;
            client.Client.ReceiveTimeout = receiveTimeout;

            client.Send(request, request.Length, deviceEP);

            try
            {
                byte[] receiveBytes = client.Receive(ref remoteEP);
                string response = Encoding.UTF8.GetString(receiveBytes);
                if (String.Equals(response, responseExpected))
                    //SyncList(remoteEP);

                    isConnected = true;
                return;
            }
            catch (Exception)
            {
            }

            client.Close();
        }
        public void Stop()
        {


            isConnected = false;
        }
        public void Send(SensorMessage message)
        {
            if (isConnected)
            {

            }
        }
        #endregion
    }
}
