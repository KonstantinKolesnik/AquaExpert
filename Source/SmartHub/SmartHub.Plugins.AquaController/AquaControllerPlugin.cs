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

        #region Private methods
        private object BuildSensorSummaryWebModel(Sensor sensor)
        {
            if (sensor == null)
                return null;

            return new
            {
                Id = sensor.Id,
                Name = sensor.Name
            };
        }
        #endregion

        #region Event handlers
        [MySensorsMessage]
        private void MessageReceived(SensorMessage message)
        {
            if (heaterController.IsMyMessage(message)
                
                
                
                )
                NotifyForSignalR(new { MsgId = "AquaControllerTileContent", Data = BuildTileContent() });
        }
        #endregion

        #region Web API
        [HttpCommand("/api/aquacontroller/sensorsByType")]
        public object GetSensorsByType(HttpRequestParams request)
        {
            var type = (SensorType)request.GetRequiredInt32("type");

            return mySensors.GetSensorsByType(type)
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

        [HttpCommand("/api/aquacontroller/heatercontroller/configuration")]
        public object GetHeaterControllerConfiguration(HttpRequestParams request)
        {
            return heaterController.ControllerConfiguration;
        }
        [HttpCommand("/api/aquacontroller/heatercontroller/configuration/set")]
        public object SetHeaterControllerConfiguration(HttpRequestParams request)
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
