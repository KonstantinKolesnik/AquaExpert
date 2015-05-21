using SmartHub.Core.Plugins;
using SmartHub.Plugins.MySensors;
using SmartHub.Plugins.MySensors.Attributes;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.MySensors.Data;
using SmartHub.Plugins.Timer.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.Plugins.AquaController.Core
{
    public class TemperatureController
    {
        private MySensorsPlugin mySensors;
        private Sensor relay;
        private Sensor sensor;
        private float minTemperature = 25.0f;
        private float maxTemperature = 26.0f;

        [Import(typeof(IServiceContext))]
        private IServiceContext context;



        public void Init(MySensorsPlugin mySensors)
        {
            this.mySensors = mySensors;

            relay = mySensors.GetSensor(1, 0);
            sensor = mySensors.GetSensor(2, 0);
        }

        //private SmartHub.Plugins.AquaController.Data.Setting GetSetting(string name)
        //{
        //    using (var session = mySensors.Context.OpenSession())
        //        return session.Query<SmartHub.Plugins.AquaController.Data.Setting>().FirstOrDefault(setting => setting.Name == name);
        //}


        [MySensorsConnected]
        private void Connected()
        {
            if (sensor != null)
                mySensors.RequestSensorValue(sensor, SensorValueType.Temperature);
            if (relay != null)
                mySensors.RequestSensorValue(relay, SensorValueType.Switch);
        }

        [MySensorsMessage]
        private void MessageReceived(SensorMessage msg)
        {
            if (relay != null && sensor != null && msg.NodeID == sensor.NodeNo && msg.SensorID == sensor.SensorNo)
                mySensors.SetSensorValue(relay, SensorValueType.Switch, msg.PayloadFloat < minTemperature ? 1 : 0);
        }

        [Timer_5_sec_Elapsed]
        private void timer_Elapsed(DateTime now)
        {
            int a = 0;
            int b = a;
        }

    }
}
