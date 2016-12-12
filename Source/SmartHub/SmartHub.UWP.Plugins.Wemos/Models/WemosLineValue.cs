using System;

namespace SmartHub.UWP.Plugins.Wemos.Models
{
    public class WemosLineValue
    {
        public int NodeID { get; set; }
        public int LineID { get; set; }
        //public virtual WemosLineValueType Type { get; set; }
        public DateTime TimeStamp { get; set; }
        public float Value { get; set; }

        //public virtual string TypeName
        //{
        //    get { return Type.ToString(); }
        //}
    }
}
