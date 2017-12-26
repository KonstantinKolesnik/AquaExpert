using System;
using Windows.Networking;

namespace SmartHub.UWP.Core.Communication.Udp
{
    public class UdpMessageEventArgs : EventArgs
    {
        public HostName RemoteAddress
        {
            get; set;
        }
        public string Data
        {
            get; set;
        }

        public UdpMessageEventArgs(HostName remoteAddress, string data)
        {
            RemoteAddress = remoteAddress;
            Data = data;
        }
    }
}
