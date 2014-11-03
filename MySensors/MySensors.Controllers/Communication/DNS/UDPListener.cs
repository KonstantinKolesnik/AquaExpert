using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MySensors.Controllers.Communication.DNS
{
    public class UDPListener
    {
        #region Fields
        private Socket socket;
        private int port;
        private string key;
        private bool isStarted = false;
        #endregion

        #region Constructor
        public UDPListener(int port, string key)
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
                        if (socket.Poll(10, SelectMode.SelectRead))
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
                                        catch (Exception) { }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
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
