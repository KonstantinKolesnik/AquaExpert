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
        /// Time interval between run (in minutes)
        /// </summary>
        public int Interval { get; private set; }

        public RunPeriodicallyAttribute(int interval)
            : base(ContractID, typeof(Action<DateTime>))
        {
            Interval = interval;
        }
    }
}
