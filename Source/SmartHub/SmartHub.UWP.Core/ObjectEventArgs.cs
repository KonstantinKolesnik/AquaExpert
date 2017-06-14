using System;

namespace SmartHub.UWP.Core
{
    public class ObjectEventArgs : EventArgs
    {
        public object Item
        {
            get;
        }

        public ObjectEventArgs(object item)
        {
            Item = item;
        }
    }
}
