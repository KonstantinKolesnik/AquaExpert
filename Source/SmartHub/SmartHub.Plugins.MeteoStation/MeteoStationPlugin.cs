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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartHub.Plugins.MeteoStation
{
    //[AppSection("Метеостанция", SectionType.Common, "/webapp/meteostation/dashboard.js", "SmartHub.Plugins.MeteoStation.Resources.js.dashboard.js", TileTypeFullName = "SmartHub.Plugins.MeteoStation.MeteoStationTile")]
    [JavaScriptResource("/webapp/meteostation/dashboard-view.js", "SmartHub.Plugins.MeteoStation.Resources.js.dashboard-view.js")]
    [JavaScriptResource("/webapp/meteostation/dashboard-model.js", "SmartHub.Plugins.MeteoStation.Resources.js.dashboard-model.js")]
    [HttpResource("/webapp/meteostation/dashboard.html", "SmartHub.Plugins.MeteoStation.Resources.js.dashboard.html")]

    //[AppSection("Метеостанция", SectionType.System, "/webapp/meteostation/settings.js", "SmartHub.Plugins.MeteoStation.Resources.js.settings.js")]
    [JavaScriptResource("/webapp/meteostation/settings-view.js", "SmartHub.Plugins.MeteoStation.Resources.js.settings-view.js")]
    [JavaScriptResource("/webapp/meteostation/settings-model.js", "SmartHub.Plugins.MeteoStation.Resources.js.settings-model.js")]
    [HttpResource("/webapp/meteostation/settings.html", "SmartHub.Plugins.MeteoStation.Resources.js.settings.html")]

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
        private MySensorsPlugin mySensors;
        private Configuration configuration;
        private MeteoStationSetting configurationSetting;
        private const string settingName = "SensorsConfiguration";
        #endregion

        #region Properties
        public Sensor SensorTemperatureInner
        {
            get { return mySensors.GetSensor(configuration.SensorTemperatureInnerID); }
        }
        public Sensor SensorHumidityInner
        {
            get { return mySensors.GetSensor(configuration.SensorHumidityInnerID); }
        }
        public Sensor SensorTemperatureOuter
        {
            get { return mySensors.GetSensor(configuration.SensorTemperatureOuterID); }
        }
        public Sensor SensorHumidityOuter
        {
            get { return mySensors.GetSensor(configuration.SensorHumidityOuterID); }
        }
        public Sensor SensorAtmospherePressure
        {
            get { return mySensors.GetSensor(configuration.SensorAtmospherePressureID); }
        }
        public Sensor SensorForecast
        {
            get { return mySensors.GetSensor(configuration.SensorForecastID); }
        }
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
            mapper.Class<MeteoStationSetting>(cfg => cfg.Table("MeteoStation_Settings"));
        }
        public override void InitPlugin()
        {
            mySensors = Context.GetPlugin<MySensorsPlugin>();

            configurationSetting = GetSetting(settingName);

            if (configurationSetting == null)
            {
                configurationSetting = new MeteoStationSetting()
                {
                    Id = Guid.NewGuid(),
                    Name = settingName
                };

                configuration = Configuration.Default;
                configurationSetting.SetValue(configuration);
                SaveOrUpdate(configurationSetting);
            }
            else
                configuration = configurationSetting.GetValue(typeof(Configuration));
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

        private void RequestSensorsValues()
        {
            mySensors.RequestSensorValue(SensorTemperatureInner, SensorValueType.Temperature);
            mySensors.RequestSensorValue(SensorHumidityInner, SensorValueType.Humidity);
            mySensors.RequestSensorValue(SensorTemperatureOuter, SensorValueType.Temperature);
            mySensors.RequestSensorValue(SensorHumidityOuter, SensorValueType.Humidity);
            mySensors.RequestSensorValue(SensorAtmospherePressure, SensorValueType.Pressure);
            mySensors.RequestSensorValue(SensorForecast, SensorValueType.Forecast);
        }

        private object ConvertSensorToMonitor(Guid sensorID, string name, string type)
        {
            return new
            {
                Name = name,
                Type = type,
                Sensor = mySensors.BuildSensorRichWebModel(mySensors.GetSensor(sensorID)),
                SensorValues = mySensors.GetSensorValues(sensorID, 24, 30).ToArray()
            };
        }
        #endregion

        #region Event handlers
        [MySensorsConnected]
        private void Connected()
        {
            RequestSensorsValues();
        }

        [MySensorsMessage]
        private void MessageReceived(SensorMessage message)
        {
            if (MySensorsPlugin.IsMessageFromSensor(message, SensorTemperatureInner) ||
                MySensorsPlugin.IsMessageFromSensor(message, SensorHumidityInner) ||
                MySensorsPlugin.IsMessageFromSensor(message, SensorTemperatureOuter) ||
                MySensorsPlugin.IsMessageFromSensor(message, SensorHumidityOuter) ||
                MySensorsPlugin.IsMessageFromSensor(message, SensorAtmospherePressure) ||
                MySensorsPlugin.IsMessageFromSensor(message, SensorForecast))
                NotifyForSignalR(new { MsgId = "MeteoStationTileContent", Data = BuildTileContent() });
        }
        #endregion

        #region Web API
        public string BuildTileContent()
        {
            SensorValue lastSVTemperatureInner = mySensors.GetLastSensorValue(SensorTemperatureInner);
            SensorValue lastSVHumidityInner = mySensors.GetLastSensorValue(SensorHumidityInner);
            SensorValue lastSVTemperatureOuter = mySensors.GetLastSensorValue(SensorTemperatureOuter);
            SensorValue lastSVHumidityOuter = mySensors.GetLastSensorValue(SensorHumidityOuter);
            SensorValue lastSVAtmospherePressure = mySensors.GetLastSensorValue(SensorAtmospherePressure);
            SensorValue lastSVForecast = mySensors.GetLastSensorValue(SensorForecast);

            StringBuilder sb = new StringBuilder();
            sb.Append("<div>Т <sub>in</sub>:  " + (lastSVTemperatureInner != null ? lastSVTemperatureInner.Value + " °C" : "&lt;нет данных&gt;") + "</div>");
            sb.Append("<div>Hum <sub>in</sub>:  " + (lastSVHumidityInner != null ? lastSVHumidityInner.Value + " %" : "&lt;нет данных&gt;") + "</div>");
            sb.Append("<div>T <sub>out</sub>:  " + (lastSVTemperatureOuter != null ? lastSVTemperatureOuter.Value + " °C" : "&lt;нет данных&gt;") + "</div>");
            sb.Append("<div>Hum <sub>out</sub>:  " + (lastSVHumidityOuter != null ? lastSVHumidityOuter.Value + " %" : "&lt;нет данных&gt;") + "</div>");
            sb.Append("<div>P: " + (lastSVAtmospherePressure != null ? (int)(lastSVAtmospherePressure.Value / 133.3f) + " mmHg" : "&lt;нет данных&gt;") + "</div>");
            
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

        [HttpCommand("/api/meteostation/monitor/list")]
        private object apiGetMonitors(HttpRequestParams request)
        {
            List<object> result = new List<object>();

            result.Add(ConvertSensorToMonitor(configuration.SensorTemperatureInnerID, "Температура внутри", "T"));
            result.Add(ConvertSensorToMonitor(configuration.SensorHumidityInnerID, "Влажность внутри", "H"));
            result.Add(ConvertSensorToMonitor(configuration.SensorTemperatureOuterID, "Температура снаружи", "T"));
            result.Add(ConvertSensorToMonitor(configuration.SensorHumidityOuterID, "Влажность снаружи", "H"));
            result.Add(ConvertSensorToMonitor(configuration.SensorAtmospherePressureID, "Давление", "P"));
            result.Add(ConvertSensorToMonitor(configuration.SensorForecastID, "Прогноз", "F"));

            return result.ToArray();
        }

        [HttpCommand("/api/meteostation/configuration")]
        public object apiGetConfiguration(HttpRequestParams request)
        {
            return configuration;
        }
        [HttpCommand("/api/meteostation/configuration/set")]
        public object apiSetConfiguration(HttpRequestParams request)
        {
            var conf = request.GetRequiredString("conf");

            configuration = (Configuration)Extensions.FromJson(typeof(Configuration), conf);
            configurationSetting.SetValue(configuration);
            SaveOrUpdate(configurationSetting);

            RequestSensorsValues();

            return null;
        }
        #endregion
    }
}
