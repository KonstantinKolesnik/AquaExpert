using Microsoft.Win32;
using SmartHub.Core.Plugins;
using SmartHub.Plugins.Timer.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Timers;

namespace SmartHub.Plugins.Timer
{
    [Plugin]
    public class TimerPlugin : PluginBase
    {
        #region Fields
        private const int TIMER_INTERVAL_3_SEC = 3000;
        private const int TIMER_INTERVAL_5_SEC = 5000;
        private const int TIMER_INTERVAL_10_SEC = 10000;
        private const int TIMER_INTERVAL_30_SEC = 30000;

        private System.Timers.Timer timer_3_sec;
        private System.Timers.Timer timer_5_sec;
        private System.Timers.Timer timer_10_sec;
        private System.Timers.Timer timer_30_sec;

        private readonly List<PeriodicalActionState> periodicalHandlers = new List<PeriodicalActionState>();
        #endregion

        #region Import
        [ImportMany("7A16BD3C-EBDB-48DC-9A0A-B0E4B9FB1A93")]
        public Action<DateTime>[] Timer_3_sec_ElapsedEventHandlers { get; set; }
        [ImportMany("D69180B5-11BE-42F8-B3B4-630449613B42")]
        public Action<DateTime>[] Timer_5_sec_ElapsedEventHandlers { get; set; }
        [ImportMany("E65DEB15-50B3-4C0F-954E-014298979874")]
        public Action<DateTime>[] Timer_10_sec_ElapsedEventHandlers { get; set; }
        [ImportMany("9A9A8F9B-8389-4481-9ECC-7F8A27DC08CB")]
        public Action<DateTime>[] Timer_30_sec_ElapsedEventHandlers { get; set; }

        [ImportMany("38A9F1A7-63A4-4688-8089-31F4ED4A9A61")]
        public Lazy<Action<DateTime>, IRunPeriodicallyAttribute>[] PeriodicalActions { get; set; }
        #endregion

        #region Plugin ovverrides
        public override void InitPlugin()
        {
            CultureInfo.CurrentCulture.ClearCachedData();
            SystemEvents.TimeChanged += (sender, args) => CultureInfo.CurrentCulture.ClearCachedData();

            timer_3_sec = new System.Timers.Timer(TIMER_INTERVAL_3_SEC);
            timer_3_sec.Elapsed += timer_3_sec_Elapsed;

            timer_5_sec = new System.Timers.Timer(TIMER_INTERVAL_5_SEC);
            timer_5_sec.Elapsed += timer_5_sec_Elapsed;

            timer_10_sec = new System.Timers.Timer(TIMER_INTERVAL_10_SEC);
            timer_10_sec.Elapsed += timer_10_sec_Elapsed;

            timer_30_sec = new System.Timers.Timer(TIMER_INTERVAL_30_SEC);
            timer_30_sec.Elapsed += timer_30_sec_Elapsed;

            RegisterPeriodicalHandlers();
        }
        public override void StartPlugin()
        {
            timer_3_sec.Enabled = true;
            timer_5_sec.Enabled = true;
            timer_10_sec.Enabled = true;
            timer_30_sec.Enabled = true;
        }
        public override void StopPlugin()
        {
            timer_3_sec.Enabled = false;
            timer_5_sec.Enabled = false;
            timer_10_sec.Enabled = false;
            timer_30_sec.Enabled = false;
        }
        #endregion

        #region Event handlers
        private void timer_3_sec_Elapsed(object source, ElapsedEventArgs e)
        {
            var now = DateTime.Now;

            // periodical actions:
            foreach (var handler in periodicalHandlers)
                handler.TryToExecute(now);

            //timer_3_sec.Enabled = false;
            Run(Timer_3_sec_ElapsedEventHandlers, handler => handler(now));
            //timer_3_sec.Enabled = true;
        }
        private void timer_5_sec_Elapsed(object source, ElapsedEventArgs e)
        {
            var now = DateTime.Now;

            //timer_5_sec.Enabled = false;
            Run(Timer_5_sec_ElapsedEventHandlers, handler => handler(now));
            //timer_5_sec.Enabled = true;
        }
        private void timer_10_sec_Elapsed(object source, ElapsedEventArgs e)
        {
            var now = DateTime.Now;

            //timer_10_sec.Enabled = false;
            Run(Timer_10_sec_ElapsedEventHandlers, handler => handler(now));
            //timer_10_sec.Enabled = true;
        }
        private void timer_30_sec_Elapsed(object source, ElapsedEventArgs e)
        {
            var now = DateTime.Now;

            //timer_30_sec.Enabled = false;
            Run(Timer_30_sec_ElapsedEventHandlers, handler => handler(now));
            //timer_30_sec.Enabled = true;
        }
        #endregion

        #region Private methods
        private void RegisterPeriodicalHandlers()
        {
            var now = DateTime.Now;

            Logger.Info("Register periodical actions at {0:yyyy.MM.dd, HH:mm:ss}", now);

            foreach (var action in PeriodicalActions)
            {
                var handler = new PeriodicalActionState(action.Value, action.Metadata.Interval, now, Logger);
                periodicalHandlers.Add(handler);
            }
        }
        #endregion
    }
}
