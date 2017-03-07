using SmartHub.UWP.Plugins.Wemos.Controllers.Models;
using SmartHub.UWP.Plugins.Wemos.Core;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using System;
using System.Collections.ObjectModel;

namespace SmartHub.UWP.Plugins.Wemos.Controllers
{
    public class Period
    {
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
        public bool IsEnabled { get; set; }
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

            public static ControllerConfiguration Default
            {
                get { return new ControllerConfiguration(); }
            }
        }

        #region Fields
        private ControllerConfiguration configuration = null;
        #endregion

        #region Properties
        public WemosLine LineSwitch
        {
            get { return host.GetLine(configuration.LineSwitchID); }
        }
        #endregion

        #region Constructor
        public WemosScheduledSwitchController(WemosController model)
            : base (model)
        {
            if (string.IsNullOrEmpty(model.Configuration))
            {
                configuration = ControllerConfiguration.Default;
                model.SerializeConfiguration(configuration);
            }
            else
                configuration = model.DeserializeConfiguration<ControllerConfiguration>();
        }
        #endregion

        #region Abstract Overrides
        public override bool IsMyMessage(WemosMessage message)
        {
            return WemosPlugin.IsMessageFromLine(message, LineSwitch);
        }
        public async override void RequestLinesValues()
        {
            await host.RequestLineValue(LineSwitch);
        }
        protected async override void Process()
        {
            DateTime now = DateTime.Now;
            bool isActiveNew = false;
            foreach (var range in configuration.ActivePeriods)
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
