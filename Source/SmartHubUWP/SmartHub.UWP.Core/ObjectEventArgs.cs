using System;

namespace SmartHub.UWP.Core
{
    public class ObjectEventArgs : EventArgs
    {
        public object Data
        {
            get;
        }

        public ObjectEventArgs(object data)
        {
            Data = data;
        }
    }
}
