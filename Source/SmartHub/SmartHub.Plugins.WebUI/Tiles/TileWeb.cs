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
        public string cssClassName;
        public bool wide = false;

        public TileWeb(Guid tileId)
        {
            id = tileId;
        }
    }
}
