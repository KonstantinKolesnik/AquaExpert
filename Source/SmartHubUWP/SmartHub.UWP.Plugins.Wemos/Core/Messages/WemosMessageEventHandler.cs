using Windows.Networking;

namespace SmartHub.UWP.Plugins.Wemos.Core.Messages
{
    public delegate void WemosMessageEventHandler(object sender, WemosMessageEventArgs args, HostName remoteAddress);
}
