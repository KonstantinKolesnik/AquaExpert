using System;
using System.ComponentModel.Composition;

namespace SmartHub.Plugins.Timer.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Timer_10sec_ElapsedAttribute : ExportAttribute
    {
        public const string CLSID = "E65DEB15-50B3-4C0F-954E-014298979874";

        public Timer_10sec_ElapsedAttribute()
            : base(CLSID, typeof(Action<DateTime>))
        {
        }
    }
}
