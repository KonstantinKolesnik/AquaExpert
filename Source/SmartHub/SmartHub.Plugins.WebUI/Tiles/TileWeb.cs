using System;

namespace SmartHub.Plugins.WebUI.Tiles
{
    public class TileWeb
    {
        public TileWeb(Guid tileIid)
        {
            id = tileIid;
        }

        public Guid id = Guid.NewGuid();
        public string url;
        public string title;
        public bool wide;
        public object[] parameters;
        public string content;
        public string className;
    }
}
