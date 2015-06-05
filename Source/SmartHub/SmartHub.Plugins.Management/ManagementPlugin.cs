using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using SmartHub.Core.Plugins;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Attributes;
using SmartHub.Plugins.Management.Core;
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
    [AppSection("Менеджмент", SectionType.Common, "/webapp/management/dashboard.js", "SmartHub.Plugins.Management.Resources.js.dashboard.js", TileTypeFullName = "SmartHub.Plugins.Management.ManagementTile")]
    [JavaScriptResource("/webapp/management/dashboard-view.js", "SmartHub.Plugins.Management.Resources.js.dashboard-view.js")]
    [JavaScriptResource("/webapp/management/dashboard-model.js", "SmartHub.Plugins.Management.Resources.js.dashboard-model.js")]
    [HttpResource("/webapp/management/dashboard.html", "SmartHub.Plugins.Management.Resources.js.dashboard.html")]

    [AppSection("Менеджмент", SectionType.System, "/webapp/management/settings.js", "SmartHub.Plugins.Management.Resources.js.settings.js")]
    [JavaScriptResource("/webapp/management/settings-view.js", "SmartHub.Plugins.Management.Resources.js.settings-view.js")]
    [JavaScriptResource("/webapp/management/settings-model.js", "SmartHub.Plugins.Management.Resources.js.settings-model.js")]
    [HttpResource("/webapp/management/settings.html", "SmartHub.Plugins.Management.Resources.js.settings.html")]

    // controller editor
    [JavaScriptResource("/webapp/management/controller-editor.js", "SmartHub.Plugins.Management.Resources.js.controller-editor.js")]
    [JavaScriptResource("/webapp/management/controller-editor-view.js", "SmartHub.Plugins.Management.Resources.js.controller-editor-view.js")]
    [JavaScriptResource("/webapp/management/controller-editor-model.js", "SmartHub.Plugins.Management.Resources.js.controller-editor-model.js")]
    [HttpResource("/webapp/management/controller-editor.html", "SmartHub.Plugins.Management.Resources.js.controller-editor.html")]

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
        private List<ControllerBase> controllers = new List<ControllerBase>();
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
            mapper.Class<Zone>(cfg => cfg.Table("Management_Zones"));
            //mapper.Class<Monitor>(cfg => cfg.Table("Management_Monitors"));
            mapper.Class<Controller>(cfg => cfg.Table("Management_Controllers"));
        }
        public override void InitPlugin()
        {
            mySensors = Context.GetPlugin<MySensorsPlugin>();

            var ctrls = GetControllers();
            foreach (var ctrl in ctrls)
            {
                ControllerBase controller = ConvertController(ctrl);
                if (controller != null)
                    controllers.Add(controller);
            }

            foreach (ControllerBase controller in controllers)
                controller.Init(Context);
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
        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
        private static ControllerBase ConvertController(Controller controller)
        {
            switch (controller.Type)
            {
                case ControllerType.Heater: return new HeaterController(controller);
                case ControllerType.Switch: return new SwitchController(controller);

                default: return null;
            }
        }

        private List<Controller> GetControllers()
        {
            using (var session = Context.OpenSession())
                return session.Query<Controller>()
                    .OrderBy(controller => controller.Name)
                    .ToList();
        }
        private List<Zone> GetZones()
        {
            using (var session = Context.OpenSession())
                return session.Query<Zone>()
                    .OrderBy(zone => zone.Name)
                    .ToList();
        }

        private object BuildControllerWebModel(Controller controller)
        {
            if (controller == null)
                return null;

            return new
            {
                Id = controller.Id,
                Name = controller.Name,
                Type = (int)controller.Type,
                TypeName = GetEnumDescription(controller.Type),
                Configuration = controller.Configuration
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
            foreach (ControllerBase controller in controllers)
                controller.RequestSensorsValues();
        }

        [MySensorsMessageCalibration]
        private void MessageCalibration(SensorMessage message)
        {
            foreach (ControllerBase controller in controllers)
                controller.MessageCalibration(message);
        }

        [MySensorsMessage]
        private void MessageReceived(SensorMessage message)
        {
            foreach (ControllerBase controller in controllers)
            {
                controller.MessageReceived(message);

                if (controller.IsMyMessage(message))
                    NotifyForSignalR(new { MsgId = "ManagementTileContent", Data = BuildTileContent() });
            }
        }

        //[RunPeriodically(1)]
        [Timer_10_sec_Elapsed]
        private void timer_Elapsed(DateTime now)
        {
            foreach (ControllerBase controller in controllers)
                controller.TimerElapsed(now);
        }
        #endregion

        #region Web API
        [HttpCommand("/api/management/monitor/list")]
        private object apiGetMonitors(HttpRequestParams request)
        {
            using (var session = Context.OpenSession())
                return session.Query<Monitor>()
                    .OrderBy(monitor => monitor.Name)
                    .Select(monitor => new
                    {
                        Id = monitor.Id,
                        Name = monitor.Name,
                        Sensor = mySensors.BuildSensorWebModel(mySensors.GetSensor(monitor.SensorId))
                    })
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

        [HttpCommand("/api/management/controllertype/list")]
        private object apiGetControllerTypes(HttpRequestParams request)
        {
            return Enum.GetValues(typeof(ControllerType))
                .Cast<ControllerType>()
                .Select(v => new
                {
                    Id = v,
                    Name = GetEnumDescription(v)
                }).ToArray();
        }
        [HttpCommand("/api/management/controller/list")]
        private object apiGetControllers(HttpRequestParams request)
        {
            return GetControllers()
                .Select(BuildControllerWebModel)
                .Where(x => x != null)
                .ToArray();
        }
        [HttpCommand("/api/management/controller")]
        private object apiGetController(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");

            using (var session = Context.OpenSession())
                return BuildControllerWebModel(session.Get<Controller>(id));
        }
        [HttpCommand("/api/management/controller/add")]
        private object apiAddController(HttpRequestParams request)
        {
            var name = request.GetRequiredString("name");
            var type = (ControllerType)request.GetRequiredInt32("type");

            var ctrl = new Controller()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Type = type
            };

            ControllerBase controller = ConvertController(ctrl);
            if (controller != null)
            {
                controller.Init(Context);
                controller.SaveToDB();
                controllers.Add(controller);

                NotifyForSignalR(new { MsgId = "ControllerAdded", Data = BuildControllerWebModel(ctrl) });
            }

            return null;
        }
        [HttpCommand("/api/management/controller/setname")]
        private object apiSetControllerName(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            var name = request.GetRequiredString("name");

            using (var session = Context.OpenSession())
            {
                var ctrl = session.Load<Controller>(id);
                ctrl.Name = name;
                session.Flush();

                NotifyForSignalR(new { MsgId = "ControllerNameChanged", Data = BuildControllerWebModel(ctrl) });
            }

            return null;
        }
        [HttpCommand("/api/management/controller/setconfiguration")]
        private object apiSetControllerConfiguration(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            var conf = request.GetRequiredString("config");

            foreach (ControllerBase controller in controllers)
                if (controller.ControllerID == id)
                {
                    controller.SetConfiguration(conf);
                    break;
                }

            //NotifyForSignalR(new { MsgId = "ControllerIsVisibleChanged", Data = BuildControllerWebModel(ctrl) });

            return null;
        }
        [HttpCommand("/api/management/controller/delete")]
        private object apiDeleteController(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("Id");

            using (var session = Context.OpenSession())
            {
                var ctrl = session.Load<Controller>(id);
                session.Delete(ctrl);
                session.Flush();

                NotifyForSignalR(new { MsgId = "ControllerDeleted", Data = new { Id = id } });
            }

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


        [HttpCommand("/api/management/monitor/list/dashboard")]
        private object apiGetMonitorsForDashboard(HttpRequestParams request)
        {
            using (var session = Context.OpenSession())
                return session.Query<Monitor>()
                    .OrderBy(monitor => monitor.Name)
                    .Select(monitor => new
                    {
                        Id = monitor.Id,
                        Name = monitor.Name,
                        Sensor = mySensors.BuildSensorWebModel(mySensors.GetSensor(monitor.SensorId)),
                        SensorValues = mySensors.GetSensorValuesByID(monitor.SensorId, 24, 30).ToArray()
                    })
                    .ToArray();
        }
        [HttpCommand("/api/management/controller/list/dashboard")]
        private object apiGetControllersForDashboard(HttpRequestParams request)
        {
            using (var session = Context.OpenSession())
                return session.Query<Controller>()
                    .OrderBy(controller => controller.Name)
                    .Select(monitor => new
                    {
                        Id = monitor.Id,
                        Name = monitor.Name,
                        //Sensor = mySensors.BuildSensorWebModel(mySensors.GetSensor(monitor.SensorId)),
                        //SensorValues = mySensors.GetSensorValuesByID(monitor.SensorId, 48).ToArray()
                    })
                    .ToArray();
        }
        #endregion
    }
}
