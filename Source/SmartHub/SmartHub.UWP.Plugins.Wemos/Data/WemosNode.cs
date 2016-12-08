namespace SmartHub.UWP.Plugins.Wemos.Data
{
    public class WemosNode
    {
        public int NodeID { get; set; }
        public string Name { get; set; }
        //public WemosLineType Type { get; set; }
        //public string ProtocolVersion { get; set; }
        //public string SketchName { get; set; }
        //public string SketchVersion { get; set; }
        public bool NeedsReboot { get; set; }

        //public virtual string TypeName
        //{
        //    get { return Type.ToString(); }
        //}
    }
}
