using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.AquaController.Data;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.MySensors.Data;
using System;
using System.Collections.Generic;

namespace SmartHub.Plugins.AquaController.Core
{
    public struct Period
    {
        public DateTime From;
        public DateTime To;
        public bool IsActive;
    }

    public class SwitchController : ControllerBase
    {
        public class ControllerConfiguration
        {
            public Guid SensorSwitchID { get; set; }
            public List<Period> ActivePeriods { get; set; }

            public bool IsAutoMode { get; set; }

            public static ControllerConfiguration Default
            {
                get
                {
                    return new ControllerConfiguration()
                    {
                        SensorSwitchID = Guid.Empty,
                        ActivePeriods = new List<Period>(),
                        
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
        private static bool IsInRange(DateTime dt, Period range)
        {
            return (dt.Hour >= range.From.Hour && dt.Minute >= range.From.Minute) &&
                   (dt.Hour <= range.To.Hour && dt.Minute <= range.To.Minute);
        }

        protected override void Process(float? value)
        {
            if (configuration.IsAutoMode)
            {
                bool isActive = false;
                DateTime now = DateTime.UtcNow;

                foreach (var range in configuration.ActivePeriods)
                    isActive |= (range.IsActive && IsInRange(now, range));

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
