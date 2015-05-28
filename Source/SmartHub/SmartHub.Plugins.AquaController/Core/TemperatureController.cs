using SmartHub.Core.Plugins;
using SmartHub.Plugins.AquaController.Data;
using SmartHub.Plugins.MySensors.Attributes;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.MySensors.Data;
using SmartHub.Plugins.Timer.Attributes;
using System;

namespace SmartHub.Plugins.AquaController.Core
{
    public class TemperatureController : ControllerBase
    {
        public class Configuration
        {
            public Guid SensorTemperatureWaterID { get; set; }
            public Guid SensorSwitchHeaterID { get; set; }
            public float TemperatureMin { get; set; }
            public float TemperatureMax { get; set; }

            public static Configuration Default
            {
                get
                {
                    return new Configuration()
                    {
                        SensorTemperatureWaterID = Guid.Empty,
                        SensorSwitchHeaterID = Guid.Empty,
                        TemperatureMin = 25.0f,
                        TemperatureMax = 26.0f
                    };
                }
            }
        }

        #region Fields
        private const string settingName = "TemperatureControllerConfiguration";
        private Configuration configuration;
        private AquaControllerSetting configurationSetting;
        #endregion

        #region Properties
        public Configuration ControllerConfiguration
        {
            get { return configuration; }
            set
            {
                configuration = value;
                configurationSetting.SetValue(configuration);
                SaveOrUpdate(configurationSetting);

                RequestSensorsValues();
            }
        }
        public Sensor SensorTemperatureWater
        {
            get { return mySensors.GetSensor(configuration.SensorTemperatureWaterID); }
        }
        public Sensor SensorSwitchHeater
        {
            get { return mySensors.GetSensor(configuration.SensorSwitchHeaterID); }
        }
        #endregion

        #region Public methods
        public override void Init(IServiceContext context)
        {
            base.Init(context);

            configurationSetting = GetSetting(settingName);

            if (configurationSetting == null)
            {
                configurationSetting = new AquaControllerSetting()
                {
                    Id = Guid.NewGuid(),
                    Name = settingName
                };

                configuration = Configuration.Default;
                configurationSetting.SetValue(configuration);
                SaveOrUpdate(configurationSetting);
            }
            else
                configuration = configurationSetting.GetValue(typeof(Configuration));
        }
        #endregion

        #region Private methods
        protected override void RequestSensorsValues()
        {
            Sensor sensor;

            if ((sensor = SensorTemperatureWater) != null)
                mySensors.RequestSensorValue(sensor, SensorValueType.Temperature);
            if ((sensor = SensorSwitchHeater) != null)
                mySensors.RequestSensorValue(sensor, SensorValueType.Switch);
        }
        #endregion

        #region Event handlers
        [MySensorsMessage]
        protected override void MessageReceived(SensorMessage message)
        {
            //if (IsMessageFromSensor(message, SensorTemperatureInner) ||
            //    IsMessageFromSensor(message, SensorHumidityInner) ||
            //    IsMessageFromSensor(message, SensorTemperatureOuter) ||
            //    IsMessageFromSensor(message, SensorHumidityOuter) ||
            //    IsMessageFromSensor(message, SensorAtmospherePressure) ||
            //    IsMessageFromSensor(message, SensorForecast))
            //    NotifyForSignalR(new { MsgId = "AquaControllerTileContent", Data = BuildTileContent() });

            if (mySensors.IsMessageFromSensor(message, SensorTemperatureWater))
            {
                var switchValue = mySensors.GetLastSensorValue(SensorSwitchHeater);

                float t = message.PayloadFloat;
                bool on = false;

                if (t < configuration.TemperatureMin)
                    on = true;
                else if (t >= configuration.TemperatureMin && t < configuration.TemperatureMax)
                    on = true;
                else if (t > configuration.TemperatureMax)
                    on = false;

                mySensors.SetSensorValue(SensorSwitchHeater, SensorValueType.Switch, on ? 1 : 0);
            }
        }

        [Timer_5_sec_Elapsed]
        private void timer_Elapsed(DateTime now)
        {
            int a = 0;
            int b = a;
        }
        #endregion
    }
}
