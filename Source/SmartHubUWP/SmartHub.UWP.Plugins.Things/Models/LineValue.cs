using SQLite.Net.Attributes;
using System;

namespace SmartHub.UWP.Plugins.Things.Models
{
    public class LineValue// : IEquatable<WemosLineValue>, IComparable<WemosLineValue>
    {
        [NotNull]
        public string LineID
        {
            get; set;
        }
        [NotNull]
        public DateTime TimeStamp
        {
            get; set;
        }
        [NotNull, Default()]
        public float Value
        {
            get; set;
        }

        //public bool Equals(WemosLineValue other)
        //{
        //    if (ReferenceEquals(other, null)) return false;
        //    if (ReferenceEquals(this, other)) return true;
        //    return TimeStamp.Equals(other.TimeStamp);
        //}
        //public int CompareTo(WemosLineValue other)
        //{
        //    if (other == null)
        //        return 1;
        //    return TimeStamp.CompareTo(other.TimeStamp);
        //}
        //public override int GetHashCode()
        //{
        //    return (int)TimeStamp.Ticks;
        //}
        //public override string ToString()
        //{
        //    return Name;
        //}
    }
}
