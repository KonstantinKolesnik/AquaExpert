using System;

namespace MySensors.Controllers.Core
{
    public class SensorMessageEventArgs : EventArgs
    {
        public SensorMessage Message { get; set; }

        public SensorMessageEventArgs()
        {
        }
        public SensorMessageEventArgs(SensorMessage message)
        {
            Message = message;
        }
    }
}
