using System;
using System.Composition;

namespace SmartHub.UWP.Plugins.Timer.Attributes
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RunPeriodicallyAttribute : ExportAttribute, IRunPeriodicallyAttribute
    {
        public const string ContractID = "38A9F1A7-63A4-4688-8089-31F4ED4A9A61";

        /// <summary>
        /// Time interval between run (in seconds). 0 to disable
        /// </summary>
        public int Interval
        {
            get;
            set;
        }

        public RunPeriodicallyAttribute()
            : base(ContractID, typeof(Action<DateTime>))
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
