using System;
using System.ComponentModel.Composition;

namespace SmartHub.Plugins.Timer.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Timer_10_sec_ElapsedAttribute : ExportAttribute
    {
        public Timer_10_sec_ElapsedAttribute()
            : base("E65DEB15-50B3-4C0F-954E-014298979874", typeof(Action<DateTime>))
        {
        }
    }
}
