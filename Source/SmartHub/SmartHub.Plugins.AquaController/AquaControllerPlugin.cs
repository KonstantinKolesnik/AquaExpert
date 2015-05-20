using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using SmartHub.Core.Plugins;
using SmartHub.Plugins.MySensors;
using SmartHub.Plugins.MySensors.Attributes;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.MySensors.Data;
using SmartHub.Plugins.Timer;
using SmartHub.Plugins.Timer.Attributes;
using SmartHub.Plugins.WebUI.Attributes;
using System;
using System.Linq;

namespace SmartHub.Plugins.AquaController
{
    [AppSection("Аква-контроллер", SectionType.Common, "/webapp/aquacontroller/module.js", "SmartHub.Plugins.AquaController.Resources.js.module.js", TileTypeFullName = "SmartHub.Plugins.AquaController.AquaControllerTile")]




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
            InitPh();
            //InitLight();
        }
        //public override void StopPlugin()
        //{
        //    //UninitLight();
        //    //UninitPh();
        //}
        #endregion

        private SmartHub.Plugins.AquaController.Data.Setting GetSetting(string name)
        {
            using (var session = Context.OpenSession())
                return session.Query<SmartHub.Plugins.AquaController.Data.Setting>().FirstOrDefault(setting => setting.Name == name);
        }

        #region Lines
        #region Heater
        private Sensor heaterRelay;
        private Sensor heaterSensor;
        private float minHeaterTemperature;

        private void InitHeater()
        {
            minHeaterTemperature = 24.0f;

            heaterRelay = mySensors.GetSensor(1, 0);
            heaterSensor = mySensors.GetSensor(2, 0);
        }

        [MySensorsConnected]
        private void heater_Connected()
        {
            if (heaterSensor != null)
                mySensors.RequestSensorValue(heaterSensor, SensorValueType.Temperature);
            if (heaterRelay != null)
                mySensors.RequestSensorValue(heaterRelay, SensorValueType.Switch);
        }

        [MySensorsMessage]
        private void heater_MessageReceived(SensorMessage msg)
        {
            //if (heaterRelay != null && heaterSensor != null && msg.NodeID == heaterSensor.NodeNo && msg.SensorID == heaterSensor.SensorNo)
            //    mySensors.SetSensorValue(heaterRelay, SensorValueType.Switch, msg.PayloadFloat < minHeaterTemperature ? 1 : 0);
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
        private Sensor phRelay;
        private Sensor phSensor;
        private float phNormalValue;

        private void InitPh()
        {
            phNormalValue = 7.0f;

            phRelay = mySensors.GetSensor(1, 1);
            phSensor = mySensors.GetSensor(2, 1);
        }

        [MySensorsConnected]
        private void ph_Connected()
        {
            if (phSensor != null)
                mySensors.RequestSensorValue(phSensor, SensorValueType.Ph);
            if (phRelay != null)
                mySensors.RequestSensorValue(phRelay, SensorValueType.Switch);
        }

        [MySensorsMessage]
        private void ph_MessageReceived(SensorMessage msg)
        {
            //if (phRelay != null && phSensor != null && msg.NodeID == phSensor.NodeNo && msg.SensorID == phSensor.SensorNo)
            //    mySensors.SetSensorValue(phRelay, SensorValueType.Switch, msg.PayloadFloat < phNormalValue ? 1 : 0);
        }
        #endregion

        #region Water level
        //private Sensor waterInputRelay;
        //private Sensor waterOutputRelay;
        //private Sensor waterSensor;
        //private float waterNormalDistance;


        #endregion
        #endregion

        #region Event handlers

        float on = 0;
        [Timer_10_sec_Elapsed]
        private void timer_Elapsed(DateTime now)
        {
            //mySensors.SetSensorValue(heaterRelay, SensorValueType.Switch, on);
            on = on == 0 ? 1 : 0;

            //var s = mySensors.GetSensor(3, 0);
            //mySensors.GetSensorValue(s, SensorValueType.Temperature);


            //mySensors.RequestSensorValue(mySensors.GetSensor(2, 0), SensorValueType.Temperature);

            //mySensors.RequestSensorValue(mySensors.GetSensor(2, 1), SensorValueType.Ph);


            //mySensors.RequestSensorValue(mySensors.GetSensor(1, 0), SensorValueType.Switch);
            //mySensors.RequestSensorValue(mySensors.GetSensor(1, 1), SensorValueType.Switch);
            //mySensors.RequestSensorValue(mySensors.GetSensor(1, 2), SensorValueType.Switch);
            //mySensors.RequestSensorValue(mySensors.GetSensor(1, 3), SensorValueType.Switch);
            //mySensors.RequestSensorValue(mySensors.GetSensor(1, 4), SensorValueType.Switch);

        }
        #endregion
    }
}
