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
using SmartHub.Plugins.WebUI.Attributes;
using System;
using System.Linq;

namespace SmartHub.Plugins.MeteoStation
{
    [AppSection("Метеостанция", SectionType.Common, "/webapp/meteostation/module-main.js", "SmartHub.Plugins.MeteoStation.Resources.js.module-main.js", TileTypeFullName = "SmartHub.Plugins.MeteoStation.MeteoStationTile")]
    [JavaScriptResource("/webapp/meteostation/module-main-view.js", "SmartHub.Plugins.MeteoStation.Resources.js.module-main-view.js")]
    [JavaScriptResource("/webapp/meteostation/module-main-model.js", "SmartHub.Plugins.MeteoStation.Resources.js.module-main-model.js")]
    [HttpResource("/webapp/meteostation/module-main.html", "SmartHub.Plugins.MeteoStation.Resources.js.module-main.html")]
    [CssResource("/webapp/meteostation/css/style.css", "SmartHub.Plugins.MeteoStation.Resources.css.style.css", AutoLoad = true)]

    [AppSection("Метеостанция", SectionType.System, "/webapp/meteostation/module-settings.js", "SmartHub.Plugins.MeteoStation.Resources.js.module-settings.js")]
    [JavaScriptResource("/webapp/meteostation/module-settings-view.js", "SmartHub.Plugins.MeteoStation.Resources.js.module-settings-view.js")]
    [JavaScriptResource("/webapp/meteostation/module-settings-model.js", "SmartHub.Plugins.MeteoStation.Resources.js.module-settings-model.js")]
    [HttpResource("/webapp/meteostation/module-settings.html", "SmartHub.Plugins.MeteoStation.Resources.js.module-settings.html")]

    [Plugin]
    public class MeteoStationPlugin : PluginBase
    {
        #region Fields
        private MySensorsPlugin mySensors;

        private SensorsConfiguration sensorsConfiguration;
        private MeteoStationSetting sensorsConfigurationSetting;

        private Sensor sensorTemperatureInner;
        private Sensor sensorTemperatureOuter;
        private Sensor sensorHumidityInner;
        private Sensor sensorHumidityOuter;
        private Sensor sensorAtmospherePressure;
        #endregion

        #region Plugin overrides
        public override void InitDbModel(ModelMapper mapper)
        {
            mapper.Class<MeteoStationSetting>(cfg => cfg.Table("MeteoStation_Settings"));
        }
        public override void InitPlugin()
        {
            mySensors = Context.GetPlugin<MySensorsPlugin>();

            sensorsConfigurationSetting = GetSetting("SensorsConfiguration");

            if (sensorsConfigurationSetting == null)
            {
                sensorsConfigurationSetting = new MeteoStationSetting()
                {
                    Id = Guid.NewGuid(),
                    Name = "SensorsConfiguration"
                };

                sensorsConfiguration = new SensorsConfiguration()
                {
                    SensorTemperatureInnerID = Guid.Empty,
                    SensorTemperatureOuterID = Guid.Empty,
                    SensorHumidityInnerID = Guid.Empty,
                    SensorHumidityOuterID = Guid.Empty,
                    SensorAtmospherePressureID = Guid.Empty
                };

                sensorsConfigurationSetting.SetValue(sensorsConfiguration);

                SaveOrUpdate(sensorsConfigurationSetting);
            }
            else
                sensorsConfiguration = sensorsConfigurationSetting.GetValue(typeof(SensorsConfiguration));

            sensorTemperatureInner = mySensors.GetSensor(sensorsConfiguration.SensorTemperatureInnerID);
            sensorHumidityInner = mySensors.GetSensor(sensorsConfiguration.SensorHumidityInnerID);
            sensorTemperatureOuter = mySensors.GetSensor(sensorsConfiguration.SensorTemperatureOuterID);
            sensorHumidityOuter = mySensors.GetSensor(sensorsConfiguration.SensorHumidityOuterID);
            sensorAtmospherePressure = mySensors.GetSensor(sensorsConfiguration.SensorAtmospherePressureID);
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
        private object BuildSensorWebModel(Sensor sensor)
        {
            if (sensor == null)
                return null;

            return new
            {
                Id = sensor.Id,
                //NodeNo = sensor.NodeNo,
                //SensorNo = sensor.SensorNo,
                //TypeName = sensor.TypeName,
                Name = sensor.Name
            };
        }
        #endregion

        #region Event handlers
        [MySensorsConnected]
        private void Connected()
        {
            //if (sensor != null)
            //    mySensors.RequestSensorValue(sensor, SensorValueType.Temperature);
            //if (relay != null)
            //    mySensors.RequestSensorValue(relay, SensorValueType.Switch);
        }

        [MySensorsMessage]
        private void MessageReceived(SensorMessage msg)
        {
            //if (relay != null && sensor != null && msg.NodeID == sensor.NodeNo && msg.SensorID == sensor.SensorNo)
            //    mySensors.SetSensorValue(relay, SensorValueType.Switch, msg.PayloadFloat < minTemperature ? 1 : 0);
        }
        #endregion

        #region Web API
        [HttpCommand("/api/meteostation/sensors")]
        public object GetSensors(HttpRequestParams request)
        {
            var type = request.GetInt32("type");

            using (var session = Context.OpenSession())
                return session.Query<Sensor>()
                    .Where(s => type.HasValue ? (int)s.Type == type.Value : true)
                    .Select(BuildSensorWebModel)
                    .Where(x => x != null)
                    .ToArray();
        }
        [HttpCommand("/api/meteostation/sensorsCofiguration")]
        public object GetSensorsConfiguration(HttpRequestParams request)
        {
            return sensorsConfiguration;
        }
        [HttpCommand("/api/meteostation/setSensorsCofiguration")]
        public object SetSensorsConfiguration(HttpRequestParams request)
        {
            var json = request.GetRequiredString("Value");
            sensorsConfiguration = (SensorsConfiguration)Extensions.FromJson(typeof(SensorsConfiguration), json);

            sensorsConfigurationSetting.SetValue(sensorsConfiguration);
            SaveOrUpdate(sensorsConfigurationSetting);

            return null;
        }
        #endregion
    }
}
