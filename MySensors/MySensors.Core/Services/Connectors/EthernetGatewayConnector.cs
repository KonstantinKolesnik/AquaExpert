using MySensors.Core.Messaging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MySensors.Core.Services.Connectors
{
    public class EthernetGatewayConnector : IGatewayConnector
    {
        #region Fields
        private int port;
        private string key;
        private int receiveTimeout = 2000;
        private IPEndPoint remoteEP;
        #endregion

        #region Events
        public event MessageEventHandler MessageReceived;
        #endregion

        #region Constructor
        public EthernetGatewayConnector(int port, string key)
        {
            this.port = port;
            this.key = key;
        }
        public EthernetGatewayConnector()
            : this(8888, "EGW")
        {
        }
        #endregion

        #region Public methods
        public bool Connect()
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
                    return true;
            }
            catch (Exception ex)
            {
            }

            client.Close();

            return false;
        }
        public void Disconnect()
        {

        }
        public void Send(Message message)
        {

        }
        #endregion
    }

}
