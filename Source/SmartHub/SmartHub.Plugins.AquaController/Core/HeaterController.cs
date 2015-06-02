using SmartHub.Core.Plugins;
using SmartHub.Plugins.AquaController.Data;
using SmartHub.Plugins.MySensors.Attributes;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.MySensors.Data;
using SmartHub.Plugins.Speech;
using SmartHub.Plugins.Timer.Attributes;
using System;

namespace SmartHub.Plugins.AquaController.Core
{
    public class HeaterController : ControllerBase
    {
        public class ControllerConfiguration
        {
            public Guid SensorTemperatureID { get; set; }
            public Guid SensorSwitchID { get; set; }
            public float TemperatureCalibration { get; set; }

            public bool IsAutoMode { get; set; }
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

                        IsAutoMode = true,
                        TemperatureMin = 25.0f,
                        TemperatureMax = 26.0f,
                        TemperatureAlarmMin = 22.0f,
                        TemperatureAlarmMax = 28.0f,
                        TemperatureAlarmMinText = "Критически холодная вода в аквариуме",
                        TemperatureAlarmMaxText = "Критически горячая вода в аквариуме"
                    };
                }
            }
        }

        #region Fields
        private ControllerConfiguration configuration = null;
        #endregion

        #region Properties
        //public ControllerConfiguration ControllerConfiguration
        //{
        //    get { return configuration; }
        //    set
        //    {
        //        configuration = value;

        //        SerializeConfiguration(configuration);
        //        Save();

        //        RequestSensorsValues();
        //    }
        //}
        public Sensor SensorTemperature
        {
            get { return mySensors.GetSensor(configuration.SensorTemperatureID); }
        }
        public Sensor SensorSwitch
        {
            get { return mySensors.GetSensor(configuration.SensorSwitchID); }
        }
        #endregion

        #region Public methods
        public override void Init(IServiceContext context)
        {
            base.Init(context);

            //var s = GetSetting();
            //if (s == null)
            //{
            //    configuration = Configuration.Default;

            //    s = new AquaControllerSetting() { Id = Guid.NewGuid(), Name = SettingName };
            //    s.SetValue(configuration);
            //    SaveSetting(s);
            //}
            //else
            if (configuration == null)
                configuration = DeserializeConfiguration(typeof(ControllerConfiguration));

        }
        public override bool IsMyMessage(SensorMessage message)
        {
            return
                mySensors.IsMessageFromSensor(message, SensorTemperature) ||
                mySensors.IsMessageFromSensor(message, SensorSwitch);
        }
        public override void SetDefaultConfiguration()
        {
            configuration = ControllerConfiguration.Default;
            SerializeConfiguration(configuration);
        }
        #endregion

        #region Private methods
        protected override void RequestSensorsValues()
        {
            mySensors.RequestSensorValue(SensorTemperature, SensorValueType.Temperature);
            mySensors.RequestSensorValue(SensorSwitch, SensorValueType.Switch);
        }
        protected override void Process(float value)
        {
            if (configuration.IsAutoMode)
            {
                //var switchValue = mySensors.GetLastSensorValue(SensorSwitchHeater);

                if (value < configuration.TemperatureMin)
                    mySensors.SetSensorValue(SensorSwitch, SensorValueType.Switch, 1);
                else if (value > configuration.TemperatureMax)
                    mySensors.SetSensorValue(SensorSwitch, SensorValueType.Switch, 0);
            }

            if (value <= configuration.TemperatureAlarmMin)
                Context.GetPlugin<SpeechPlugin>().Say(configuration.TemperatureAlarmMinText + string.Format("{0} градусов.", value));
            else if (value >= configuration.TemperatureAlarmMax)
                Context.GetPlugin<SpeechPlugin>().Say(configuration.TemperatureAlarmMaxText + string.Format("{0} градусов.", value));
        }
        #endregion

        #region Event handlers
        public override void MessageCalibration(SensorMessage message)
        {
            if (mySensors.IsMessageFromSensor(message, SensorTemperature))
                message.PayloadFloat += configuration.TemperatureCalibration;
        }
        public override void MessageReceived(SensorMessage message)
        {
            if (mySensors.IsMessageFromSensor(message, SensorTemperature))
                Process(message.PayloadFloat);
        }
        public override void TimerElapsed(DateTime now)
        {
            var lastSV = mySensors.GetLastSensorValue(SensorTemperature);
            if (lastSV != null)
                Process(lastSV.Value);
        }
        #endregion
    }
}
