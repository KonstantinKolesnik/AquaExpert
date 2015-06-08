using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using SmartHub.Core.Plugins;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Attributes;
using SmartHub.Plugins.Management.Data;
using SmartHub.Plugins.MySensors;
using SmartHub.Plugins.MySensors.Attributes;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.SignalR;
using SmartHub.Plugins.Timer.Attributes;
using SmartHub.Plugins.WebUI.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SmartHub.Plugins.Management
{
    //[AppSection("Менеджмент", SectionType.Common, "/webapp/management/dashboard.js", "SmartHub.Plugins.Management.Resources.js.dashboard.js", TileTypeFullName = "SmartHub.Plugins.Management.ManagementTile")]
    [JavaScriptResource("/webapp/management/dashboard-view.js", "SmartHub.Plugins.Management.Resources.js.dashboard-view.js")]
    [JavaScriptResource("/webapp/management/dashboard-model.js", "SmartHub.Plugins.Management.Resources.js.dashboard-model.js")]
    [HttpResource("/webapp/management/dashboard.html", "SmartHub.Plugins.Management.Resources.js.dashboard.html")]

    //[AppSection("Менеджмент", SectionType.System, "/webapp/management/settings.js", "SmartHub.Plugins.Management.Resources.js.settings.js")]
    [AppSection("Менеджмент", SectionType.System, "/webapp/management/settings.js", "SmartHub.Plugins.Management.Resources.js.settings.js", TileTypeFullName = "SmartHub.Plugins.Management.ManagementTile")]
    [JavaScriptResource("/webapp/management/settings-view.js", "SmartHub.Plugins.Management.Resources.js.settings-view.js")]
    [JavaScriptResource("/webapp/management/settings-model.js", "SmartHub.Plugins.Management.Resources.js.settings-model.js")]
    [HttpResource("/webapp/management/settings.html", "SmartHub.Plugins.Management.Resources.js.settings.html")]

    // zone editor
    [JavaScriptResource("/webapp/management/zone-editor.js", "SmartHub.Plugins.Management.Resources.js.zone-editor.js")]
    [JavaScriptResource("/webapp/management/zone-editor-view.js", "SmartHub.Plugins.Management.Resources.js.zone-editor-view.js")]
    [JavaScriptResource("/webapp/management/zone-editor-model.js", "SmartHub.Plugins.Management.Resources.js.zone-editor-model.js")]
    [HttpResource("/webapp/management/zone-editor.html", "SmartHub.Plugins.Management.Resources.js.zone-editor.html")]

    [CssResource("/webapp/management/css/style.css", "SmartHub.Plugins.Management.Resources.css.style.css", AutoLoad = true)]

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
            //mapper.Class<Zone>(cfg => cfg.Table("Management_Zones"));
            //mapper.Class<Monitor>(cfg => cfg.Table("Management_Monitors"));
        }
        public override void InitPlugin()
        {
            mySensors = Context.GetPlugin<MySensorsPlugin>();
        }
        #endregion

        #region Public methods
        public string BuildTileContent()
        {
            //SensorValue lastSVHeaterTemperature = mySensors.GetLastSensorValue(heaterController.SensorTemperature);
            //SensorValue lastSVHeaterSwitch = mySensors.GetLastSensorValue(heaterController.SensorSwitch);
            //SensorValue lastSVTemperatureOuter = mySensors.GetLastSensorValue(SensorTemperatureOuter);
            //SensorValue lastSVHumidityOuter = mySensors.GetLastSensorValue(SensorHumidityOuter);
            //SensorValue lastSVAtmospherePressure = mySensors.GetLastSensorValue(SensorAtmospherePressure);
            //SensorValue lastSVForecast = mySensors.GetLastSensorValue(SensorForecast);

            StringBuilder sb = new StringBuilder();
            //sb.Append("<div>Температура воды: " + (lastSVHeaterTemperature != null ? lastSVHeaterTemperature.Value + " °C" : "&lt;нет данных&gt;") + "</div>");
            //sb.Append("<div>Обогреватель: " + (lastSVHeaterSwitch != null ? (lastSVHeaterSwitch.Value == 1 ? "Вкл." : "Выкл.") : "&lt;нет данных&gt;") + "</div>");
            //sb.Append("<div>Температура снаружи: " + (lastSVTemperatureOuter != null ? lastSVTemperatureOuter.Value + " °C" : "&lt;нет данных&gt;") + "</div>");
            //sb.Append("<div>Влажность снаружи: " + (lastSVHumidityOuter != null ? lastSVHumidityOuter.Value + " %" : "&lt;нет данных&gt;") + "</div>");
            //sb.Append("<div>Давление: " + (lastSVAtmospherePressure != null ? (int)(lastSVAtmospherePressure.Value / 133.3f) + " mmHg" : "&lt;нет данных&gt;") + "</div>");

            //string[] weather = { "Ясно", "Солнечно", "Облачно", "К дождю", "Дождь", "-" };
            //sb.Append("<div>Прогноз: " + (lastSVForecast != null ? weather[(int)lastSVForecast.Value] : "&lt;нет данных&gt;") + "</div>");
            return sb.ToString();
        }
        public string BuildSignalRReceiveHandler()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("if (data.MsgId == 'AquaControllerTileContent') { ");
            sb.Append("model.tileModel.set({ 'content': data.Data }); ");
            sb.Append("}");
            return sb.ToString();
        }
        #endregion

        #region Private methods
        private List<Monitor> GetMonitors()
        {
            using (var session = Context.OpenSession())
                return session.Query<Monitor>()
                    .OrderBy(monitor => monitor.Name)
                    .ToList();
        }
        private List<Zone> GetZones()
        {
            using (var session = Context.OpenSession())
                return session.Query<Zone>()
                    .OrderBy(zone => zone.Name)
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
        private object BuildZoneWebModel(Zone zone)
        {
            if (zone == null)
                return null;

            return new
            {
                Id = zone.Id,
                Name = zone.Name,
                MonitorsList = zone.MonitorsList ?? "[]",
                ControllersList = zone.ControllersList ?? "[]"
            };
        }
        #endregion

        #region Event handlers
        [MySensorsConnected]
        private void Connected()
        {
        }

        [MySensorsMessageCalibration]
        private void MessageCalibration(SensorMessage message)
        {
        }

        [MySensorsMessage]
        private void MessageReceived(SensorMessage message)
        {
        }

        //[RunPeriodically(1)]
        [Timer_10_sec_Elapsed]
        private void timer_Elapsed(DateTime now)
        {
        }
        #endregion

        #region Web API
        [HttpCommand("/api/management/monitor/list")]
        private object apiGetMonitors(HttpRequestParams request)
        {
            return GetMonitors()
                .Select(BuildMonitorWebModel)
                .Where(x => x != null)
                .ToArray();
        }
        [HttpCommand("/api/management/monitor/list/dashboard")]
        private object apiGetMonitorsForDashboard(HttpRequestParams request)
        {
            return GetMonitors()
                .Select(BuildMonitorRichWebModel)
                .Where(x => x != null)
                .ToArray();
        }
        [HttpCommand("/api/management/monitor/add")]
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
        [HttpCommand("/api/management/monitor/setname")]
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
        [HttpCommand("/api/management/monitor/delete")]
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

        [HttpCommand("/api/management/zone/list")]
        private object apiGetZones(HttpRequestParams request)
        {
            return GetZones()
                .Select(BuildZoneWebModel)
                .Where(x => x != null)
                .ToArray();
        }
        [HttpCommand("/api/management/zone")]
        private object apiGetZone(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");

            using (var session = Context.OpenSession())
                return BuildZoneWebModel(session.Get<Zone>(id));
        }
        [HttpCommand("/api/management/zone/add")]
        private object apiAddZone(HttpRequestParams request)
        {
            var name = request.GetRequiredString("name");

            using (var session = Context.OpenSession())
            {
                Zone zone = new Zone()
                {
                    Id = Guid.NewGuid(),
                    Name = name
                };

                session.Save(zone);
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
        [HttpCommand("/api/management/zone/setname")]
        private object apiSetZoneName(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            var name = request.GetRequiredString("name");

            using (var session = Context.OpenSession())
            {
                var zone = session.Load<Zone>(id);
                zone.Name = name;
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
        [HttpCommand("/api/management/zone/setconfiguration")]
        private object apiSetZoneConfiguration(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            var monitorsList = request.GetRequiredString("monitorsList");
            var controllersList = request.GetRequiredString("controllersList");

            using (var session = Context.OpenSession())
            {
                var zone = session.Load<Zone>(id);
                zone.MonitorsList = monitorsList;
                zone.ControllersList = controllersList;
                session.Flush();
            }

            //NotifyForSignalR(new { MsgId = "ControllerIsVisibleChanged", Data = BuildControllerWebModel(ctrl) });

            return null;
        }
        [HttpCommand("/api/management/zone/delete")]
        private object apiDeleteZone(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("Id");

            using (var session = Context.OpenSession())
            {
                var zone = session.Load<Zone>(id);
                session.Delete(zone);
                session.Flush();
            }

            //NotifyForSignalR(new { MsgId = "SensorDeleted", Data = new { Id = id } });

            return null;
        }
        #endregion
    }
}
