using GHI.Premium.Hardware;
using MFE.Net.Udp;
using Microsoft.SPOT.Time;
using System;
using System.Net;
using System.Threading;

namespace AquaExpert.Server
{
    static class TimeManager
    {
        public static DateTime CurrentTime
        {
            get { return RealTimeClock.GetTime(); }
        }
        public static bool IsTimeValid
        {
            //Date = {01/01/2007 00:00:00}
            get { return CurrentTime > new DateTime(2014, 1, 1); }
        }

        static TimeManager()
        {
            FixedTimeService.Settings = new TimeServiceSettings()
            {
                AutoDayLightSavings = true,
                ForceSyncAtWakeUp = true,
                RefreshTime = 12 * 60 * 60, // twice a day
                //public uint Tolerance = ?;
            };
            FixedTimeService.SetTimeZoneOffset(+3 * 60); /// PST
            //FixedTimeService.SetDst("Mar Sun>=8 @2", "Nov Sun>=1 @2", 60); // US DST
            FixedTimeService.SetDst("Mar lastSun @1", "Oct lastSun @1", 60); // EU DST

            FixedTimeService.SystemTimeChanged += new SystemTimeChangedEventHandler(TimeService_SystemTimeChanged);
            FixedTimeService.TimeSyncFailed += new TimeSyncFailedEventHandler(TimeService_TimeSyncFailed);
            // New event: called when after we check the time (even if we don't end up changing the time)
            FixedTimeService.SystemTimeChecked += new SystemTimeChangedEventHandler(TimeService_SystemTimeChecked);
        }

        public static void Start()
        {
            new Thread(() =>
            {
                // "nist1-sj.ustiming.org,us.pool.ntp.org,clock.tricity.wsu.edu,clock-1.cs.cmu.edu,time-a.nist.gov"
                FixedTimeService.Settings.PrimaryServer = Dns.GetHostEntry("nist1-sj.ustiming.org").AddressList[0].GetAddressBytes();
                FixedTimeService.Settings.AlternateServer = Dns.GetHostEntry("pool.ntp.org").AddressList[0].GetAddressBytes();

                // wait for internet connection
                while (IPAddress.GetDefaultLocalAddress() == IPAddress.Any)
                    Thread.Sleep(1000);

                FixedTimeService.Start();
            }).Start();
        }
        public static void Stop()
        {
            FixedTimeService.Stop();
        }

        #region Event handlers
        private static void TimeService_SystemTimeChanged(object sender, SystemTimeChangedEventArgs e)
        {
        }
        private static void TimeService_SystemTimeChecked(object sender, SystemTimeChangedEventArgs e)
        {
            RealTimeClock.SetTime(e.EventTime);
        }
        private static void TimeService_TimeSyncFailed(object sender, TimeSyncFailedEventArgs e)
        {
        }
        #endregion
    }
}
