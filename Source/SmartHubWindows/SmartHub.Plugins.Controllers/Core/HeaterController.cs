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
                        TemperatureAlarmMin = 20.0f,
                        TemperatureAlarmMax = 30.0f,
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
            configuration = (ControllerConfiguration)Extensions.FromJson(typeof(ControllerConfiguration), config);
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
        protected override void InitLastValues()
        {
            var lastSV = mySensors.GetLastSensorValue(SensorTemperature);
            lastSensorValue = lastSV != null ? lastSV.Value : (float?)null;
        }
        protected override void Process()
        {
            if (IsAutoMode)
            {
                if (lastSensorValue.HasValue)
                {
                    float value = lastSensorValue.Value;

                    if (value < configuration.TemperatureMin)
                        mySensors.SetSensorValue(SensorSwitch, SensorValueType.Switch, 1);
                    else if (value > configuration.TemperatureMax)
                        mySensors.SetSensorValue(SensorSwitch, SensorValueType.Switch, 0);

                    // voice alarm:
                    if (value <= configuration.TemperatureAlarmMin)
                        Context.GetPlugin<SpeechPlugin>().Say(string.Format("{0}, {1} градусов.", configuration.TemperatureAlarmMinText, value));
                    else if (value >= configuration.TemperatureAlarmMax)
                        Context.GetPlugin<SpeechPlugin>().Say(string.Format("{0}, {1} градусов.", configuration.TemperatureAlarmMaxText, value));
                }
                else
                    RequestSensorsValues();
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
                lastSensorValue = message.PayloadFloat;

            base.MessageReceived(message);
        }
        #endregion
    }
}
