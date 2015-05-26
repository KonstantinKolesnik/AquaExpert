using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using SmartHub.Core.Plugins;
using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Attributes;
using SmartHub.Plugins.MeteoStation.Data;
using SmartHub.Plugins.MySensors;
using SmartHub.Plugins.MySensors.Attributes;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.MySensors.Data;
using SmartHub.Plugins.SignalR;
using SmartHub.Plugins.WebUI.Attributes;
using System;
using System.Linq;
using System.Text;

namespace SmartHub.Plugins.MeteoStation
{
    [AppSection("Метеостанция", SectionType.Common, "/webapp/meteostation/module-main.js", "SmartHub.Plugins.MeteoStation.Resources.js.module-main.js", TileTypeFullName = "SmartHub.Plugins.MeteoStation.MeteoStationTile")]
    [JavaScriptResource("/webapp/meteostation/module-main-view.js", "SmartHub.Plugins.MeteoStation.Resources.js.module-main-view.js")]
    [JavaScriptResource("/webapp/meteostation/module-main-model.js", "SmartHub.Plugins.MeteoStation.Resources.js.module-main-model.js")]
    [HttpResource("/webapp/meteostation/module-main.html", "SmartHub.Plugins.MeteoStation.Resources.js.module-main.html")]

    [AppSection("Метеостанция", SectionType.System, "/webapp/meteostation/module-settings.js", "SmartHub.Plugins.MeteoStation.Resources.js.module-settings.js")]
    [JavaScriptResource("/webapp/meteostation/module-settings-view.js", "SmartHub.Plugins.MeteoStation.Resources.js.module-settings-view.js")]
    [JavaScriptResource("/webapp/meteostation/module-settings-model.js", "SmartHub.Plugins.MeteoStation.Resources.js.module-settings-model.js")]
    [HttpResource("/webapp/meteostation/module-settings.html", "SmartHub.Plugins.MeteoStation.Resources.js.module-settings.html")]

    [CssResource("/webapp/meteostation/css/style.css", "SmartHub.Plugins.MeteoStation.Resources.css.style.css", AutoLoad = true)]
    [CssResource("/webapp/meteostation/css/weather-icons.min.css", "SmartHub.Plugins.MeteoStation.Resources.css.weather-icons.min.css", AutoLoad = true)]
    [HttpResource("/webapp/meteostation/fonts/weathericons-regular-webfont.eot", "SmartHub.Plugins.MeteoStation.Resources.fonts.weathericons-regular-webfont.eot", "application/vnd.ms-fontobject")]
    [HttpResource("/webapp/meteostation/fonts/weathericons-regular-webfont.svg", "SmartHub.Plugins.MeteoStation.Resources.fonts.weathericons-regular-webfont.svg", "image/svg+xml")]
    [HttpResource("/webapp/meteostation/fonts/weathericons-regular-webfont.ttf", "SmartHub.Plugins.MeteoStation.Resources.fonts.weathericons-regular-webfont.ttf", "application/x-font-truetype")]
    [HttpResource("/webapp/meteostation/fonts/weathericons-regular-webfont.woff", "SmartHub.Plugins.MeteoStation.Resources.fonts.weathericons-regular-webfont.woff", "application/font-woff")]

    [Plugin]
    public class MeteoStationPlugin : PluginBase
    {
        #region Fields
        private SignalRPlugin signalServer;
        private MySensorsPlugin mySensors;
        private SensorsConfiguration sensorsConfiguration;
        private MeteoStationSetting sensorsConfigurationSetting;
        #endregion

        #region Properties
        public Sensor SensorTemperatureInner
        {
            get { return mySensors.GetSensor(sensorsConfiguration.SensorTemperatureInnerID); }
        }
        public Sensor SensorHumidityInner
        {
            get { return mySensors.GetSensor(sensorsConfiguration.SensorHumidityInnerID); }
        }
        public Sensor SensorTemperatureOuter
        {
            get { return mySensors.GetSensor(sensorsConfiguration.SensorTemperatureOuterID); }
        }
        public Sensor SensorHumidityOuter
        {
            get { return mySensors.GetSensor(sensorsConfiguration.SensorHumidityOuterID); }
        }
        public Sensor SensorAtmospherePressure
        {
            get { return mySensors.GetSensor(sensorsConfiguration.SensorAtmospherePressureID); }
        }
        public Sensor SensorForecast
        {
            get { return mySensors.GetSensor(sensorsConfiguration.SensorForecastID); }
        }
        #endregion

        #region SignalR events
        private void NotifyForSignalR(object msg)
        {
            signalServer.Broadcast(msg);
        }
        #endregion

        #region Plugin overrides
        public override void InitDbModel(ModelMapper mapper)
        {
            mapper.Class<MeteoStationSetting>(cfg => cfg.Table("MeteoStation_Settings"));
        }
        public override void InitPlugin()
        {
            signalServer = Context.GetPlugin<SignalRPlugin>();
            mySensors = Context.GetPlugin<MySensorsPlugin>();

            sensorsConfigurationSetting = GetSetting("SensorsConfiguration");

            if (sensorsConfigurationSetting == null)
            {
                sensorsConfigurationSetting = new MeteoStationSetting()
                {
                    Id = Guid.NewGuid(),
                    Name = "SensorsConfiguration"
                };

                sensorsConfiguration = SensorsConfiguration.Default;
                sensorsConfigurationSetting.SetValue(sensorsConfiguration);
                SaveOrUpdate(sensorsConfigurationSetting);
            }
            else
                sensorsConfiguration = sensorsConfigurationSetting.GetValue(typeof(SensorsConfiguration));
        }
        #endregion

        #region Public methods
        public string BuildTileContent()
        {
            SensorValue lastSVTemperatureInner = GetLastSensorValue(SensorTemperatureInner);
            SensorValue lastSVHumidityInner = GetLastSensorValue(SensorHumidityInner);
            SensorValue lastSVTemperatureOuter = GetLastSensorValue(SensorTemperatureOuter);
            SensorValue lastSVHumidityOuter = GetLastSensorValue(SensorHumidityOuter);
            SensorValue lastSVAtmospherePressure = GetLastSensorValue(SensorAtmospherePressure);
            SensorValue lastSVForecast = GetLastSensorValue(SensorForecast);

            StringBuilder sb = new StringBuilder();
            sb.Append("<div>Температура внутри: " + (lastSVTemperatureInner != null ? lastSVTemperatureInner.Value + " °C" : "&lt;нет данных&gt;") + "</div>");
            sb.Append("<div>Влажность внутри: " + (lastSVHumidityInner != null ? lastSVHumidityInner.Value + " %" : "&lt;нет данных&gt;") + "</div>");
            sb.Append("<div>Температура снаружи: " + (lastSVTemperatureOuter != null ? lastSVTemperatureOuter.Value + " °C" : "&lt;нет данных&gt;") + "</div>");
            sb.Append("<div>Влажность снаружи: " + (lastSVHumidityOuter != null ? lastSVHumidityOuter.Value + " %" : "&lt;нет данных&gt;") + "</div>");
            sb.Append("<div>Давление: " + (lastSVAtmospherePressure != null ? (int)(lastSVAtmospherePressure.Value / 133.3f) + " mmHg" : "&lt;нет данных&gt;") + "</div>");
            
            string[] weather = { "Ясно", "Солнечно", "Облачно", "К дождю", "Дождь", "-" };
            sb.Append("<div>Прогноз: " + (lastSVForecast != null ? weather[(int)lastSVForecast.Value] : "&lt;нет данных&gt;") + "</div>");
            return sb.ToString();
        }
        public string BuildSignalRReceiveHandler()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("if (data.MsgId == 'MeteoStationTileContent') { ");
            sb.Append("model.tileModel.set({ 'content': data.Data }); ");
            sb.Append("}");
            return sb.ToString();
        }
        #endregion

        #region Private methods
        private MeteoStationSetting GetSetting(string name)
        {
            using (var session = Context.OpenSession())
                return session.Query<MeteoStationSetting>().FirstOrDefault(setting => setting.Name == name);
        }
        private void SaveOrUpdate(object item)
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
        
        private void UpdateSensorsValues()
        {
            Sensor sensor;

            if ((sensor = SensorTemperatureInner) != null)
                mySensors.RequestSensorValue(sensor, SensorValueType.Temperature);
            if ((sensor = SensorHumidityInner) != null)
                mySensors.RequestSensorValue(sensor, SensorValueType.Humidity);
            if ((sensor = SensorTemperatureOuter) != null)
                mySensors.RequestSensorValue(sensor, SensorValueType.Temperature);
            if ((sensor = SensorHumidityOuter) != null)
                mySensors.RequestSensorValue(sensor, SensorValueType.Humidity);
            if ((sensor = SensorAtmospherePressure) != null)
                mySensors.RequestSensorValue(sensor, SensorValueType.Pressure);
            if ((sensor = SensorForecast) != null)
                mySensors.RequestSensorValue(sensor, SensorValueType.Forecast);
        }
        private SensorValue GetLastSensorValue(Sensor sensor)
        {
            if (sensor == null)
                return null;

            using (var session = Context.OpenSession())
                return session.Query<SensorValue>().Where(sv => sv.NodeNo == sensor.NodeNo && sv.SensorNo == sensor.SensorNo).OrderByDescending(sv => sv.TimeStamp).FirstOrDefault();
        }
        private object BuildSensorSummaryWebModel(Sensor sensor)
        {
            if (sensor == null)
                return null;

            return new
            {
                Id = sensor.Id,
                //NodeNo = sensor.NodeNo,
                //SensorNo = sensor.SensorNo,
                Name = sensor.Name
            };
        }
        private bool IsMessageFromSensor(SensorMessage msg, Sensor sensor)
        {
            return sensor.NodeNo == msg.NodeNo && sensor.SensorNo == msg.SensorNo;
        }
        #endregion

        #region Event handlers
        [MySensorsConnected]
        private void Connected()
        {
            UpdateSensorsValues();
        }

        [MySensorsMessage]
        private void MessageReceived(SensorMessage message)
        {
            if (IsMessageFromSensor(message, SensorTemperatureInner) ||
                IsMessageFromSensor(message, SensorHumidityInner) ||
                IsMessageFromSensor(message, SensorTemperatureOuter) ||
                IsMessageFromSensor(message, SensorHumidityOuter) ||
                IsMessageFromSensor(message, SensorAtmospherePressure) ||
                IsMessageFromSensor(message, SensorForecast))
                NotifyForSignalR(new { MsgId = "MeteoStationTileContent", Data = BuildTileContent() });
        }
        #endregion

        #region Web API
        [HttpCommand("/api/meteostation/sensorsDataSource")]
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
        [HttpCommand("/api/meteostation/sensor")]
        public object GetSensor(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            return mySensors.BuildSensorWebModel(mySensors.GetSensor(id));
        }
        [HttpCommand("/api/meteostation/sensorvalues")]
        public object GetSensorValues(HttpRequestParams request)
        {
            var nodeNo = request.GetRequiredInt32("nodeNo");
            var sensorNo = request.GetRequiredInt32("sensorNo");
            var hours = request.GetRequiredInt32("hours");

            DateTime dt = DateTime.UtcNow.AddHours(-hours);

            using (var session = Context.OpenSession())
                return session.Query<SensorValue>().Where(sv => sv.NodeNo == nodeNo && sv.SensorNo == sensorNo && sv.TimeStamp >= dt).ToArray();
        }

        [HttpCommand("/api/meteostation/sensorsCofiguration")]
        public object GetSensorsConfiguration(HttpRequestParams request)
        {
            return sensorsConfiguration;
        }
        [HttpCommand("/api/meteostation/sensorsCofiguration/set")]
        public object SetSensorsConfiguration(HttpRequestParams request)
        {
            var json = request.GetRequiredString("sc");
            sensorsConfiguration = (SensorsConfiguration)Extensions.FromJson(typeof(SensorsConfiguration), json);
            sensorsConfigurationSetting.SetValue(sensorsConfiguration);
            SaveOrUpdate(sensorsConfigurationSetting);

            UpdateSensorsValues();

            return null;
        }
        #endregion
    }
}
