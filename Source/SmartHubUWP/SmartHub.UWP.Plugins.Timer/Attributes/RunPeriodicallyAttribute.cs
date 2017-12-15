using System;
using System.Composition;

namespace SmartHub.UWP.Plugins.Timer.Attributes
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RunPeriodicallyAttribute : Attribute
    {
        /// <summary>
        /// Time interval (seconds) between runs; set to 0 to disable
        /// </summary>
        public int Interval
        {
            get; set;
        } = 0;
    }
}
