using System;

namespace MySensors.Controllers.Communication
{
    public class NetworkMessageEventArgs : EventArgs
    {
        public NetworkMessage Message { get; set; }

        public NetworkMessageEventArgs()
        {
        }
        public NetworkMessageEventArgs(NetworkMessage message)
        {
            Message = message;
        }
    }
}
