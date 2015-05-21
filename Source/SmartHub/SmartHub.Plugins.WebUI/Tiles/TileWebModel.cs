using System;

namespace SmartHub.Plugins.WebUI.Tiles
{
    /// <summary>
    /// Used in response to client fetching tiles from server.
    /// Transport object.
    /// </summary>
    public class TileWebModel
    {
        public Guid id = Guid.NewGuid();
        public string title; // bootom line text
        public string content; // body text
        public string className; // CSS class attribute
        public bool wide = false; // normal or wide
        public string url; // navigate url
        public object[] parameters; // if (url) -> navigate url parameters; 
        public string SignalRReceiveHandler;

        public TileWebModel(Guid tileId)
        {
            id = tileId;
        }
    }
}
