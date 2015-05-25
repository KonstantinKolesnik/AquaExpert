using SmartHub.Plugins.MySensors.Data;
using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;
using System.Text;
using NHibernate.Linq;
using System.Linq;
using NHibernate;

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
                tileWebModel.url = "/webapp/meteostation/module-main.js";
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
            string result = "";

            SensorValue lastSVTemperatureInner = null;
            SensorValue lastSVHumidityInner = null;
            SensorValue lastSVTemperatureOuter = null;
            SensorValue lastSVHumidityOuter = null;
            SensorValue lastSVAtmospherePressure = null;

            var meteoStationPlugin = Context.GetPlugin<MeteoStationPlugin>();

            using (var session = Context.OpenSession())
            {
                lastSVTemperatureInner = GetLastSensorValue(meteoStationPlugin.SensorTemperatureInner, session);
                lastSVHumidityInner = GetLastSensorValue(meteoStationPlugin.SensorHumidityInner, session);
                lastSVTemperatureOuter = GetLastSensorValue(meteoStationPlugin.SensorTemperatureOuter, session);
                lastSVHumidityOuter = GetLastSensorValue(meteoStationPlugin.SensorHumidityOuter, session);
                lastSVAtmospherePressure = GetLastSensorValue(meteoStationPlugin.SensorAtmospherePressure, session);
            }

            result += "<div>Температура внутренняя: " + (lastSVTemperatureInner != null ? lastSVTemperatureInner.Value + "°C" : "") + "</div>";
            result += "<div>Влажность внутренняя: " + (lastSVHumidityInner != null ? lastSVHumidityInner.Value + "%" : "") + "</div>";
            result += "<div>Температура наружная: " + (lastSVTemperatureOuter != null ? lastSVTemperatureOuter.Value + "°C" : "") + "</div>";
            result += "<div>Влажность наружная: " + (lastSVHumidityOuter != null ? lastSVHumidityOuter.Value + "%" : "") + "</div>";
            result += "<div>Атмосферное давление: " + (lastSVAtmospherePressure != null ? lastSVAtmospherePressure.Value + "" : "") + "</div>";

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

        private SensorValue GetLastSensorValue(Sensor sensor, ISession session)
        {
            return sensor == null ? null : session.Query<SensorValue>().Where(sv => sv.NodeNo == sensor.NodeNo && sv.SensorNo == sensor.SensorNo).OrderByDescending(sv => sv.TimeStamp).FirstOrDefault();
        }
    }
}
