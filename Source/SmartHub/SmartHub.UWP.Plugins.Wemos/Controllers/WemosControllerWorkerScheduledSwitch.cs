using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.Wemos.Controllers.Models;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using System;
using System.Collections.ObjectModel;

namespace SmartHub.UWP.Plugins.Wemos.Controllers
{
    class WemosControllerWorkerScheduledSwitch : WemosControllerWorker
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
        public WemosControllerWorkerScheduledSwitch(WemosController ctrl, IServiceContext context)
            : base(ctrl, context)
        {
        }
        #endregion

        #region Abstract Overrides
        protected override Type GetConfigurationType() => typeof(ControllerConfiguration);
        protected override object GetDefaultConfiguration() => new ControllerConfiguration();

        protected override bool IsMyMessage(WemosLineValue value) => WemosPlugin.IsValueFromLine(value, LineSwitch);
        protected async override void RequestLinesValues()
        {
            await host.RequestLineValueAsync(LineSwitch);
        }
        protected async override void DoWork()
        {
            var config = Configuration as ControllerConfiguration;

            var now = DateTime.Now;
            bool isActiveNew = false;
            foreach (var range in config.ActivePeriods)
                isActiveNew |= (range.IsEnabled && IsInRange(now, range));

            await host.SetLineValueAsync(LineSwitch, isActiveNew ? 1 : 0);
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
