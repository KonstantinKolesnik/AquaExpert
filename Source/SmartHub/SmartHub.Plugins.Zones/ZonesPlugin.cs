﻿using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using SmartHub.Core.Plugins;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Attributes;
using SmartHub.Plugins.MySensors;
using SmartHub.Plugins.MySensors.Attributes;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.SignalR;
using SmartHub.Plugins.Timer.Attributes;
using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.Zones.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartHub.Plugins.Zones
{
    [AppSection("Зоны", SectionType.Common, "/webapp/zones/dashboard.js", "SmartHub.Plugins.Zones.Resources.js.dashboard.js")]
    [JavaScriptResource("/webapp/zones/dashboard-view.js", "SmartHub.Plugins.Zones.Resources.js.dashboard-view.js")]
    [JavaScriptResource("/webapp/zones/dashboard-model.js", "SmartHub.Plugins.Zones.Resources.js.dashboard-model.js")]
    [HttpResource("/webapp/zones/dashboard.html", "SmartHub.Plugins.Zones.Resources.js.dashboard.html")]

    [AppSection("Зоны", SectionType.System, "/webapp/zones/settings.js", "SmartHub.Plugins.Zones.Resources.js.settings.js", TileTypeFullName = "SmartHub.Plugins.Zones.ZonesTile")]
    [JavaScriptResource("/webapp/zones/settings-view.js", "SmartHub.Plugins.Zones.Resources.js.settings-view.js")]
    [JavaScriptResource("/webapp/zones/settings-model.js", "SmartHub.Plugins.Zones.Resources.js.settings-model.js")]
    [HttpResource("/webapp/zones/settings.html", "SmartHub.Plugins.Zones.Resources.js.settings.html")]

    [CssResource("/webapp/zones/css/style.css", "SmartHub.Plugins.Zones.Resources.css.style.css", AutoLoad = true)]

    // zone editor
    [JavaScriptResource("/webapp/zones/zone-editor.js", "SmartHub.Plugins.Zones.Resources.js.zone-editor.js")]
    [JavaScriptResource("/webapp/zones/zone-editor-view.js", "SmartHub.Plugins.Zones.Resources.js.zone-editor-view.js")]
    [JavaScriptResource("/webapp/zones/zone-editor-model.js", "SmartHub.Plugins.Zones.Resources.js.zone-editor-model.js")]
    [HttpResource("/webapp/zones/zone-editor.html", "SmartHub.Plugins.Zones.Resources.js.zone-editor.html")]

    [Plugin]
    public class ZonesPlugin : PluginBase
    {
        #region Fields
        private MySensorsPlugin mySensors;
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
            mapper.Class<Zone>(cfg => cfg.Table("Zones_Zones"));
        }
        public override void InitPlugin()
        {
            mySensors = Context.GetPlugin<MySensorsPlugin>();
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
            sb.Append("if (data.MsgId == 'ZonesTileContent') { ");
            sb.Append("model.tileModel.set({ 'content': data.Data }); ");
            sb.Append("}");
            return sb.ToString();
        }
        #endregion

        #region Private methods
        private List<Zone> GetZones()
        {
            using (var session = Context.OpenSession())
                return session.Query<Zone>()
                    .OrderBy(zone => zone.Name)
                    .ToList();
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
                ControllersList = zone.ControllersList ?? "[]",
                ScriptsList = zone.ScriptsList ?? "[]",
                GraphsList = zone.GraphsList ?? "[]"
            };
        }
        #endregion

        #region Event handlers
        [MySensorsConnected]
        private void Connected()
        {
        }

        [MySensorsMessageCalibration]
        private void MessageCalibration(SensorMessage message)
        {
        }

        [MySensorsMessage]
        private void MessageReceived(SensorMessage message)
        {
        }

        //[RunPeriodically(1)]
        [Timer_10_sec_Elapsed]
        private void timer_Elapsed(DateTime now)
        {
        }
        #endregion

        #region Web API
        [HttpCommand("/api/zones/list")]
        private object apiGetZones(HttpRequestParams request)
        {
            return GetZones()
                .Select(BuildZoneWebModel)
                .Where(x => x != null)
                .ToArray();
        }
        [HttpCommand("/api/zones/get")]
        private object apiGetZone(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");

            using (var session = Context.OpenSession())
                return BuildZoneWebModel(session.Get<Zone>(id));
        }
        [HttpCommand("/api/zones/add")]
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
        [HttpCommand("/api/zones/setname")]
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
        [HttpCommand("/api/zones/setconfiguration")]
        private object apiSetZoneConfiguration(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            var monitorsList = request.GetRequiredString("monitorsList");
            var controllersList = request.GetRequiredString("controllersList");
            var scriptsList = request.GetRequiredString("scriptsList");
            var graphsList = request.GetRequiredString("graphsList");

            using (var session = Context.OpenSession())
            {
                var zone = session.Load<Zone>(id);
                zone.MonitorsList = monitorsList;
                zone.ControllersList = controllersList;
                zone.ScriptsList = scriptsList;
                zone.GraphsList = graphsList;

                session.Flush();
            }

            //NotifyForSignalR(new { MsgId = "ControllerIsVisibleChanged", Data = BuildControllerWebModel(ctrl) });

            return null;
        }
        [HttpCommand("/api/zones/delete")]
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
        #endregion
    }
}