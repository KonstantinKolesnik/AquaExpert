using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AquaExpert.Finder
{
    public class ServerFinder : INotifyPropertyChanged
    {
        #region Fields
        private int serverPort;
        private string serverName;
        private UdpClient client;
        private bool isServerAvailable = false;
        private IPEndPoint serverEP = null;
        private int receiveTimeout = 2000;
        private int pollPeriod = 2000;
        private bool started = false;
        #endregion

        #region Properties
        public bool IsServerAvailable
        {
            get { return isServerAvailable; }
            private set
            {
                if (isServerAvailable != value)
                {
                    isServerAvailable = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsServerAvailable"));
                }
            }
        }
        public IPEndPoint ServerEP
        {
            get { return serverEP; }
            private set
            {
                if (serverEP != value)
                {
                    serverEP = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ServerEP"));
                }
            }
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
        public ServerFinder(int serverPort, string serverName)
        {
            this.serverPort = serverPort;
            this.serverName = serverName;

            //client = new UdpClient(9999);
            //client.Client.ReceiveTimeout = receiveTimeout;
            //client.EnableBroadcast = true;

            //client.Client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast);
            //client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
        }
        #endregion

        #region Public methods
        public void Start()
        {
            if (!started)
            {
                started = true;

                new Thread(() => {
                    byte[] request = Encoding.UTF8.GetBytes(serverName);
                    string responseExpected = serverName + "OK";

                    while (started)
                    {
                        UdpClient client = new UdpClient(9999);
                        client.EnableBroadcast = true;
                        client.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 255);
                        IPEndPoint groupEp = new IPEndPoint(IPAddress.Broadcast, serverPort);
                        client.Send(request, request.Length, groupEp);
                        client.Close();


                        UdpClient udpResponse = new UdpClient(9999);
                        udpResponse.EnableBroadcast = true;
                        udpResponse.Client.ReceiveTimeout = receiveTimeout;
                        IPEndPoint recvEp = new IPEndPoint(IPAddress.Any, serverPort);
                        try
                        {
                            if (udpResponse.Available > 0)
                            {
                                Byte[] receiveBytes = udpResponse.Receive(ref recvEp);
                                string response = Encoding.UTF8.GetString(receiveBytes);
                                if (String.Equals(response, responseExpected))
                                {
                                    if (!IsServerAvailable && ServerFound != null)
                                        ServerFound(this, EventArgs.Empty);

                                    IsServerAvailable = true;
                                    ServerEP = recvEp;

                                    Thread.Sleep(pollPeriod);
                                }
                            }

                            udpResponse.Close();
                        }
                        catch (SocketException e)
                        {
                            if (IsServerAvailable && ServerLost != null)
                                ServerLost(this, EventArgs.Empty);

                            IsServerAvailable = false;
                            ServerEP = null;

                        }
                        udpResponse.Close();

                        





                        
                        //IPEndPoint hostEP = new IPEndPoint(IPAddress.Broadcast, serverPort);
                        ////IPEndPoint hostEP = new IPEndPoint(IPAddress.Parse("192.168.1.84"), serverPort);
                        //client.Send(request, request.Length, hostEP);

                        //try
                        //{
                        //    if (client.Available > 0)
                        //    {
                        //        IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
                        //        Byte[] receiveBytes = client.Receive(ref ep); // blocks until: 1) socket receives a message from a remote host; 2) timeout elapsed.
                        //        string response = Encoding.UTF8.GetString(receiveBytes);
                        //        if (String.Equals(response, responseExpected))
                        //        {
                        //            if (!IsServerAvailable && ServerFound != null)
                        //                ServerFound(this, EventArgs.Empty);

                        //            IsServerAvailable = true;
                        //            ServerEP = ep;

                        //            Thread.Sleep(pollPeriod);
                        //        }
                        //    }
                        //}
                        //catch (SocketException e)
                        //{
                        //    if (IsServerAvailable && ServerLost != null)
                        //        ServerLost(this, EventArgs.Empty);

                        //    IsServerAvailable = false;
                        //    ServerEP = null;
                        //}
                    }

                    if (ServerLost != null)
                        ServerLost(this, EventArgs.Empty);

                    IsServerAvailable = false;
                    ServerEP = null;
                }).Start();
            }
        }
        public void Stop()
        {
            started = false;
        }
        #endregion
    }
}
