using SmartNetwork.Core.Plugins;
using System;
using System.ComponentModel.Composition;
using System.Timers;

namespace SmartNetwork.Plugins.Timer
{
    [Plugin]
    public class TimerPlugin : PluginBase
    {
        #region Fields
        private const int TIMER_INTERVAL = 5000;
        private System.Timers.Timer timer;
        #endregion

        #region Import
        [ImportMany("E62C804C-B96B-4CA8-822E-B1725B363534")]
        public Action<DateTime>[] OnEvent { get; set; }
        #endregion

        #region Plugin ovverrides
        public override void InitPlugin()
        {
            timer = new System.Timers.Timer(TIMER_INTERVAL);
            timer.Elapsed += timer_Elapsed;
        }
        public override void StartPlugin()
        {
            timer.Enabled = true;
        }
        public override void StopPlugin()
        {
            timer.Enabled = false;
        }
        #endregion

        #region Event handlers
        private void timer_Elapsed(object source, ElapsedEventArgs e)
        {
            //timer.Enabled = false;
            Run(OnEvent, x => x(DateTime.Now));
            //timer.Enabled = true;
        }
        #endregion
    }
}
