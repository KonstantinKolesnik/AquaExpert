using SmartHub.UWP.Plugins.Wemos.Controllers.Models;
using SmartHub.UWP.Plugins.Wemos.Core;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using System;
using System.Collections.Generic;

namespace SmartHub.UWP.Plugins.Wemos.Controllers
{
    public struct Period
    {
        public DateTime From;
        public DateTime To;
        public bool IsEnabled;
    }

    public class WemosSwitchController : WemosControllerBase
    {
        public class ControllerConfiguration
        {
            public int LineSwitchID { get; set; }
            public List<Period> ActivePeriods { get; set; }

            public static ControllerConfiguration Default
            {
                get
                {
                    return new ControllerConfiguration()
                    {
                        LineSwitchID = -1,
                        ActivePeriods = new List<Period>()
                    };
                }
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
        public WemosSwitchController(WemosController controller)
            : base (controller)
        {
            if (string.IsNullOrEmpty(controller.Configuration))
            {
                configuration = ControllerConfiguration.Default;
                controller.SerializeConfiguration(configuration);
            }
            else
                configuration = controller.DeserializeConfiguration<ControllerConfiguration>();
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
            if (IsAutoMode)
            {
                DateTime now = DateTime.Now;
                bool isActiveNew = false;
                foreach (var range in configuration.ActivePeriods)
                    isActiveNew |= (range.IsEnabled && IsInRange(now, range));

                await host.SetLineValue(LineSwitch, isActiveNew ? 1 : 0);
            }
        }
        #endregion

        #region Private methods
        private static bool IsInRange(DateTime dt, Period range)
        {
            TimeSpan start = range.From.ToLocalTime().TimeOfDay;
            TimeSpan end = range.To.ToLocalTime().TimeOfDay;
            TimeSpan now = dt.TimeOfDay;

            if (start < end)
                return start <= now && now <= end;
            else
                return !(end < now && now < start);
        }
        #endregion
    }
}
