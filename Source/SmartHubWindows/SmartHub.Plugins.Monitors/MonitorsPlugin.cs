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

    [CssResource("/webapp/monitors/css/weather-icons.min.css", "SmartHub.Plugins.Monitors.Resources.css.weather-icons.min.css", AutoLoad = true)]
    [HttpResource("/webapp/monitors/fonts/weathericons-regular-webfont.eot", "SmartHub.Plugins.Monitors.Resources.fonts.weathericons-regular-webfont.eot", "application/vnd.ms-fontobject")]
    [HttpResource("/webapp/monitors/fonts/weathericons-regular-webfont.svg", "SmartHub.Plugins.Monitors.Resources.fonts.weathericons-regular-webfont.svg", "image/svg+xml")]
    [HttpResource("/webapp/monitors/fonts/weathericons-regular-webfont.ttf", "SmartHub.Plugins.Monitors.Resources.fonts.weathericons-regular-webfont.ttf", "application/x-font-truetype")]
    [HttpResource("/webapp/monitors/fonts/weathericons-regular-webfont.woff", "SmartHub.Plugins.Monitors.Resources.fonts.weathericons-regular-webfont.woff", "application/font-woff")]

    // monitor editor
    [JavaScriptResource("/webapp/monitors/monitor-editor.js", "SmartHub.Plugins.Monitors.Resources.js.monitor-editor.js")]
    [JavaScriptResource("/webapp/monitors/monitor-editor-view.js", "SmartHub.Plugins.Monitors.Resources.js.monitor-editor-view.js")]
    [JavaScriptResource("/webapp/monitors/monitor-editor-model.js", "SmartHub.Plugins.Monitors.Resources.js.monitor-editor-model.js")]
    [HttpResource("/webapp/monitors/monitor-editor.html", "SmartHub.Plugins.Monitors.Resources.js.monitor-editor.html")]

    [JavaScriptResource("/webapp/monitors/jquery.jsoneditor.min.js", "SmartHub.Plugins.Monitors.Resources.js.jquery.jsoneditor.min.js")]
    [CssResource("/webapp/monitors/jsoneditor.css", "SmartHub.Plugins.Monitors.Resources.css.jsoneditor.css", AutoLoad = true)]

    [JavaScriptResource("/webapp/monitors/utils.js", "SmartHub.Plugins.Monitors.Resources.js.utils.js")]
    [HttpResource("/webapp/monitors/utils.html", "SmartHub.Plugins.Monitors.Resources.js.utils.html")]

    [Plugin]
    public class MonitorsPlugin : PluginBase
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

        #region API
        public List<Monitor> Get()
        {
            using (var session = Context.OpenSession())
                return session.Query<Monitor>()
                    .OrderBy(monitor => monitor.Name)
                    .ToList();
        }
        public Monitor Get(Guid id)
        {
            using (var session = Context.OpenSession())
                return session.Get<Monitor>(id);
        }
        public Monitor GetBySensor(Guid id)
        {
            using (var session = Context.OpenSession())
                return session.Query<Monitor>().Where(m => m.SensorId == id).FirstOrDefault();
        }
        #endregion

        #region Web API
        public object BuildMonitorWebModel(Monitor monitor)
        {
            if (monitor == null)
                return null;

            return new
            {
                Id = monitor.Id,
                Name = monitor.Name,
                NameForInformer = monitor.NameForInformer,
                SensorName = mySensors.GetSensor(monitor.SensorId).Name
            };
        }
        public object BuildMonitorRichWebModel(Monitor monitor)
        {
            if (monitor == null)
                return null;

            var sensor = mySensors.GetSensor(monitor.SensorId);
            if (sensor == null)
                return null;

            return new
            {
                Id = monitor.Id,
                Name = monitor.Name,
                SensorNodeNo = sensor.NodeNo,
                SensorSensorNo = sensor.SensorNo,
                SensorValues = mySensors.GetSensorValues(sensor, 10).ToArray(),
                Configuration = monitor.Configuration
            };
        }

        [HttpCommand("/api/monitors/list")]
        private object apiGetMonitors(HttpRequestParams request)
        {
            return Get()
                .Select(BuildMonitorWebModel)
                .Where(x => x != null)
                .ToArray();
        }
        [HttpCommand("/api/monitors/get/rich")]
        private object apiGetMonitorRich(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            return BuildMonitorRichWebModel(Get(id));
        }
        [HttpCommand("/api/monitors/getBySensor")]
        private object apiGetMonitorBySensor(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            return BuildMonitorRichWebModel(GetBySensor(id));
        }

        [HttpCommand("/api/monitors/add")]
        private object apiAddMonitor(HttpRequestParams request)
        {
            var name = request.GetRequiredString("name");
            var sensorId = request.GetRequiredGuid("sensorId");
            var configuration = request.GetRequiredString("config");

            using (var session = Context.OpenSession())
            {
                Monitor monitor = new Monitor()
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    SensorId = sensorId,
                    Configuration = configuration
                };

                session.Save(monitor);
                session.Flush();
            }

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
                var monitor = session.Load<Monitor>(id);
                monitor.Name = name;
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
        [HttpCommand("/api/monitors/setnameforinformer")]
        private object apiSetMonitorNameForInformer(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            var name = request.GetString("name");

            using (var session = Context.OpenSession())
            {
                var monitor = session.Load<Monitor>(id);
                monitor.NameForInformer = name;
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
        [HttpCommand("/api/monitors/setconfiguration")]
        private object apiSetMonitorConfiguration(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            var configuration = request.GetRequiredString("config");

            using (var session = Context.OpenSession())
            {
                var monitor = session.Load<Monitor>(id);
                monitor.Configuration = configuration;
                session.Flush();
            }

            //NotifyForSignalR(new { MsgId = "ControllerIsVisibleChanged", Data = BuildControllerWebModel(ctrl) });

            return null;
        }
        [HttpCommand("/api/monitors/delete")]
        private object apiDeleteMonitor(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("Id");

            using (var session = Context.OpenSession())
            {
                var monitor = session.Load<Monitor>(id);
                session.Delete(monitor);
                session.Flush();
            }

            //NotifyForSignalR(new { MsgId = "SensorDeleted", Data = new { Id = id } });

            return null;
        }
        #endregion
    }
}
