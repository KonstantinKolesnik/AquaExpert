using System;

namespace SmartHub.UWP.Plugins.UI.Attributes
{
    public class AppSectionItemEventArgs : EventArgs
    {
        public AppSectionItemAttribute Item
        {
            get;
        }

        public AppSectionItemEventArgs(AppSectionItemAttribute item)
        {
            Item = item;
        }
    }
}
