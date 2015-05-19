using System;

namespace SmartHub.Plugins.WebUI.Tiles
{
    public class TileWeb
    {
        public Guid id = Guid.NewGuid();
        public string title; // bootom line text
        public string content; // body text
        public string url; // navigate url
        public object[] parameters; // if (url) -> navigate url parameters; 
        public string className; // CSS class attribute
        public bool wide = false;
        public string SignalRReceiveHandler;

        public TileWeb(Guid tileId)
        {
            id = tileId;
        }
    }
}
