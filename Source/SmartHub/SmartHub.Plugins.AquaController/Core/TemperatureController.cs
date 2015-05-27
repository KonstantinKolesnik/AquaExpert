using SmartHub.Core.Plugins;
using SmartHub.Plugins.AquaController.Data;
using SmartHub.Plugins.MySensors;
using SmartHub.Plugins.MySensors.Attributes;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.MySensors.Data;
using SmartHub.Plugins.Timer.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.Plugins.AquaController.Core
{
    public class TemperatureController : ControllerBase
    {
        public class Configuration
        {
            public Guid SensorTemperatureID { get; set; }
            public Guid SensorSwitchID { get; set; }
            public float TemperatureMin { get; set; }
            public float TemperatureMax { get; set; }

            public static Configuration Default
            {
                get
                {
                    return new Configuration()
                    {
                        SensorTemperatureID = Guid.Empty,
                        SensorSwitchID = Guid.Empty,
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

            if ((sensor = SensorTemperature) != null)
                mySensors.RequestSensorValue(sensor, SensorValueType.Temperature);
            if ((sensor = SensorSwitch) != null)
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

            if (IsMessageFromSensor(message, SensorTemperature))
            {
                var switchValue = GetLastSensorValue(SensorSwitch);

                float t = message.PayloadFloat;
                bool on = false;

                if (t < configuration.TemperatureMin)
                    on = true;
                else if (t >= configuration.TemperatureMin && t < configuration.TemperatureMax)
                    on = true;
                else if (t > configuration.TemperatureMax)
                    on = false;

                mySensors.SetSensorValue(SensorSwitch, SensorValueType.Switch, on ? 1 : 0);
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
