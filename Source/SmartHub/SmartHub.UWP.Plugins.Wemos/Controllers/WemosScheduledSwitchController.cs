using SmartHub.UWP.Core;
using SmartHub.UWP.Plugins.Wemos.Controllers.Models;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using System;
using System.Collections.ObjectModel;

namespace SmartHub.UWP.Plugins.Wemos.Controllers
{
    public class Period : ObservableObject
    {
        private TimeSpan from;
        private TimeSpan to;
        private bool isEnabled;

        public TimeSpan From
        {
            get { return from; }
            set
            {
                from = value;
                NotifyPropertyChanged();
            }
        }
        public TimeSpan To
        {
            get { return to; }
            set
            {
                to = value;
                NotifyPropertyChanged();
            }
        }
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                NotifyPropertyChanged();
            }
        }
    }

    public class WemosScheduledSwitchController : WemosControllerBase
    {
        public class ControllerConfiguration
        {
            public int LineSwitchID
            {
                get; set;
            } = -1;
            public ObservableCollection<Period> ActivePeriods
            {
                get;
            } = new ObservableCollection<Period>();
        }

        #region Properties
        public WemosLine LineSwitch
        {
            get { return host.GetLine((Configuration as ControllerConfiguration).LineSwitchID); }
        }
        #endregion

        #region Constructor
        public WemosScheduledSwitchController(WemosController model)
            : base (model)
        {
        }
        #endregion

        #region Abstract Overrides
        protected override Type GetConfigurationType()
        {
            return typeof(ControllerConfiguration);
        }
        protected override object GetDefaultConfiguration()
        {
            return new ControllerConfiguration();
        }

        protected override bool IsMyMessage(WemosLineValue value)
        {
            return WemosPlugin.IsValueFromLine(value, LineSwitch);
        }
        protected async override void RequestLinesValues()
        {
            await host.RequestLineValue(LineSwitch);
        }
        protected async override void Process()
        {
            var config = Configuration as ControllerConfiguration;

            DateTime now = DateTime.Now;
            bool isActiveNew = false;
            foreach (var range in config.ActivePeriods)
                isActiveNew |= (range.IsEnabled && IsInRange(now, range));

            await host.SetLineValue(LineSwitch, isActiveNew ? 1 : 0);
        }
        #endregion

        #region Private methods
        private static bool IsInRange(DateTime dt, Period range)
        {
            //TimeSpan start = range.From.ToLocalTime().TimeOfDay;
            //TimeSpan end = range.To.ToLocalTime().TimeOfDay;
            TimeSpan start = range.From;
            TimeSpan end = range.To;

            TimeSpan now = dt.TimeOfDay;

            if (start < end)
                return start <= now && now <= end;
            else
                return !(end < now && now < start);
        }
        #endregion
    }
}
