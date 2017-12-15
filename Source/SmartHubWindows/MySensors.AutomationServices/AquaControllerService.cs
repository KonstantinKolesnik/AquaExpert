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
        #region Fields
        private Controller controller;
        #endregion

        #region Interface
        public object Parameters { get; set; }

        public void Start(Controller controller)
        {
            this.controller = controller;

            InitHeater();
            InitLight();
            InitPh();
        }
        public void Stop()
        {
            UninitHeater();
            UninitLight();
            UninitPh();
        }
        #endregion

        #region Heater
        private Sensor heaterRelay;
        private Sensor heaterTemperatureSensor;
        private float minHeaterTemperature;

        private void InitHeater()
        {
            minHeaterTemperature = 24.0f;

            heaterRelay = controller.GetSensor(20, 0);
            if (heaterRelay == null)
                throw new ArgumentNullException("heaterRelay (20, 0)");

            heaterTemperatureSensor = controller.GetSensor(20, 7);
            if (heaterTemperatureSensor == null)
                throw new ArgumentNullException("heaterTemperatureSensor (20, 8)");

            if (heaterTemperatureSensor.LastValue != null)
                controller.SetSensorValue(heaterRelay, SensorValueType.Light, heaterTemperatureSensor.LastValue.Value < minHeaterTemperature ? 1 : 0);

            heaterTemperatureSensor.PropertyChanged += heaterTemperatureSensor_PropertyChanged;
        }
        private void heaterTemperatureSensor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LastValue")
                controller.SetSensorValue(heaterRelay, SensorValueType.Light, heaterTemperatureSensor.LastValue.Value < minHeaterTemperature ? 1 : 0);
        }
        private void UninitHeater()
        {
            heaterTemperatureSensor.PropertyChanged -= heaterTemperatureSensor_PropertyChanged;
            heaterTemperatureSensor = null;
            heaterRelay = null;
        }
        #endregion

        #region Light
        private Sensor lightRelay;
        private Timer lightTimer;
        private DateTime lightTimeOn;
        private DateTime lightTimeOff;

        private void InitLight()
        {
            lightTimeOn = new DateTime(1970, 1, 1, 10, 0, 0);
            lightTimeOff = new DateTime(1970, 1, 1, 22, 0, 0);

            lightRelay = controller.GetSensor(20, 1);
            if (lightRelay == null)
                throw new ArgumentNullException("lightRelay (20, 1)");

            lightTimer = new Timer(1000 * 5);
            lightTimer.Elapsed += lightTimer_Elapsed;
            lightTimer.Start();
        }
        //private bool relayValue = false;
        private void lightTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //controller.SetSensorValue(lightRelay, SensorValueType.Light, relayValue ? 1 : 0);
            //relayValue = !relayValue;

            DateTime now = DateTime.Now;
            bool isOn = now.Hour > lightTimeOn.Hour && now.Hour < lightTimeOff.Hour;

            controller.SetSensorValue(lightRelay, SensorValueType.Light, isOn ? 1 : 0);
        }
        private void UninitLight()
        {
            lightTimer.Stop();
            lightTimer.Elapsed -= lightTimer_Elapsed;
            lightTimer = null;
            lightRelay = null;
        }
        #endregion

        #region Ph
        private Sensor phRelay;
        private Sensor phSensor;
        private float phNormalValue;

        private void InitPh()
        {
            phNormalValue = 7.0f;

            phRelay = controller.GetSensor(20, 0);
            if (phRelay == null)
                throw new ArgumentNullException("phRelay (20, 2)");

            phSensor = controller.GetSensor(20, 10);
            if (phSensor == null)
                throw new ArgumentNullException("phSensor (20, 10)");

            if (phSensor.LastValue != null)
                controller.SetSensorValue(phRelay, SensorValueType.Light, phSensor.LastValue.Value <= phNormalValue ? 1 : 0);

            phSensor.PropertyChanged += phSensor_PropertyChanged;
        }
        private void phSensor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LastValue")
                controller.SetSensorValue(phRelay, SensorValueType.Light, phSensor.LastValue.Value < phNormalValue ? 1 : 0);
        }
        private void UninitPh()
        {
            phSensor.PropertyChanged -= phSensor_PropertyChanged;
            phSensor = null;
            phRelay = null;
        }
        #endregion

        #region Water level
        private Sensor waterInputRelay;
        private Sensor waterOutputRelay;
        private Sensor waterSensor;
        private float waterNormalDistance;


        #endregion
    }
}
