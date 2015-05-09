using NHibernate.Mapping.ByCode;
using SmartHub.Core.Plugins;
using SmartHub.Plugins.MySensors;
using SmartHub.Plugins.MySensors.Attributes;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.MySensors.Data;
using SmartHub.Plugins.Timer;
using System;
using System.Diagnostics;

namespace SmartHub.Plugins.AquaController
{
    [Plugin]
    public class AquaControllerPlugin : PluginBase
    {
        #region Fields
        private MySensorsPlugin mySensors;
        #endregion

        #region Plugin overrides
        public override void InitDbModel(ModelMapper mapper)
        {
            //mapper.Class<Setting>(cfg => cfg.Table("AquaController_Settings"));
        }
        public override void InitPlugin()
        {
            mySensors = Context.GetPlugin<MySensorsPlugin>();

            //if (GetSetting("SerialPortName") == null)
            //    Save(new Setting() { Id = Guid.NewGuid(), Name = "SerialPortName", Value = "" });

        }
        public override void StartPlugin()
        {
            InitHeater();
            //InitLight();
            //InitPh();
        }
        //public override void StopPlugin()
        //{
        //    //UninitLight();
        //    //UninitPh();
        //}
        #endregion

        #region Heater
        private Sensor heaterRelay;
        private Sensor heaterTemperatureSensor;
        private float minHeaterTemperature;

        private void InitHeater()
        {
            minHeaterTemperature = 24.0f;

            heaterRelay = mySensors.GetSensor(1, 0);
            if (heaterRelay == null)
                throw new ArgumentNullException("heaterRelay (1, 0)");

            heaterTemperatureSensor = mySensors.GetSensor(2, 0);
            if (heaterTemperatureSensor == null)
                throw new ArgumentNullException("heaterTemperatureSensor (2, 0)");

            SensorValue sv = mySensors.GetSensorValue(heaterTemperatureSensor);
            if (sv != null)
                mySensors.SetSensorValue(heaterRelay, SensorValueType.Switch, sv.Value < minHeaterTemperature ? 1 : 0);
        }

        private void heaterTemperatureSensor_MessageReceived(SensorMessage msg)
        {
            //if (e.PropertyName == "LastValue")
            //    mySensors.SetSensorValue(heaterRelay, SensorValueType.Light, heaterTemperatureSensor.LastValue.Value < minHeaterTemperature ? 1 : 0);
        }
        #endregion

        #region Light
        //private Sensor lightRelay;
        //private Timer lightTimer;
        //private DateTime lightTimeOn;
        //private DateTime lightTimeOff;

        //private void InitLight()
        //{
        //    lightTimeOn = new DateTime(1970, 1, 1, 10, 0, 0);
        //    lightTimeOff = new DateTime(1970, 1, 1, 22, 0, 0);

        //    lightRelay = controller.GetSensor(20, 1);
        //    if (lightRelay == null)
        //        throw new ArgumentNullException("lightRelay (20, 1)");

        //    lightTimer = new Timer(1000 * 5);
        //    lightTimer.Elapsed += lightTimer_Elapsed;
        //    lightTimer.Start();
        //}
        ////private bool relayValue = false;
        //private void lightTimer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    //controller.SetSensorValue(lightRelay, SensorValueType.Light, relayValue ? 1 : 0);
        //    //relayValue = !relayValue;

        //    DateTime now = DateTime.Now;
        //    bool isOn = now.Hour > lightTimeOn.Hour && now.Hour < lightTimeOff.Hour;

        //    controller.SetSensorValue(lightRelay, SensorValueType.Light, isOn ? 1 : 0);
        //}
        //private void UninitLight()
        //{
        //    lightTimer.Stop();
        //    lightTimer.Elapsed -= lightTimer_Elapsed;
        //    lightTimer = null;
        //    lightRelay = null;
        //}
        #endregion

        #region Ph
        //private Sensor phRelay;
        //private Sensor phSensor;
        //private float phNormalValue;

        //private void InitPh()
        //{
        //    phNormalValue = 7.0f;

        //    phRelay = controller.GetSensor(20, 0);
        //    if (phRelay == null)
        //        throw new ArgumentNullException("phRelay (20, 2)");

        //    phSensor = controller.GetSensor(20, 10);
        //    if (phSensor == null)
        //        throw new ArgumentNullException("phSensor (20, 10)");

        //    if (phSensor.LastValue != null)
        //        controller.SetSensorValue(phRelay, SensorValueType.Light, phSensor.LastValue.Value <= phNormalValue ? 1 : 0);

        //    phSensor.PropertyChanged += phSensor_PropertyChanged;
        //}
        //private void phSensor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "LastValue")
        //        controller.SetSensorValue(phRelay, SensorValueType.Light, phSensor.LastValue.Value < phNormalValue ? 1 : 0);
        //}
        //private void UninitPh()
        //{
        //    phSensor.PropertyChanged -= phSensor_PropertyChanged;
        //    phSensor = null;
        //    phRelay = null;
        //}
        #endregion

        #region Water level
        //private Sensor waterInputRelay;
        //private Sensor waterOutputRelay;
        //private Sensor waterSensor;
        //private float waterNormalDistance;


        #endregion

        #region Event handlers
        [MySensorsMessage]
        private void MySensorMessage_Received(SensorMessage msg)
        {
            if (msg.NodeID == heaterRelay.NodeNo && msg.SensorID == heaterRelay.SensorNo)
                heaterTemperatureSensor_MessageReceived(msg);
        }
        [MySensorsConnected]
        private void MySensorsConnected()
        {

        }
        [MySensorsDisconnected]
        private void MySensorsDisconnected()
        {

        }

        float vvv = 0;
        [Timer_3_sec_Elapsed]
        private void timer_1_sec_Elapsed(DateTime now)
        {
            //mySensors.SetSensorValue(heaterRelay, SensorValueType.Switch, vvv);
            //vvv = vvv == 0 ? 1 : 0;
        }
        #endregion
    }
}
