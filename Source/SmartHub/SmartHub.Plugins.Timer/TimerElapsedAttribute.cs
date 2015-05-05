using System;
using System.ComponentModel.Composition;

namespace SmartHub.Plugins.Timer
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Timer_3_sec_ElapsedAttribute : ExportAttribute
    {
        public Timer_3_sec_ElapsedAttribute()
            : base("7A16BD3C-EBDB-48DC-9A0A-B0E4B9FB1A93", typeof(Action<DateTime>))
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Timer_5_sec_ElapsedAttribute : ExportAttribute
    {
        public Timer_5_sec_ElapsedAttribute()
            : base("D69180B5-11BE-42F8-B3B4-630449613B42", typeof(Action<DateTime>))
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Timer_10_sec_ElapsedAttribute : ExportAttribute
    {
        public Timer_10_sec_ElapsedAttribute()
            : base("E65DEB15-50B3-4C0F-954E-014298979874", typeof(Action<DateTime>))
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Timer_30_sec_ElapsedAttribute : ExportAttribute
    {
        public Timer_30_sec_ElapsedAttribute()
            : base("9A9A8F9B-8389-4481-9ECC-7F8A27DC08CB", typeof(Action<DateTime>))
        {
        }
    }
}
