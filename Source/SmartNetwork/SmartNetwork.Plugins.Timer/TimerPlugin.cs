using SmartNetwork.Core.Plugins;
using System;
using System.ComponentModel.Composition;
//using System.Diagnostics;
using System.Timers;

namespace SmartNetwork.Plugins.Timer
{
    [Plugin]
    public class TimerPlugin : PluginBase
    {
        private const int TIMER_INTERVAL = 1000;
        private System.Timers.Timer timer;

        [ImportMany("E62C804C-B96B-4CA8-822E-B1725B363534")]
        public Action<DateTime>[] OnEvent { get; set; }

        public override void InitPlugin()
        {
            timer = new System.Timers.Timer(TIMER_INTERVAL);
            timer.Elapsed += OnTimedEvent;
        }
        public override void StartPlugin()
        {
            timer.Enabled = true;
        }
        public override void StopPlugin()
        {
            timer.Enabled = false;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            timer.Enabled = false;
            var now = DateTime.Now;
            Run(OnEvent, x => x(now));

            //Debugger.Launch();
            //Logger.Info("Hello, world!");
            //Logger.Error("Error message");
            timer.Enabled = true;
        }
    }
}
