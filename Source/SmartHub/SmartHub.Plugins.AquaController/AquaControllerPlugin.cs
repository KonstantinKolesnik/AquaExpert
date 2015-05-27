using NHibernate.Mapping.ByCode;
using SmartHub.Core.Plugins;
using SmartHub.Plugins.AquaController.Core;
using SmartHub.Plugins.AquaController.Data;
using SmartHub.Plugins.HttpListener.Attributes;
using SmartHub.Plugins.WebUI.Attributes;
using System.Text;

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
        private TemperatureController temperatureController = new TemperatureController();
        #endregion

        #region Plugin overrides
        public override void InitDbModel(ModelMapper mapper)
        {
            mapper.Class<AquaControllerSetting>(cfg => cfg.Table("AquaController_Settings"));
        }
        public override void InitPlugin()
        {
            temperatureController.Init(Context);
        }
        #endregion

        #region Public methods
        public string BuildTileContent()
        {
            //SensorValue lastSVTemperatureInner = GetLastSensorValue(SensorTemperatureInner);
            //SensorValue lastSVHumidityInner = GetLastSensorValue(SensorHumidityInner);
            //SensorValue lastSVTemperatureOuter = GetLastSensorValue(SensorTemperatureOuter);
            //SensorValue lastSVHumidityOuter = GetLastSensorValue(SensorHumidityOuter);
            //SensorValue lastSVAtmospherePressure = GetLastSensorValue(SensorAtmospherePressure);
            //SensorValue lastSVForecast = GetLastSensorValue(SensorForecast);

            StringBuilder sb = new StringBuilder();
            //sb.Append("<div>Температура внутри: " + (lastSVTemperatureInner != null ? lastSVTemperatureInner.Value + " °C" : "&lt;нет данных&gt;") + "</div>");
            //sb.Append("<div>Влажность внутри: " + (lastSVHumidityInner != null ? lastSVHumidityInner.Value + " %" : "&lt;нет данных&gt;") + "</div>");
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
