using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.Wemos.Controllers.Models;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

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

        public bool Contains(DateTime dt)
        {
            var now = dt.TimeOfDay;

            if (From < To)
                return From <= now && now <= To;
            else
                return !(To < now && now < From);
        }
    }

    public class WemosControllerWorkerScheduledSwitch : WemosControllerWorker
    {
        public class ControllerConfiguration
        {
            public string LineSwitchID
            {
                get; set;
            }
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
        protected async override void DoWork(DateTime now)
        {
            var config = Configuration as ControllerConfiguration;
            var isActiveNew = config.ActivePeriods.Where(p => p.IsEnabled && p.Contains(now)).Any();
            await host.SetLineValueAsync(LineSwitch, isActiveNew ? 1 : 0);
        }
        #endregion
    }
}
