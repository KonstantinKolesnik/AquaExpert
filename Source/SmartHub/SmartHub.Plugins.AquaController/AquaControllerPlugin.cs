using NHibernate.Mapping.ByCode;
using SmartHub.Core.Plugins;
using SmartHub.Plugins.AquaController.Core;
using SmartHub.Plugins.AquaController.Data;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Attributes;
using SmartHub.Plugins.MySensors;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.MySensors.Data;
using SmartHub.Plugins.WebUI.Attributes;
using System.Text;
using System.Linq;
using System;
using NHibernate.Linq;
using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.MySensors.Attributes;
using SmartHub.Plugins.SignalR;
using SmartHub.Plugins.Timer.Attributes;
using System.Collections.Generic;

namespace SmartHub.Plugins.AquaController
{
    [AppSection("Аква-контроллер", SectionType.Common, "/webapp/aquacontroller/module-main.js", "SmartHub.Plugins.AquaController.Resources.js.module-main.js", TileTypeFullName = "SmartHub.Plugins.AquaController.AquaControllerTile")]
    [JavaScriptResource("/webapp/aquacontroller/module-main-view.js", "SmartHub.Plugins.AquaController.Resources.js.module-main-view.js")]
    [JavaScriptResource("/webapp/aquacontroller/module-main-model.js", "SmartHub.Plugins.AquaController.Resources.js.module-main-model.js")]
    [HttpResource("/webapp/aquacontroller/module-main.html", "SmartHub.Plugins.AquaController.Resources.js.module-main.html")]

    [AppSection("Аква-контроллер", SectionType.System, "/webapp/aquacontroller/module-settings.js", "SmartHub.Plugins.AquaController.Resources.js.module-settings.js")]
    [JavaScriptResource("/webapp/aquacontroller/module-settings-view.js", "SmartHub.Plugins.AquaController.Resources.js.module-settings-view.js")]
    [JavaScriptResource("/webapp/aquacontroller/module-settings-model.js", "SmartHub.Plugins.AquaController.Resources.js.module-settings-model.js")]
    [HttpResource("/webapp/aquacontroller/module-settings.html", "SmartHub.Plugins.AquaController.Resources.js.module-settings.html")]

    [CssResource("/webapp/aquacontroller/css/style.css", "SmartHub.Plugins.AquaController.Resources.css.style.css", AutoLoad = true)]

    [Plugin]
    public class AquaControllerPlugin : PluginBase
    {
        #region Fields
        private MySensorsPlugin mySensors;
        private HeaterController heaterController = new HeaterController();

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
        }
        public override void InitPlugin()
        {
            mySensors = Context.GetPlugin<MySensorsPlugin>();

            heaterController.Init(Context);


        }
        #endregion

        #region Public methods
        public string BuildTileContent()
        {
            SensorValue lastSVHeaterTemperature = mySensors.GetLastSensorValue(heaterController.SensorTemperature);
            SensorValue lastSVHeaterSwitch = mySensors.GetLastSensorValue(heaterController.SensorSwitch);
            //SensorValue lastSVTemperatureOuter = mySensors.GetLastSensorValue(SensorTemperatureOuter);
            //SensorValue lastSVHumidityOuter = mySensors.GetLastSensorValue(SensorHumidityOuter);
            //SensorValue lastSVAtmospherePressure = mySensors.GetLastSensorValue(SensorAtmospherePressure);
            //SensorValue lastSVForecast = mySensors.GetLastSensorValue(SensorForecast);

            StringBuilder sb = new StringBuilder();
            sb.Append("<div>Температура воды: " + (lastSVHeaterTemperature != null ? lastSVHeaterTemperature.Value + " °C" : "&lt;нет данных&gt;") + "</div>");
            sb.Append("<div>Обогреватель: " + (lastSVHeaterSwitch != null ? (lastSVHeaterSwitch.Value == 1 ? "Вкл." : "Выкл.") : "&lt;нет данных&gt;") + "</div>");
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

        #region Event handlers
        [MySensorsConnected]
        private void Connected()
        {
            heaterController.Connected();


        }

        [MySensorsMessageCalibration]
        private void MessageCalibration(SensorMessage message)
        {
            heaterController.MessageCalibration(message);


        }

        [MySensorsMessage]
        private void MessageReceived(SensorMessage message)
        {
            heaterController.MessageReceived(message);



            if (heaterController.IsMyMessage(message)
                //|| hhhhjiujijhuhgo
                
                
                )
                NotifyForSignalR(new { MsgId = "AquaControllerTileContent", Data = BuildTileContent() });
        }

        //[RunPeriodically(1)]
        [Timer_5_sec_Elapsed]
        private void timer_Elapsed(DateTime now)
        {
            heaterController.TimerElapsed(now);

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
                        SensorValues = mySensors.GetSensorValuesByID(monitor.SensorId, 48).ToArray()
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
        private object apiControllerTypes(HttpRequestParams request)
        {
            //Enum.GetNames
            //ControllerType.

            //using (var session = Context.OpenSession())
            //    return session.Query<Controller>()
            //        .OrderBy(controller => controller.Name)
            //        .Select(controller => new
            //        {
            //            Id = controller.Id,
            //            Name = controller.Name,
            //            //Sensor = mySensors.BuildSensorWebModel(mySensors.GetSensor(monitor.SensorId)),
            //            IsVisible = controller.IsVisible
            //        })
            //        .ToArray();

            return new List<ControllerType>().ToArray();
        }

        [HttpCommand("/api/aquacontroller/controller/listall")]
        private object apiGetAllControllers(HttpRequestParams request)
        {
            using (var session = Context.OpenSession())
                return session.Query<Controller>()
                    .OrderBy(controller => controller.Name)
                    .Select(controller => new
                    {
                        Id = controller.Id,
                        Name = controller.Name,
                        Type = controller.Type.ToString(),
                        IsVisible = controller.IsVisible
                    })
                    .ToArray();
        }







        [HttpCommand("/api/aquacontroller/heatercontroller/configuration")]
        public object apiGetHeaterControllerConfiguration(HttpRequestParams request)
        {
            return heaterController.ControllerConfiguration;
        }
        [HttpCommand("/api/aquacontroller/heatercontroller/configuration/set")]
        public object apiSetHeaterControllerConfiguration(HttpRequestParams request)
        {
            var json = request.GetRequiredString("conf");
            heaterController.ControllerConfiguration = (HeaterController.Configuration)Extensions.FromJson(typeof(HeaterController.Configuration), json);

            return null;
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
    }
}
