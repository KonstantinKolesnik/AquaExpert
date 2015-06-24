using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.Controllers.Data;
using SmartHub.Plugins.MySensors;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.MySensors.Data;
using SmartHub.Plugins.Speech;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.Plugins.Controllers.Core
{
    public class WaterLevelController : ControllerBase
    {
        public class ControllerConfiguration
        {
            public Guid SensorInSwitchID { get; set; }
            public Guid SensorOutSwitchID { get; set; }
            public Guid SensorDistanceID { get; set; }

            public int DistanceMin { get; set; }
            public int DistanceMax { get; set; }
            public int DistanceExchangeMax { get; set; }

            public int DistanceAlarmMin { get; set; }
            public string DistanceAlarmMinText { get; set; }


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

                        DistanceMin = 3,
                        DistanceMax = 5,
                        DistanceExchangeMax = 12,

                        DistanceAlarmMin = 2,
                        DistanceAlarmMinText = "Критически высокий уровень воды",

                        IsExchangeMode = false
                    };
                }
            }
        }

        #region Fields
        private ControllerConfiguration configuration = null;

        private float distance = 20;
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
            configuration = (WaterLevelController.ControllerConfiguration)Extensions.FromJson(typeof(WaterLevelController.ControllerConfiguration), config);
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
        protected override void Process(float? value)
        {
            if (IsAutoMode)
            {
                if (value.HasValue) // distance
                {
                    //var distance = value.Value;

                    var svInSwitch = mySensors.GetLastSensorValue(SensorInSwitch);
                    var vInSwitch = svInSwitch != null ? svInSwitch.Value : (float?)null;

                    var svOutSwitch = mySensors.GetLastSensorValue(SensorOutSwitch);
                    var vOutSwitch = svOutSwitch != null ? svOutSwitch.Value : (float?)null;

                    if (configuration.IsExchangeMode)
                    {


                    }
                    else
                    {
                        if (distance <= configuration.DistanceMin) // overflow
                        {
                            if (vInSwitch == 1)
                                mySensors.SetSensorValue(SensorInSwitch, SensorValueType.Switch, 0); // stop In
                            else
                                mySensors.SetSensorValue(SensorOutSwitch, SensorValueType.Switch, 1); // start Out
                        }
                        else // insufficient level
                        {
                            if (vOutSwitch == 1)
                                mySensors.SetSensorValue(SensorOutSwitch, SensorValueType.Switch, 0); // stop Out
                            else
                                mySensors.SetSensorValue(SensorInSwitch, SensorValueType.Switch, 1); // start In
                        }

                        Debug.WriteLine();
                    }
                }
                else
                    RequestSensorsValues();
            }

            // voice alarm:
            if (value.HasValue)
            {
                if (value.Value <= configuration.DistanceAlarmMin)
                    Context.GetPlugin<SpeechPlugin>().Say(string.Format("{0}, {1} сантиметров.", configuration.DistanceAlarmMinText, value.Value));
                //else if (value.Value >= configuration.DistanceAlarmMax)
                //    Context.GetPlugin<SpeechPlugin>().Say(string.Format("{0}, {1} сантиметров.", configuration.DistanceAlarmMaxText, value.Value));
            }
        }
        #endregion

        #region Event handlers
        public override void MessageReceived(SensorMessage message)
        {
            if (MySensorsPlugin.IsMessageFromSensor(message, SensorDistance))
                Process(message.PayloadFloat);
            else if (MySensorsPlugin.IsMessageFromSensor(message, SensorInSwitch) || MySensorsPlugin.IsMessageFromSensor(message, SensorOutSwitch))
            {
                var lastSV = mySensors.GetLastSensorValue(SensorDistance);
                Process(lastSV != null ? lastSV.Value : (float?)null);
            }
        }
        public override void TimerElapsed(DateTime now)
        {
            //var lastSV = mySensors.GetLastSensorValue(SensorDistance);
            //Process(lastSV != null ? lastSV.Value : (float?)null);

            Process(distance);
        }
        #endregion
    }
}
