using NHibernate.Linq;
using SmartHub.Core.Plugins;
using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.AquaController.Data;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Attributes;
using SmartHub.Plugins.MySensors;
using SmartHub.Plugins.MySensors.Attributes;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.MySensors.Data;
using SmartHub.Plugins.SignalR;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace SmartHub.Plugins.AquaController.Core
{
    public abstract class ControllerBase
    {
        #region Fields
        protected MySensorsPlugin mySensors;
        protected IServiceContext Context;
        #endregion

        #region SignalR events
        private void NotifyForSignalR(object msg)
        {
            Context.GetPlugin<SignalRPlugin>().Broadcast(msg);
        }
        #endregion

        #region Public methods
        public virtual void Init(IServiceContext context)
        {
            Context = context;

            mySensors = Context.GetPlugin<MySensorsPlugin>();
        }
        #endregion

        #region Private methods
        protected AquaControllerSetting GetSetting(string name)
        {
            using (var session = Context.OpenSession())
                return session.Query<AquaControllerSetting>().FirstOrDefault(setting => setting.Name == name);
        }
        protected void SaveOrUpdate(object item)
        {
            using (var session = Context.OpenSession())
            {
                try
                {
                    session.SaveOrUpdate(item);
                    session.Flush();
                }
                catch (Exception) { }
            }
        }

        abstract protected void RequestSensorsValues();

        protected SensorValue GetLastSensorValue(Sensor sensor)
        {
            if (sensor == null)
                return null;

            using (var session = Context.OpenSession())
                return session.Query<SensorValue>()
                    .Where(sv => sv.NodeNo == sensor.NodeNo && sv.SensorNo == sensor.SensorNo)
                    .OrderByDescending(sv => sv.TimeStamp)
                    .FirstOrDefault();
        }
        protected object BuildSensorSummaryWebModel(Sensor sensor)
        {
            if (sensor == null)
                return null;

            return new
            {
                Id = sensor.Id,
                Name = sensor.Name
            };
        }
        protected bool IsMessageFromSensor(SensorMessage msg, Sensor sensor)
        {
            return (sensor == null || msg == null) ? false : (sensor.NodeNo == msg.NodeNo && sensor.SensorNo == msg.SensorNo);
        }
        #endregion

        #region Event handlers
        [MySensorsConnected]
        private void Connected()
        {
            RequestSensorsValues();
        }

        [MySensorsMessage]
        abstract protected void MessageReceived(SensorMessage message);
        #endregion

        #region Web API
        [HttpCommand("/api/aquacontroller/sensorsDataSource")]
        public object GetSensorsDataSource(HttpRequestParams request)
        {
            var type = request.GetInt32("type");

            using (var session = Context.OpenSession())
                return session.Query<Sensor>()
                    .Where(s => type.HasValue ? (int)s.Type == type.Value : true)
                    .Select(BuildSensorSummaryWebModel)
                    .Where(x => x != null)
                    .ToArray();
        }
        [HttpCommand("/api/aquacontroller/sensor")]
        public object GetSensor(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            return mySensors.BuildSensorWebModel(mySensors.GetSensor(id));
        }
        [HttpCommand("/api/aquacontroller/sensorvalues")]
        public object GetSensorValues(HttpRequestParams request)
        {
            var nodeNo = request.GetRequiredInt32("nodeNo");
            var sensorNo = request.GetRequiredInt32("sensorNo");
            var hours = request.GetRequiredInt32("hours");

            DateTime dt = DateTime.UtcNow.AddHours(-hours);

            using (var session = Context.OpenSession())
                return session.Query<SensorValue>().Where(sv => sv.NodeNo == nodeNo && sv.SensorNo == sensorNo && sv.TimeStamp >= dt).ToArray();
        }


        //[HttpCommand("/api/aquacontroller/configuration")]
        //public object GetSensorsConfiguration(HttpRequestParams request)
        //{
        //    return configuration;
        //}
        //[HttpCommand("/api/aquacontroller/configuration/set")]
        //public object SetSensorsConfiguration(HttpRequestParams request)
        //{
        //    var json = request.GetRequiredString("sc");
        //    configuration = (Configuration)Extensions.FromJson(typeof(Configuration), json);
        //    configurationSetting.SetValue(configuration);
        //    SaveOrUpdate(configurationSetting);

        //    RequestSensorsValues();

        //    return null;
        //}
        #endregion
    }
}
