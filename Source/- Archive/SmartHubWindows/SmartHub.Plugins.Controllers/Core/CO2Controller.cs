using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.Controllers.Data;
using SmartHub.Plugins.MySensors;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.MySensors.Data;
using System;

namespace SmartHub.Plugins.Controllers.Core
{
    public class CO2Controller : ControllerBase
    {
        public class ControllerConfiguration
        {
            public Guid SensorSwitchID { get; set; }
            public Guid SensorLightSwitchID { get; set; }
            public Guid SensorPhID { get; set; }

            public static ControllerConfiguration Default
            {
                get
                {
                    return new ControllerConfiguration()
                    {
                        SensorSwitchID = Guid.Empty,
                        SensorLightSwitchID = Guid.Empty,
                        SensorPhID = Guid.Empty,
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
        public Sensor SensorLightSwitch
        {
            get { return mySensors.GetSensor(configuration.SensorLightSwitchID); }
        }
        public Sensor SensorPh
        {
            get { return mySensors.GetSensor(configuration.SensorPhID); }
        }
        #endregion

        #region Constructor
        public CO2Controller(Controller controller)
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
            configuration = (CO2Controller.ControllerConfiguration)Extensions.FromJson(typeof(CO2Controller.ControllerConfiguration), config);
            controller.SetConfiguration(configuration);
            SaveToDB();
        }
        public override bool IsMyMessage(SensorMessage message)
        {
            return
                MySensorsPlugin.IsMessageFromSensor(message, SensorSwitch) ||
                MySensorsPlugin.IsMessageFromSensor(message, SensorLightSwitch) ||
                MySensorsPlugin.IsMessageFromSensor(message, SensorPh);
        }
        public override void RequestSensorsValues()
        {
            mySensors.RequestSensorValue(SensorSwitch, SensorValueType.Switch);
            mySensors.RequestSensorValue(SensorLightSwitch, SensorValueType.Switch);
            mySensors.RequestSensorValue(SensorPh, SensorValueType.Ph);
        }
        #endregion

        #region Private methods
        protected override void Process()
        {
            if (IsAutoMode)
            {
                //bool isActiveNew = false;






                //mySensors.SetSensorValue(SensorSwitch, SensorValueType.Switch, isActiveNew ? 1 : 0);
            }
        }
        #endregion
    }
}
