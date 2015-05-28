﻿using SmartHub.Core.Plugins;
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
        public class Configuration
        {
            public Guid SensorTemperatureID { get; set; }
            public Guid SensorSwitchID { get; set; }

            public bool IsAutoMode { get; set; }
            public float TemperatureMin { get; set; }
            public float TemperatureMax { get; set; }
            public float TemperatureAlarmMin { get; set; }
            public float TemperatureAlarmMax { get; set; }
            public string TemperatureAlarmMinText { get; set; }
            public string TemperatureAlarmMaxText { get; set; }

            public static Configuration Default
            {
                get
                {
                    return new Configuration()
                    {
                        SensorTemperatureID = Guid.Empty,
                        SensorSwitchID = Guid.Empty,

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
        private const string settingName = "HeaterControllerConfiguration";
        private Configuration configuration;
        #endregion

        #region Properties
        public Configuration ControllerConfiguration
        {
            get
            {
                if (configuration == null)
                {
                    var s = GetSetting(settingName);
                    if (s == null)
                    {
                        configuration = Configuration.Default;

                        s = new AquaControllerSetting() { Id = Guid.NewGuid(), Name = settingName };
                        s.SetValue(configuration);
                        SaveOrUpdate(s);
                    }
                    else
                        configuration = s.GetValue(typeof(Configuration));
                }

                return configuration;
            }
            set
            {
                configuration = value;

                var s = GetSetting(settingName);
                s.SetValue(configuration);
                SaveOrUpdate(s);

                RequestSensorsValues();
            }
        }
        public Sensor SensorTemperature
        {
            get { return mySensors.GetSensor(ControllerConfiguration.SensorTemperatureID); }
        }
        public Sensor SensorSwitch
        {
            get { return mySensors.GetSensor(ControllerConfiguration.SensorSwitchID); }
        }
        #endregion

        #region Public methods
        public override void Init(IServiceContext context)
        {
            base.Init(context);

            //configurationSetting = GetSetting(settingName);

            //if (configurationSetting == null)
            //{
            //    configurationSetting = new AquaControllerSetting()
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = settingName
            //    };

            //    configuration = Configuration.Default;
            //    configurationSetting.SetValue(configuration);
            //    SaveOrUpdate(configurationSetting);
            //}
            //else
            //    configuration = configurationSetting.GetValue(typeof(Configuration));
        }
        #endregion

        #region Private methods
        protected override void RequestSensorsValues()
        {
            mySensors.RequestSensorValue(SensorTemperature, SensorValueType.Temperature);
            mySensors.RequestSensorValue(SensorSwitch, SensorValueType.Switch);
        }
        private void Process(float temperature)
        {
            //if (ControllerConfiguration.IsAutoMode)
            //{
            //    //var switchValue = mySensors.GetLastSensorValue(SensorSwitchHeater);

            //    if (temperature < ControllerConfiguration.TemperatureMin)
            //        mySensors.SetSensorValue(SensorSwitch, SensorValueType.Switch, 1);
            //    else if (temperature > ControllerConfiguration.TemperatureMax)
            //        mySensors.SetSensorValue(SensorSwitch, SensorValueType.Switch, 0);
            //}

            //if (temperature <= ControllerConfiguration.TemperatureAlarmMin)
            //    Context.GetPlugin<SpeechPlugin>().Say(ControllerConfiguration.TemperatureAlarmMinText);
            //else if (temperature >= ControllerConfiguration.TemperatureAlarmMax)
            //    Context.GetPlugin<SpeechPlugin>().Say(ControllerConfiguration.TemperatureAlarmMaxText);
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

            if (mySensors.IsMessageFromSensor(message, SensorTemperature))
                Process(message.PayloadFloat);
        }

        [RunPeriodically(1)]
        private void timer_Elapsed(DateTime now)
        {
            var lastSV = mySensors.GetLastSensorValue(SensorTemperature);
            if (lastSV != null)
                Process(lastSV.Value);
        }
        #endregion
    }
}
