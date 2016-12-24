using Windows.Networking;

namespace SmartHub.UWP.Plugins.Wemos.Core
{
    public delegate void WemosMessageEventHandler(object sender, WemosMessageEventArgs args, HostName remoteAddress);
}
