using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.AquaController.Data;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.MySensors.Data;
using System;
using System.Collections.Generic;

namespace SmartHub.Plugins.AquaController.Core
{
    public struct DatePoint
    {
        public int Hours;
        public int Minutes;
    }
    public struct DateRange
    {
        public DatePoint From;
        public DatePoint To;
    }

    public class SwitchController : ControllerBase
    {
        public class ControllerConfiguration
        {
            public Guid SensorSwitchID { get; set; }
            public List<DateRange> ActivePeriods { get; set; }

            public bool IsAutoMode { get; set; }

            public static ControllerConfiguration Default
            {
                get
                {
                    return new ControllerConfiguration()
                    {
                        SensorSwitchID = Guid.Empty,
                        ActivePeriods = new List<DateRange>(),
                        
                        IsAutoMode = true
                    };
                }
            }
        }

        #region Fields
        private ControllerConfiguration configuration = null;
        #endregion

        #region Properties
        public Sensor SensorSwitch
        {
            get { return mySensors.GetSensor(configuration.SensorSwitchID); }
        }
        #endregion

        #region Constructor
        public SwitchController(Controller controller)
            : base (controller)
        {
            if (string.IsNullOrEmpty(controller.Configuration))
            {
                configuration = ControllerConfiguration.Default;
                controller.SerializeConfiguration(configuration);
            }
            else
                configuration = controller.DeserializeConfiguration(typeof(ControllerConfiguration));
        }
        #endregion

        #region Public methods
        public override void SetConfiguration(string config)
        {
            configuration = (SwitchController.ControllerConfiguration)Extensions.FromJson(typeof(SwitchController.ControllerConfiguration), config);
            controller.SerializeConfiguration(configuration);
            Save();
        }
        public override bool IsMyMessage(SensorMessage message)
        {
            return mySensors.IsMessageFromSensor(message, SensorSwitch);
        }
        public override void RequestSensorsValues()
        {
            mySensors.RequestSensorValue(SensorSwitch, SensorValueType.Switch);
        }
        #endregion

        #region Private methods
        private static bool IsInRange(DateTime dt, DateRange range)
        {
            return (dt.Hour >= range.From.Hours && dt.Minute >= range.From.Minutes) &&
                   (dt.Hour <= range.To.Hours && dt.Minute <= range.To.Minutes);
        }

        protected override void Process(float? value)
        {
            if (configuration.IsAutoMode)
            {
                bool isActive = false;
                DateTime now = DateTime.Now;

                foreach (var range in configuration.ActivePeriods)
                    isActive |= IsInRange(now, range);

                mySensors.SetSensorValue(SensorSwitch, SensorValueType.Switch, isActive ? 1 : 0);
            }
        }
        #endregion

        #region Event handlers
        public override void TimerElapsed(DateTime now)
        {
            Process(null);
        }
        #endregion
    }
}
