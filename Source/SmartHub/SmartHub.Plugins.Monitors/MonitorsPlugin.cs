using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using SmartHub.Core.Plugins;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Attributes;
using SmartHub.Plugins.Monitors.Data;
using SmartHub.Plugins.MySensors;
using SmartHub.Plugins.SignalR;
using SmartHub.Plugins.WebUI.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartHub.Plugins.Monitors
{
    [AppSection("Мониторы", SectionType.System, "/webapp/monitors/settings.js", "SmartHub.Plugins.Monitors.Resources.js.settings.js", TileTypeFullName = "SmartHub.Plugins.Monitors.MonitorsTile")]
    [JavaScriptResource("/webapp/monitors/settings-view.js", "SmartHub.Plugins.Monitors.Resources.js.settings-view.js")]
    [JavaScriptResource("/webapp/monitors/settings-model.js", "SmartHub.Plugins.Monitors.Resources.js.settings-model.js")]
    [HttpResource("/webapp/monitors/settings.html", "SmartHub.Plugins.Monitors.Resources.js.settings.html")]

    [Plugin]
    public class ManagementPlugin : PluginBase
    {
        #region Fields
        private MySensorsPlugin mySensors;
        #endregion

        #region SignalR events
        private void NotifyForSignalR(object msg)
        {
            Context.GetPlugin<SignalRPlugin>().Broadcast(msg);
        }
        #endregion

        #region Plugin overrides
        public override void InitDbModel(ModelMapper mapper)
        {
            //mapper.Class<Monitor>(cfg => cfg.Table("Monitors_Monitors"));
        }
        public override void InitPlugin()
        {
            mySensors = Context.GetPlugin<MySensorsPlugin>();
        }
        #endregion

        #region Public methods

        #endregion

        #region Private methods
        private List<Monitor> GetMonitors()
        {
            using (var session = Context.OpenSession())
                return session.Query<Monitor>()
                    .OrderBy(monitor => monitor.Name)
                    .ToList();
        }

        private object BuildMonitorWebModel(Monitor monitor)
        {
            if (monitor == null)
                return null;

            return new
            {
                Id = monitor.Id,
                Name = monitor.Name,
                Sensor = mySensors.BuildSensorWebModel(mySensors.GetSensor(monitor.SensorId))
            };
        }
        private object BuildMonitorRichWebModel(Monitor monitor)
        {
            if (monitor == null)
                return null;

            return new
            {
                Id = monitor.Id,
                Name = monitor.Name,
                Sensor = mySensors.BuildSensorWebModel(mySensors.GetSensor(monitor.SensorId)),
                SensorValues = mySensors.GetSensorValuesByID(monitor.SensorId, 24, 30).ToArray()
            };
        }
        #endregion

        #region Web API
        [HttpCommand("/api/monitors/list")]
        private object apiGetMonitors(HttpRequestParams request)
        {
            return GetMonitors()
                .Select(BuildMonitorWebModel)
                .Where(x => x != null)
                .ToArray();
        }
        [HttpCommand("/api/monitors/list/dashboard")]
        private object apiGetMonitorsForDashboard(HttpRequestParams request)
        {
            return GetMonitors()
                .Select(BuildMonitorRichWebModel)
                .Where(x => x != null)
                .ToArray();
        }
        [HttpCommand("/api/monitors/add")]
        private object apiAddMonitor(HttpRequestParams request)
        {
            var name = request.GetRequiredString("name");
            var sensorId = request.GetRequiredGuid("sensorId");

            using (var session = Context.OpenSession())
            {
                Monitor monitor = new Monitor()
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    SensorId = sensorId
                };

                session.Save(monitor);
                session.Flush();
            }

            //NotifyForSignalR(new
            //{
            //    MsgId = "SensorNameChanged",
            //    Data = new
            //    {
            //        Id = id,
            //        Name = name
            //    }
            //});

            return null;
        }
        [HttpCommand("/api/monitors/setname")]
        private object apiSetMonitorName(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            var name = request.GetRequiredString("name");

            using (var session = Context.OpenSession())
            {
                var sensor = session.Load<Monitor>(id);
                sensor.Name = name;
                session.Flush();
            }

            //NotifyForSignalR(new
            //{
            //    MsgId = "SensorNameChanged",
            //    Data = new
            //    {
            //        Id = id,
            //        Name = name
            //    }
            //});

            return null;
        }
        [HttpCommand("/api/monitors/delete")]
        private object apiDeleteMonitor(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("Id");

            using (var session = Context.OpenSession())
            {
                var sensor = session.Load<Monitor>(id);
                session.Delete(sensor);
                session.Flush();
            }

            //NotifyForSignalR(new { MsgId = "SensorDeleted", Data = new { Id = id } });

            return null;
        }
        #endregion
    }
}
