using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.Controllers.Data;
using SmartHub.Plugins.MySensors;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.MySensors.Data;
using SmartHub.Plugins.Speech;
using System;

namespace SmartHub.Plugins.Controllers.Core
{
    public class WaterLevelController : ControllerBase
    {
        public class ControllerConfiguration
        {
            public Guid SensorInSwitchID { get; set; }
            public Guid SensorOutSwitchID { get; set; }
            public Guid SensorDistanceID { get; set; }

            // regular distance:
            public int DistanceMin { get; set; }
            public int DistanceMax { get; set; }
            public int DistanceAlarmMin { get; set; }
            public string DistanceAlarmMinText { get; set; }

            // water exchange:
            public int DistanceExchangeMax { get; set; } // water out max distance
            public string DistanceAlarmMaxText { get; set; }
            public DayOfWeek ExchangeWeekDay { get; set; }
            public DateTime ExchangeTime { get; set; }
            public bool IsExchangeMode { get; set; }

            public static ControllerConfiguration Default
            {
                get
                {
                    return new ControllerConfiguration()
                    {
                        SensorInSwitchID = Guid.Empty,
                        SensorOutSwitchID = Guid.Empty,
                        SensorDistanceID = Guid.Empty,

                        DistanceMin = 2,
                        DistanceMax = 3,
                        DistanceAlarmMin = 1,
                        DistanceAlarmMinText = "Критически высокий уровень воды",

                        ExchangeWeekDay = DayOfWeek.Saturday,
                        ExchangeTime = new DateTime(1970, 1, 1, 14, 0, 0, DateTimeKind.Local),
                        DistanceExchangeMax = 15,
                        DistanceAlarmMaxText = "Критически низкий уровень воды",

                        IsExchangeMode = false
                    };
                }
            }
        }

        #region Fields
        private ControllerConfiguration configuration = null;
        private DateTime lastExchangeTime = DateTime.MinValue;
        #endregion

        #region Properties
        public Sensor SensorInSwitch
        {
            get { return mySensors.GetSensor(configuration.SensorInSwitchID); }
        }
        public Sensor SensorOutSwitch
        {
            get { return mySensors.GetSensor(configuration.SensorOutSwitchID); }
        }
        public Sensor SensorDistance
        {
            get { return mySensors.GetSensor(configuration.SensorDistanceID); }
        }
        #endregion

        #region Constructor
        public WaterLevelController(Controller controller)
            : base (controller)
        {
            if (string.IsNullOrEmpty(controller.Configuration))
            {
                configuration = ControllerConfiguration.Default;
                controller.SetConfiguration(configuration);
            }
            else
                configuration = controller.GetConfiguration(typeof(ControllerConfiguration));
        }
        #endregion

        #region Public methods
        public override void SetConfiguration(string config)
        {
            configuration = (ControllerConfiguration)Extensions.FromJson(typeof(ControllerConfiguration), config);
            controller.SetConfiguration(configuration);
            SaveToDB();
        }
        public override bool IsMyMessage(SensorMessage message)
        {
            return
                MySensorsPlugin.IsMessageFromSensor(message, SensorInSwitch) ||
                MySensorsPlugin.IsMessageFromSensor(message, SensorOutSwitch) ||
                MySensorsPlugin.IsMessageFromSensor(message, SensorDistance);
        }
        public override void RequestSensorsValues()
        {
            mySensors.RequestSensorValue(SensorInSwitch, SensorValueType.Switch);
            mySensors.RequestSensorValue(SensorOutSwitch, SensorValueType.Switch);
            mySensors.RequestSensorValue(SensorDistance, SensorValueType.Distance);
        }
        #endregion

        #region Private methods
        private static DateTime GetDateTime(DateTime time, DateTime now, DateTime lastAlarm)
        {
            //var time = now.Date.AddHours(time.Hour).AddMinutes(time.Minute);

            if (time < lastAlarm || time.AddMinutes(5) < now)
                time = time.AddDays(1);

            return time;
        }
        private static bool CheckTime(DateTime time, DateTime now, DateTime lastAlarm)
        {
            // если прошло время события
            // и от этого времени не прошло 5 минут
            // и событие сегодня еще не произошло
            var date = GetDateTime(time, now, lastAlarm);
            return lastAlarm < date && date < now;
        }
        private void CheckForStartExchange()
        {
            if (!configuration.IsExchangeMode)
            {
                DateTime now = DateTime.Now;
                if (now.DayOfWeek == configuration.ExchangeWeekDay && CheckTime(configuration.ExchangeTime, now, lastExchangeTime))
                {
                    lastExchangeTime = now;
                    configuration.IsExchangeMode = true;
                    SaveToDB();
                }
            }
        }

        protected override void InitLastValues()
        {
            var lastSV = mySensors.GetLastSensorValue(SensorDistance);
            lastSensorValue = lastSV != null ? lastSV.Value : (float?)null;
        }
        protected override void Process()
        {
            if (IsAutoMode)
            {
                if (lastSensorValue.HasValue) // distance
                {
                    float value = lastSensorValue.Value;

                    //var svInSwitch = mySensors.GetLastSensorValue(SensorInSwitch);
                    //var vInSwitch = svInSwitch != null ? svInSwitch.Value : (float?)null;

                    //var svOutSwitch = mySensors.GetLastSensorValue(SensorOutSwitch);
                    //var vOutSwitch = svOutSwitch != null ? svOutSwitch.Value : (float?)null;

                    CheckForStartExchange();

                    if (configuration.IsExchangeMode)
                    {
                        if (value < configuration.DistanceExchangeMax)
                            mySensors.SetSensorValue(SensorOutSwitch, SensorValueType.Switch, 1);
                        else
                        {
                            configuration.IsExchangeMode = false;
                            SaveToDB();
                            mySensors.SetSensorValue(SensorOutSwitch, SensorValueType.Switch, 0);
                        }
                    }
                    else
                    {
                        if (value <= configuration.DistanceMin) // overflow
                            mySensors.SetSensorValue(SensorInSwitch, SensorValueType.Switch, 0); // stop In
                        else if (value > configuration.DistanceMax) // insufficient level
                            mySensors.SetSensorValue(SensorInSwitch, SensorValueType.Switch, 1); // start In
                    }

                    // voice alarm:
                    if (value <= configuration.DistanceAlarmMin)
                        Context.GetPlugin<SpeechPlugin>().Say(string.Format("{0}, {1} сантиметров.", configuration.DistanceAlarmMinText, value));
                    else if (value > configuration.DistanceExchangeMax)
                        Context.GetPlugin<SpeechPlugin>().Say(string.Format("{0}, {1} сантиметров.", configuration.DistanceAlarmMaxText, value));
                }
                else
                    RequestSensorsValues();
            }
        }
        #endregion

        #region Event handlers
        public override void MessageReceived(SensorMessage message)
        {
            if (MySensorsPlugin.IsMessageFromSensor(message, SensorDistance))
                lastSensorValue = message.PayloadFloat;

            base.MessageReceived(message);
        }
        #endregion
    }
}
