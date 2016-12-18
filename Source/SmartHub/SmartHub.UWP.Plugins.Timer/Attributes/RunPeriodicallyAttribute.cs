using System;
using System.Composition;

namespace SmartHub.UWP.Plugins.Timer.Attributes
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RunPeriodicallyAttribute : ExportAttribute, IRunPeriodicallyAttribute
    {
        public const string ContractID = nameof(RunPeriodicallyAttribute);

        /// <summary>
        /// Time interval (seconds) between runs; set to 0 to disable
        /// </summary>
        public int Interval
        {
            get; set;
        }

        public RunPeriodicallyAttribute()
            : this(0)
        {
        }
        public RunPeriodicallyAttribute(int interval)
            : base(ContractID, typeof(Action<DateTime>))
        {
            Interval = interval;
        }
    }

    //[MetadataAttribute]
    //[AttributeUsage(AttributeTargets.Class)]
    //public class PluginExtensionMetadataAttribute : Attribute
    //{
    //    public string Title { get; set; }
    //    public string Description { get; set; }
    //    public string ImageUri { get; set; }
    //}
}
