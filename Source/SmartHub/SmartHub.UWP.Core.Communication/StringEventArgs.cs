﻿using System;

namespace SmartHub.UWP.Core.Communication
{
    public class StringEventArgs : EventArgs
    {
        public string Data { get; set; }

        public StringEventArgs()
        {
        }
        public StringEventArgs(string data)
        {
            Data = data;
        }
    }
}
