using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using SmartHub.Core.Plugins;
using SmartHub.Plugins.AquaController.Core;
using SmartHub.Plugins.AquaController.Data;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Attributes;
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

namespace SmartHub.Plugins.AquaController
{
    [AppSection("Аква-контроллер", SectionType.Common, "/webapp/aquacontroller/dashboard.js", "SmartHub.Plugins.AquaController.Resources.js.dashboard.js", TileTypeFullName = "SmartHub.Plugins.AquaController.AquaControllerTile")]
    [JavaScriptResource("/webapp/aquacontroller/dashboard-view.js", "SmartHub.Plugins.AquaController.Resources.js.dashboard-view.js")]
    [JavaScriptResource("/webapp/aquacontroller/dashboard-model.js", "SmartHub.Plugins.AquaController.Resources.js.dashboard-model.js")]
    [HttpResource("/webapp/aquacontroller/dashboard.html", "SmartHub.Plugins.AquaController.Resources.js.dashboard.html")]

    [AppSection("Аква-контроллер", SectionType.System, "/webapp/aquacontroller/settings.js", "SmartHub.Plugins.AquaController.Resources.js.settings.js")]
    [JavaScriptResource("/webapp/aquacontroller/settings-view.js", "SmartHub.Plugins.AquaController.Resources.js.settings-view.js")]
    [JavaScriptResource("/webapp/aquacontroller/settings-model.js", "SmartHub.Plugins.AquaController.Resources.js.settings-model.js")]
    [HttpResource("/webapp/aquacontroller/settings.html", "SmartHub.Plugins.AquaController.Resources.js.settings.html")]

    // controller editor
    [JavaScriptResource("/webapp/aquacontroller/editor.js", "SmartHub.Plugins.AquaController.Resources.js.editor.js")]
    [JavaScriptResource("/webapp/aquacontroller/editor-view.js", "SmartHub.Plugins.AquaController.Resources.js.editor-view.js")]
    [JavaScriptResource("/webapp/aquacontroller/editor-model.js", "SmartHub.Plugins.AquaController.Resources.js.editor-model.js")]
    [HttpResource("/webapp/aquacontroller/editor.html", "SmartHub.Plugins.AquaController.Resources.js.editor.html")]

    [CssResource("/webapp/aquacontroller/css/style.css", "SmartHub.Plugins.AquaController.Resources.css.style.css", AutoLoad = true)]

    [Plugin]
    public class AquaControllerPlugin : PluginBase
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
            mapper.Class<AquaControllerSetting>(cfg => cfg.Table("AquaController_Settings"));
            mapper.Class<Monitor>(cfg => cfg.Table("AquaController_Monitors"));
            mapper.Class<Controller>(cfg => cfg.Table("AquaController_Controllers"));
        }
        public override void InitPlugin()
        {
            mySensors = Context.GetPlugin<MySensorsPlugin>();

            var ctrls = GetAllControllers();
            foreach (var ctrl in ctrls)
            {
                ControllerBase controller = CreateController(ctrl);
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
        private static ControllerBase CreateController(Controller controller)
        {
            switch (controller.Type)
            {
                case ControllerType.Heater: return new HeaterController(controller);

                default: return null;
            }
        }
        private List<Controller> GetAllControllers()
        {
            using (var session = Context.OpenSession())
                return session.Query<Controller>()
                    .OrderBy(controller => controller.Name)
                    .ToList();
        }
        private Controller GetController(Guid id)
        {
            using (var session = Context.OpenSession())
                return session.Get<Controller>(id);
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
                IsVisible = controller.IsVisible,
                Configuration = controller.Configuration
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
                    NotifyForSignalR(new { MsgId = "AquaControllerTileContent", Data = BuildTileContent() });
            }
        }

        //[RunPeriodically(1)]
        [Timer_5_sec_Elapsed]
        private void timer_Elapsed(DateTime now)
        {
            foreach (ControllerBase controller in controllers)
                controller.TimerElapsed(now);
        }
        #endregion

        #region Web API
        [HttpCommand("/api/aquacontroller/monitor/listall")]
        private object apiGetAllMonitors(HttpRequestParams request)
        {
            using (var session = Context.OpenSession())
                return session.Query<Monitor>()
                    .OrderBy(monitor => monitor.Name)
                    .Select(monitor => new
                    {
                        Id = monitor.Id,
                        Name = monitor.Name,
                        Sensor = mySensors.BuildSensorWebModel(mySensors.GetSensor(monitor.SensorId)),
                        IsVisible = monitor.IsVisible
                    })
                    .ToArray();
        }
        [HttpCommand("/api/aquacontroller/monitor/listvisible")]
        private object apiGetVisibleMonitors(HttpRequestParams request)
        {
            using (var session = Context.OpenSession())
                return session.Query<Monitor>()
                    .Where(monitor => monitor.IsVisible)
                    .OrderBy(monitor => monitor.Name)
                    .Select(monitor => new {
                        Id = monitor.Id,
                        Name = monitor.Name,
                        Sensor = mySensors.BuildSensorWebModel(mySensors.GetSensor(monitor.SensorId)),
                        SensorValues = mySensors.GetSensorValuesByID(monitor.SensorId, 24).ToArray()
                    })
                    .ToArray();
        }
        [HttpCommand("/api/aquacontroller/monitor/add")]
        private object apiAddMonitor(HttpRequestParams request)
        {
            var name = request.GetRequiredString("name");
            var sensorId = request.GetRequiredGuid("sensorId");
            var isVisible = request.GetRequiredBool("isVisible");

            using (var session = Context.OpenSession())
            {
                Monitor monitor = new Monitor()
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    SensorId = sensorId,
                    IsVisible = isVisible
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
        [HttpCommand("/api/aquacontroller/monitor/setname")]
        private object apiSetMonitorName(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            var name = request.GetString("name");

            using (var session = Context.OpenSession())
            {
                var sensor = session.Get<Monitor>(id);
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
        [HttpCommand("/api/aquacontroller/monitor/setisvisible")]
        private object apiSetMonitorIsVisible(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            var isVisible = request.GetRequiredBool("isVisible");

            using (var session = Context.OpenSession())
            {
                var sensor = session.Get<Monitor>(id);
                sensor.IsVisible = isVisible;
                session.Flush();
            }

            //NotifyForSignalR(new
            //{
            //    MsgId = "SensorIsVisibleChanged",
            //    Data = new
            //    {
            //        Id = id,
            //        Name = name
            //    }
            //});

            return null;
        }
        [HttpCommand("/api/aquacontroller/monitor/delete")]
        private object apiDeleteMonitor(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("Id");

            using (var session = Context.OpenSession())
            {
                var sensor = session.Get<Monitor>(id);
                session.Delete(sensor);
                session.Flush();
            }

            //NotifyForSignalR(new { MsgId = "SensorDeleted", Data = new { Id = id } });

            return null;
        }

        [HttpCommand("/api/aquacontroller/controller/type/list")]
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
        [HttpCommand("/api/aquacontroller/controller/listall")]
        private object apiGetAllControllers(HttpRequestParams request)
        {
            return GetAllControllers().Select(BuildControllerWebModel).Where(x => x != null).ToArray();
        }
        [HttpCommand("/api/aquacontroller/controller/listvisible")]
        private object apiGetVisibleControllers(HttpRequestParams request)
        {
            using (var session = Context.OpenSession())
                return session.Query<Controller>()
                    .Where(controller => controller.IsVisible)
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
        [HttpCommand("/api/aquacontroller/controller")]
        private object apiGetController(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            return GetController(id);
        }
        [HttpCommand("/api/aquacontroller/controller/add")]
        private object apiAddController(HttpRequestParams request)
        {
            var name = request.GetRequiredString("name");
            var type = (ControllerType)request.GetRequiredInt32("type");
            var isVisible = request.GetRequiredBool("isVisible");

            var ctrl = new Controller()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Type = type,
                IsVisible = isVisible
            };

            ControllerBase controller = CreateController(ctrl);
            if (controller != null)
            {
                controller.Init(Context);
                controller.Save();
                controllers.Add(controller);

                //NotifyForSignalR(new
                //{
                //    MsgId = "SensorNameChanged",
                //    Data = new
                //    {
                //        Id = id,
                //        Name = name
                //    }
                //});
            }

            return null;
        }
        [HttpCommand("/api/aquacontroller/controller/setname")]
        private object apiSetControllerName(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            var name = request.GetString("name");

            using (var session = Context.OpenSession())
            {
                var sensor = session.Get<Controller>(id);
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
        [HttpCommand("/api/aquacontroller/controller/setisvisible")]
        private object apiSetControllerIsVisible(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            var isVisible = request.GetRequiredBool("isVisible");

            using (var session = Context.OpenSession())
            {
                var controller = session.Get<Controller>(id);
                controller.IsVisible = isVisible;
                session.Flush();
            }

            //NotifyForSignalR(new
            //{
            //    MsgId = "SensorIsVisibleChanged",
            //    Data = new
            //    {
            //        Id = id,
            //        Name = name
            //    }
            //});

            return null;
        }
        [HttpCommand("/api/aquacontroller/controller/setconfiguration")]
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
        [HttpCommand("/api/aquacontroller/controller/delete")]
        private object apiDeleteController(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("Id");

            using (var session = Context.OpenSession())
            {
                var controller = session.Get<Controller>(id);
                session.Delete(controller);
                session.Flush();
            }

            //NotifyForSignalR(new { MsgId = "SensorDeleted", Data = new { Id = id } });

            return null;
        }
        #endregion
    }
}
