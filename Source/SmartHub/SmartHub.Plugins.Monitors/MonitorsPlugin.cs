using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using SmartHub.Core.Plugins;
using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Attributes;
using SmartHub.Plugins.Monitors.Core;
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
            mapper.Class<Monitor>(cfg => cfg.Table("Monitors_Monitors"));
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
                Type = (int)monitor.Type,
                TypeName = monitor.Type.GetEnumDescription(),
                Sensor = mySensors.BuildSensorWebModel(mySensors.GetSensor(monitor.SensorId)),
                Configuration = monitor.Configuration
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
                Type = (int)monitor.Type,
                TypeName = monitor.Type.GetEnumDescription(),
                Sensor = mySensors.BuildSensorWebModel(mySensors.GetSensor(monitor.SensorId)),
                SensorValues = mySensors.GetSensorValuesByID(monitor.SensorId, 24, 30).ToArray(),
                Configuration = monitor.Configuration
            };
        }
        #endregion

        #region Web API
        [HttpCommand("/api/monitors/monitortype/list")]
        private object apiGetMonitorTypes(HttpRequestParams request)
        {
            return Enum.GetValues(typeof(MonitorType))
                .Cast<MonitorType>()
                .Select(v => new
                {
                    Id = v,
                    Name = v.GetEnumDescription(),
                }).ToArray();
        }
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
        [HttpCommand("/api/monitors/get")]
        private object apiGetMonitor(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");

            using (var session = Context.OpenSession())
                return BuildMonitorWebModel(session.Get<Monitor>(id));
        }
        [HttpCommand("/api/monitors/add")]
        private object apiAddMonitor(HttpRequestParams request)
        {
            var name = request.GetRequiredString("name");
            var sensorId = request.GetRequiredGuid("sensorId");
            var type = (MonitorType)request.GetRequiredInt32("type");

            using (var session = Context.OpenSession())
            {
                Monitor monitor = new Monitor()
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    SensorId = sensorId,
                    Type = type,
                    Configuration = "{}" // ?????????????
                };

                session.Save(monitor);
                session.Flush();
            }

            //ControllerBase controller = ConvertController(ctrl);
            //if (controller != null)
            //{
            //    controller.Init(Context);
            //    controller.SaveToDB();
            //    controllers.Add(controller);

            //    NotifyForSignalR(new { MsgId = "ControllerAdded", Data = BuildControllerWebModel(ctrl) });
            //}

            //NotifyForSignalR(new { MsgId = "MonitorAdded", Data = BuildMonitorWebModel(ctrl) });

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
