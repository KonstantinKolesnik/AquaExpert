using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.Timer.Attributes;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Threading;

namespace SmartHub.UWP.Plugins.Timer
{
    //[Plugin]
    public class TimerPlugin : PluginBase
    {
        #region Fields
        private const double TIMER_INTERVAL = 10;
        private System.Threading.Timer timer;
        private bool isTimerActive = false;
        private readonly List<PeriodicalAction> periodicalActions = new List<PeriodicalAction>();
        #endregion

        #region Imports
        //[ImportMany(Timer_10sec_ElapsedAttribute.ContractID)]
        //public Action<DateTime>[] Timer_ElapsedEventHandlers
        //{
        //    get; set;
        //}
        [ImportMany(RunPeriodicallyAttribute.ContractID)]
        public Lazy<Action<DateTime>, RunPeriodicallyAttribute>[] PeriodicalHandlers
        {
            get; set;
        }
        #endregion

        #region Plugin ovverrides
        public override void InitPlugin()
        {
            timer = new System.Threading.Timer(timerCallback, null, TimeSpan.FromSeconds(TIMER_INTERVAL).Milliseconds, Timeout.Infinite);

            // register periodical actions:
            var now = DateTime.Now;
            //Logger.Info("Register periodical actions at {0:yyyy.MM.dd, HH:mm:ss}", now);
            foreach (var handler in PeriodicalHandlers)
                periodicalActions.Add(new PeriodicalAction(handler.Value, handler.Metadata.Interval, now/*, Logger*/));
        }
        public override void StartPlugin()
        {
            isTimerActive = true;
        }
        public override void StopPlugin()
        {
            isTimerActive = false;
        }
        #endregion

        #region Event handlers
        private /*async*/ void timerCallback(object state)
        {
            if (isTimerActive)
            {
                //isTimerActive = false;

                var now = DateTime.Now;

                // periodical actions
                foreach (var handler in periodicalActions)
                    handler.TryToExecute(now);

                //Run(Timer_ElapsedEventHandlers, handler => handler(now));

                // do some work not connected with UI:
                //await Window.Current.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                //    () => {
                //    // do some work on UI here;
                //});

                //isTimerActive = true;
            }
        }
        #endregion
    }
}
