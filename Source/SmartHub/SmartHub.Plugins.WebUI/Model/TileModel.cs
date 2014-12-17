using System;

namespace SmartHub.Plugins.WebUI.Model
{
    public class TileModel
    {
        public TileModel(Guid tileIid)
        {
            id = tileIid;
        }

        public Guid id = Guid.NewGuid();
        public string title;
        public bool wide;
        public string url;
        public object[] parameters;
        public string content;
        public string className;
    }
}
