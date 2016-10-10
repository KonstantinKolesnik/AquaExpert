//using NLog;
using System;
using System.Threading.Tasks;

namespace SmartHub.UWP.Plugins.Timer
{
    class PeriodicalAction
    {
        //private readonly Logger logger;
        private readonly Action<DateTime> action;
        private readonly int interval;
        private DateTime lastRun;
        private static readonly Random random = new Random();
        private readonly object lockObject = new object();

        public PeriodicalAction(Action<DateTime> action, int interval, DateTime now/*, Logger logger*/)
        {
            //logger.Info("Register periodical action: {0} ({1})", action.Method, action.Method.DeclaringType);

            if (interval < 1)
                throw new Exception(string.Format("Wrong interval: {0} sec", interval));

            this.interval = interval;
            this.action = action;
            //this.logger = logger;

            // offset
            int offset = random.Next(interval);
            //logger.Info("Interval: {0} seconds, random offset: {1} seconds", interval, offset);
            lastRun = now.AddSeconds(offset - interval);
        }

        public void TryToExecute(DateTime now)
        {
            if (lastRun.AddSeconds(interval) < now)
                lock (lockObject)
                    if (lastRun.AddSeconds(interval) < now)
                    {
                        lastRun = now;

                        //string taskInfo = string.Format("{0}, {1} at {2}", action.Method, action.Method.DeclaringType, now);

                        Task.Run(() =>
                        {
                            try
                            {
                                //logger.Info("Run periodical task {0}", taskInfo);
                                action(now);
                                //logger.Info("Task is completed: {0}", taskInfo);
                            }
                            catch (Exception ex)
                            {
                                //logger.Error(ex, string.Format("Error when running periodical task {0}", taskInfo));
                            }
                        });
                    }
        }
    }
}
