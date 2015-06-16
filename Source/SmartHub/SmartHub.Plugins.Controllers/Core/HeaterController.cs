using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.Controllers.Data;
using SmartHub.Plugins.MySensors;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.MySensors.Data;
using SmartHub.Plugins.Speech;
using System;

namespace SmartHub.Plugins.Controllers.Core
{
    public class HeaterController : ControllerBase
    {
        public class ControllerConfiguration
        {
            public Guid SensorTemperatureID { get; set; }
            public Guid SensorSwitchID { get; set; }
            public float TemperatureCalibration { get; set; }

            public float TemperatureMin { get; set; }
            public float TemperatureMax { get; set; }
            public float TemperatureAlarmMin { get; set; }
            public float TemperatureAlarmMax { get; set; }
            public string TemperatureAlarmMinText { get; set; }
            public string TemperatureAlarmMaxText { get; set; }

            public static ControllerConfiguration Default
            {
                get
                {
                    return new ControllerConfiguration()
                    {
                        SensorTemperatureID = Guid.Empty,
                        SensorSwitchID = Guid.Empty,
                        TemperatureCalibration = 0.0f,

                        TemperatureMin = 25.0f,
                        TemperatureMax = 26.0f,
                        TemperatureAlarmMin = 22.0f,
                        TemperatureAlarmMax = 28.0f,
                        TemperatureAlarmMinText = "Критически низкая температура",
                        TemperatureAlarmMaxText = "Критически высокая температура"
                    };
                }
            }
        }

        #region Fields
        private ControllerConfiguration configuration = null;
        #endregion

        #region Properties
        public Sensor SensorTemperature
        {
            get { return mySensors.GetSensor(configuration.SensorTemperatureID); }
        }
        public Sensor SensorSwitch
        {
            get { return mySensors.GetSensor(configuration.SensorSwitchID); }
        }
        #endregion

        #region Constructor
        public HeaterController(Controller controller)
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
            configuration = (HeaterController.ControllerConfiguration)Extensions.FromJson(typeof(HeaterController.ControllerConfiguration), config);
            controller.SetConfiguration(configuration);
            SaveToDB();
        }
        public override bool IsMyMessage(SensorMessage message)
        {
            return
                MySensorsPlugin.IsMessageFromSensor(message, SensorTemperature) ||
                MySensorsPlugin.IsMessageFromSensor(message, SensorSwitch);
        }
        public override void RequestSensorsValues()
        {
            mySensors.RequestSensorValue(SensorTemperature, SensorValueType.Temperature);
            mySensors.RequestSensorValue(SensorSwitch, SensorValueType.Switch);
        }
        #endregion

        #region Private methods
        protected override void Process(float? value)
        {
            if (IsAutoMode)
            {
                if (value.HasValue)
                {
                    if (value.Value < configuration.TemperatureMin)
                        mySensors.SetSensorValue(SensorSwitch, SensorValueType.Switch, 1);
                    else if (value.Value > configuration.TemperatureMax)
                        mySensors.SetSensorValue(SensorSwitch, SensorValueType.Switch, 0);
                }
                else
                    RequestSensorsValues();
            }

            // voice alarm:
            if (value.HasValue)
            {
                if (value.Value <= configuration.TemperatureAlarmMin)
                    Context.GetPlugin<SpeechPlugin>().Say(string.Format("{0}, {1} градусов.", configuration.TemperatureAlarmMinText, value.Value));
                else if (value.Value >= configuration.TemperatureAlarmMax)
                    Context.GetPlugin<SpeechPlugin>().Say(string.Format("{0}, {1} градусов.", configuration.TemperatureAlarmMaxText, value.Value));
            }
        }
        #endregion

        #region Event handlers
        public override void MessageCalibration(SensorMessage message)
        {
            if (MySensorsPlugin.IsMessageFromSensor(message, SensorTemperature))
                message.PayloadFloat += configuration.TemperatureCalibration;
        }
        public override void MessageReceived(SensorMessage message)
        {
            if (MySensorsPlugin.IsMessageFromSensor(message, SensorTemperature))
                Process(message.PayloadFloat);
            else if (MySensorsPlugin.IsMessageFromSensor(message, SensorSwitch))
            {
                var lastSV = mySensors.GetLastSensorValue(SensorTemperature);
                Process(lastSV != null ? lastSV.Value : (float?)null);
            }
        }
        public override void TimerElapsed(DateTime now)
        {
            var lastSV = mySensors.GetLastSensorValue(SensorTemperature);
            Process(lastSV != null ? lastSV.Value : (float?)null);
        }
        #endregion
    }
}
