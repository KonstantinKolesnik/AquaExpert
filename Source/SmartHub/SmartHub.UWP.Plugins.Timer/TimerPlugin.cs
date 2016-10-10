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
        private readonly List<PeriodicalAction> periodicalHandlers = new List<PeriodicalAction>();
        #endregion

        #region Import
        [ImportMany(Timer_10sec_ElapsedAttribute.ContractID)]
        public Action<DateTime>[] Timer_ElapsedEventHandlers
        {
            get; set;
        }

        [ImportMany(RunPeriodicallyAttribute.ContractID)]
        public Lazy<Action<DateTime>, IRunPeriodicallyAttribute>[] PeriodicalHandlers
        {
            get; set;
        }
        #endregion

        #region Plugin ovverrides
        public override void InitPlugin()
        {
            timer = new System.Threading.Timer(timerCallback, null, TimeSpan.FromSeconds(TIMER_INTERVAL).Milliseconds, Timeout.Infinite);
            RegisterPeriodicalHandlers();
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
                foreach (var handler in periodicalHandlers)
                    handler.TryToExecute(now);

                Run(Timer_ElapsedEventHandlers, handler => handler(now));


                // do some work not connected with UI:
                //await Window.Current.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                //    () => {
                //    // do some work on UI here;
                //});

                //isTimerActive = true;
            }
        }
        #endregion

        #region Private methods
        private void RegisterPeriodicalHandlers()
        {
            var now = DateTime.Now;

            //Logger.Info("Register periodical actions at {0:yyyy.MM.dd, HH:mm:ss}", now);

            foreach (var action in PeriodicalHandlers)
                periodicalHandlers.Add(new PeriodicalAction(action.Value, action.Metadata.Interval, now/*, Logger*/));
        }
        #endregion

    }
}
