using System;

namespace SmartHub.Plugins.WebUI.Tiles
{
    public class TileWeb
    {
        public Guid id = Guid.NewGuid();
        public string url;
        public string title;
        public string className;
        public bool wide = false;
        public object[] parameters;
        public string content;

        public TileWeb(Guid tileId)
        {
            id = tileId;
        }
    }
}
