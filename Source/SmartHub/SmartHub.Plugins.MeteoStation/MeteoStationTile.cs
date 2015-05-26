using NHibernate;
using NHibernate.Linq;
using SmartHub.Plugins.MySensors.Data;
using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;
using System.Linq;
using System.Text;

namespace SmartHub.Plugins.MeteoStation
{
    [Tile]
    public class MeteoStationTile : TileBase
    {
        public override void PopulateWebModel(TileWebModel tileWebModel, dynamic options)
        {
            try
            {
                tileWebModel.title = "Метеостанция";
                tileWebModel.url = "webapp/meteostation/module-main";
                tileWebModel.className = "btn-info th-tile-icon th-tile-icon-fa fa-umbrella";
                tileWebModel.wide = true;
                tileWebModel.content = BuildContent();
                tileWebModel.SignalRReceiveHandler = BuildSignalRReceiveHandler();
            }
            catch (Exception ex)
            {
                tileWebModel.content = ex.Message;
            }
        }

        private string BuildContent()
        {
            var meteoStationPlugin = Context.GetPlugin<MeteoStationPlugin>();

            SensorValue lastSVTemperatureInner = meteoStationPlugin.GetLastSensorValue(meteoStationPlugin.SensorTemperatureInner);
            SensorValue lastSVHumidityInner = meteoStationPlugin.GetLastSensorValue(meteoStationPlugin.SensorHumidityInner);
            SensorValue lastSVTemperatureOuter = meteoStationPlugin.GetLastSensorValue(meteoStationPlugin.SensorTemperatureOuter);
            SensorValue lastSVHumidityOuter = meteoStationPlugin.GetLastSensorValue(meteoStationPlugin.SensorHumidityOuter);
            SensorValue lastSVAtmospherePressure = meteoStationPlugin.GetLastSensorValue(meteoStationPlugin.SensorAtmospherePressure);
            SensorValue lastSVForecast = meteoStationPlugin.GetLastSensorValue(meteoStationPlugin.SensorForecast);

            string result = "";
            result += "<div>Температура внутренняя: " + (lastSVTemperatureInner != null ? lastSVTemperatureInner.Value + "°C" : "&lt;нет данных&gt;") + "</div>";
            result += "<div>Влажность внутренняя: " + (lastSVHumidityInner != null ? lastSVHumidityInner.Value + "%" : "&lt;нет данных&gt;") + "</div>";
            result += "<div>Температура наружная: " + (lastSVTemperatureOuter != null ? lastSVTemperatureOuter.Value + "°C" : "&lt;нет данных&gt;") + "</div>";
            result += "<div>Влажность наружная: " + (lastSVHumidityOuter != null ? lastSVHumidityOuter.Value + "%" : "&lt;нет данных&gt;") + "</div>";
            result += "<div>Атмосферное давление: " + (lastSVAtmospherePressure != null ? (int)(lastSVAtmospherePressure.Value /133.3f) + "mmHg" : "&lt;нет данных&gt;") + "</div>";
            result += "<div>Прогноз: " + (lastSVForecast != null ? lastSVForecast.Value + "" : "&lt;нет данных&gt;") + "</div>";
            //const char *weather[] = { "stable", "sunny", "cloudy", "unstable", "thunderstorm", "unknown" };

            return result;
        }
        private string BuildSignalRReceiveHandler()
        {
            StringBuilder sb = new StringBuilder();

            //sb.Append("if (data.MsgId == 'SensorValue') { ");
            //sb.Append("var dt = kendo.toString(new Date(data.Data.TimeStamp), 'dd.MM.yyyy'); ");
            //sb.Append("var tm = kendo.toString(new Date(data.Data.TimeStamp), 'HH:mm:ss'); ");
            //sb.Append("var val = '[' + data.Data.NodeNo + '][' + data.Data.SensorNo + '] ' + data.Data.TypeName + ': ' + data.Data.Value; ");
            //sb.Append("var result = '<span>' + dt + '</span>&nbsp;&nbsp;'; ");
            //sb.Append("result += '<span style=\"font-size:0.9em; font-style:italic;\">' + tm + '</span>'; ");
            //sb.Append("result += '<div>' + val + '</div>'; ");
            //sb.Append("model.tileModel.set({ 'content': result }); ");
            //sb.Append("}");

            return sb.ToString();
        }
    }
}
