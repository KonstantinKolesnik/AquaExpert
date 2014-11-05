using MySensors.Controllers;
using MySensors.Controllers.Automation;
using MySensors.Controllers.Core;
using System;

namespace MySensors.AutomationServices
{
    public class AquaControllerService : IAutomationService
    {
        private Controller controller;

        private Sensor heaterRelay;
        private Sensor heaterTemperatureSensor;
        private float minHeaterTemperature;






        public void Start(Controller controller)
        {
            this.controller = controller;

            InitHeater();

        }
        public void Stop()
        {

        }

        private void InitHeater()
        {
            minHeaterTemperature = 24.0f;

            heaterRelay = controller.GetSensor(20, 0);
            if (heaterRelay == null)
                throw new ArgumentNullException("heaterRelay");

            heaterTemperatureSensor = controller.GetSensor(20, 7);
            if (heaterTemperatureSensor == null)
                throw new ArgumentNullException("heaterTemperatureSensor");

            heaterTemperatureSensor.PropertyChanged += ((sender, e) =>
            {
                if (e.PropertyName == "LastValue")
                    controller.SetSensorValue(heaterRelay, SensorValueType.Light, heaterTemperatureSensor.LastValue.Value < minHeaterTemperature ? 1 : 0);
            });
        }
    }
}
