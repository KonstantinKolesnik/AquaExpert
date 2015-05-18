﻿using System;
using System.ComponentModel.Composition;

namespace SmartHub.Plugins.Timer.Attributes
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RunPeriodicallyAttribute : ExportAttribute, IRunPeriodicallyAttribute
    {
        public RunPeriodicallyAttribute(int interval)
            : base("38A9F1A7-63A4-4688-8089-31F4ED4A9A61", typeof(Action<DateTime>))
        {
            Interval = interval;
        }

        /// <summary>
        /// Time interval between run (in minutes)
        /// </summary>
        public int Interval { get; private set; }
    }
}
