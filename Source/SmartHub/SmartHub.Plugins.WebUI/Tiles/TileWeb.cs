using System;

namespace SmartHub.Plugins.WebUI.Tiles
{
    public class TileWeb
    {
        public Guid id = Guid.NewGuid();
        public string url;
        public string title;
        public bool wide;
        public object[] parameters;
        public string content;
        public string className;

        public TileWeb(Guid tileIid)
        {
            id = tileIid;
        }
    }
}
