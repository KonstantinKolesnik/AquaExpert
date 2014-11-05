using MySensors.Controllers;
using MySensors.Controllers.Automation;
using MySensors.Controllers.Core;
using System;
using System.ComponentModel;
using System.Timers;

namespace MySensors.AutomationServices
{
    public class AquaControllerService : IAutomationService
    {
        private Controller controller;

        private Sensor heaterRelay;
        private Sensor heaterTemperatureSensor;
        private float minHeaterTemperature;

        private Sensor relay;
        private bool relayValue = false;
        private Timer timer = new Timer(2000);



        public void Start(Controller controller)
        {
            this.controller = controller;

            InitHeater();

            relay = controller.GetSensor(20, 6);
            timer.Elapsed += ((sender, e) => {
                controller.SetSensorValue(relay, SensorValueType.Light, relayValue ? 1 : 0);
                relayValue = !relayValue;
            });
            timer.Start();
        }
        public void Stop()
        {
            heaterRelay = null;
            heaterTemperatureSensor = null;

            timer.Stop();
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

            controller.SetSensorValue(heaterRelay, SensorValueType.Light, heaterTemperatureSensor.LastValue.Value < minHeaterTemperature ? 1 : 0);
            //heaterTemperatureSensor.PropertyChanged += ((sender, e) =>
            //{
            //    if (e.PropertyName == "LastValue")
            //        controller.SetSensorValue(heaterRelay, SensorValueType.Light, heaterTemperatureSensor.LastValue.Value < minHeaterTemperature ? 1 : 0);
            //});
            heaterTemperatureSensor.PropertyChanged += heaterTemperatureSensor_PropertyChanged;
        }

        void heaterTemperatureSensor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LastValue")
                controller.SetSensorValue(heaterRelay, SensorValueType.Light, heaterTemperatureSensor.LastValue.Value < minHeaterTemperature ? 1 : 0);
        }
    }
}
