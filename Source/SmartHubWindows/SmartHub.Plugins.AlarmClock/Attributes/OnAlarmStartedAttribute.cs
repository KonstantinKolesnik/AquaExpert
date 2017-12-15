﻿using System;
using System.ComponentModel.Composition;

namespace SmartHub.Plugins.AlarmClock.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class OnAlarmStartedAttribute : ExportAttribute
    {
        public OnAlarmStartedAttribute()
            : base("0917789F-A980-4224-B43F-A820DEE093C8", typeof(Action<Guid>))
        {
        }
    }
}
