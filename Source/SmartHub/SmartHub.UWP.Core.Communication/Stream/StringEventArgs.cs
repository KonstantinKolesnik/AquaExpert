using System;

namespace SmartHub.UWP.Core.Communication.Stream
{
    public class StringEventArgs : EventArgs
    {
        public string Data
        {
            get; set;
        }

        public StringEventArgs(string data)
        {
            Data = data;
        }
    }
}
