using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using SmartHub.Core.Plugins;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Attributes;
using SmartHub.Plugins.Informers.Data;
using SmartHub.Plugins.Monitors;
using SmartHub.Plugins.MySensors;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.MySensors.Data;
using SmartHub.Plugins.Timer.Attributes;
using SmartHub.Plugins.WebUI.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartHub.Plugins.Informers
{
    [AppSection("Информеры", SectionType.System, "/webapp/informers/settings.js", "SmartHub.Plugins.Informers.Resources.js.settings.js")]
    [JavaScriptResource("/webapp/informers/settings-view.js", "SmartHub.Plugins.Informers.Resources.js.settings-view.js")]
    [JavaScriptResource("/webapp/informers/settings-model.js", "SmartHub.Plugins.Informers.Resources.js.settings-model.js")]
    [HttpResource("/webapp/informers/settings.html", "SmartHub.Plugins.Informers.Resources.js.settings.html")]

    [CssResource("/webapp/informers/css/style.css", "SmartHub.Plugins.Informers.Resources.css.style.css", AutoLoad = true)]

    // informer editor
    [JavaScriptResource("/webapp/informers/informer-editor.js", "SmartHub.Plugins.Informers.Resources.js.informer-editor.js")]
    [JavaScriptResource("/webapp/informers/informer-editor-view.js", "SmartHub.Plugins.Informers.Resources.js.informer-editor-view.js")]
    [JavaScriptResource("/webapp/informers/informer-editor-model.js", "SmartHub.Plugins.Informers.Resources.js.informer-editor-model.js")]
    [HttpResource("/webapp/informers/informer-editor.html", "SmartHub.Plugins.Informers.Resources.js.informer-editor.html")]

    [Plugin]
    public class InformersPlugin : PluginBase
    {
        #region Fields
        private MySensorsPlugin mySensors;
        private MonitorsPlugin monitors;
        #endregion

        #region Plugin overrides
        public override void InitDbModel(ModelMapper mapper)
        {
            mapper.Class<Informer>(cfg => cfg.Table("Informers_Informers"));
        }
        public override void InitPlugin()
        {
            mySensors = Context.GetPlugin<MySensorsPlugin>();
            monitors = Context.GetPlugin<MonitorsPlugin>();
        }
        #endregion

        #region API
        public List<Informer> Get()
        {
            using (var session = Context.OpenSession())
                return session.Query<Informer>()
                    .OrderBy(informer => informer.Name)
                    .ToList();
        }
        public Informer Get(Guid id)
        {
            using (var session = Context.OpenSession())
                return session.Get<Informer>(id);
        }
        public void Delete(Guid id)
        {
            using (var session = Context.OpenSession())
            {
                var zone = session.Load<Informer>(id);
                session.Delete(zone);
                session.Flush();
            }
        }
        #endregion

        #region Private methods
        private string FormatSensorValue(SensorValue sv)
        {
            if (sv != null)
                switch (sv.Type)
                {
                    case SensorValueType.Temperature: return sv.Value.ToString("N1") + " C"; // " °C";
                    case SensorValueType.Humidity: return sv.Value.ToString("N1") + "%";
                    case SensorValueType.Pressure: return (sv.Value / 133.3).ToString("N0") + " mmHg";
                    case SensorValueType.Tripped:
                    case SensorValueType.Armed:
                    case SensorValueType.Switch: return sv.Value == 1 ? "On" : "Off";
                    case SensorValueType.Distance: return sv.Value.ToString("N0") + " cm";
                    case SensorValueType.Forecast: return (new string[] {"Clear", "Sunny", "Cloudy", "Rain", "Thunderstorm", "Unknown"})[(int)sv.Value];
                    case SensorValueType.RainRate: return (new string[] { "Dry", "Wet", "Rain", "Thunderstorm" })[(int)sv.Value];
                    //case SensorValueType.: return sv.Value.ToString("N1") + "%";
                    default:
                        return sv.Value.ToString("N1");
                }

            return "--";
        }
        private void UpdateInformer(Informer informer)
        {
            if (informer != null)
            {
                var sensorDisplay = mySensors.GetSensor(informer.SensorDisplayId);
                if (sensorDisplay != null)
                {
                    var monitorsIds = informer.GetMonitorsIds();
                    foreach (var monitorId in monitorsIds)
                    {
                        var monitor = monitors.Get(monitorId);
                        if (monitor != null)
                        {
                            var sensor = mySensors.GetSensor(monitor.SensorId);
                            if (sensor != null)
                            {
                                var lineNo = (byte)monitorsIds.IndexOf(monitorId);
                                string property = string.IsNullOrEmpty(monitor.NameForInformer) ? monitor.Name : monitor.NameForInformer;

                                var lastSV = mySensors.GetLastSensorValue(sensor);
                                string value = FormatSensorValue(lastSV);
                                
                                //string value = "--";
                                //if (lastSV != null)
                                //{
                                //    value = lastSV.Value.ToString();

                                //    //var json = string.IsNullOrWhiteSpace(monitor.Configuration) ? "{}" : monitor.Configuration;
                                //    //dynamic config = Extensions.FromJson(json);
                                //    //string valueTemplate = config["series"][0]["tooltip"]["template"];
                                //    //"#= kendo.toString(data.value, 'n1') # °C"
                                //    //"#= kendo.toString(value / 133.3, 'n2') # mmHg"

                                //    //Debug.WriteLine(lineNo + ": " + property);

                                //    //JObject obj = JObject.Parse(json);
                                //    //MyJsonObject[] objArr = JsonConvert.DeserializeObject<MyJsonObject[]>(obj["json"][0]["zayavki"].ToString());
                                //    //foreach (MyJsonObject myJsonObj in objArr)
                                //    //{
                                //    //    Console.WriteLine("Uuid: {0}", myJsonObj.Uuid);
                                //    //    Console.WriteLine("Date: {0}", myJsonObj.Date.ToString("dd/MM/yyyy"));
                                //    //    Console.WriteLine("Comment: {0}", myJsonObj.Comment);
                                //    //    Console.WriteLine("FirmID: {0}", myJsonObj.FirmID);
                                //    //    Console.WriteLine(new string('-', 10));
                                //    //}
                                //}

                                StringBuilder sb = new StringBuilder();
                                sb.AppendFormat("{0}: {1}", property, value);

                                string str = sb.ToString();
                                var fromEncodind = Encoding.UTF8; //из какой кодировки
                                var buf = fromEncodind.GetBytes(str);
                                var toEncoding = Encoding.GetEncoding(866); //в какую кодировку
                                var buf2 = Encoding.Convert(fromEncodind, toEncoding, buf);
                                str = toEncoding.GetString(buf2);

                                mySensors.SetSensorValue(sensorDisplay, lineNo, str);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Event handlers
        [RunPeriodically(1)]
        private void timer_Elapsed(DateTime now)
        {
            foreach (var informer in Get())
                UpdateInformer(informer);
        }
        #endregion

        #region Web API
        public object BuildInformerWebModel(Informer informer)
        {
            if (informer == null)
                return null;

            return new
            {
                Id = informer.Id,
                Name = informer.Name,
                SensorDisplayName = mySensors.GetSensor(informer.SensorDisplayId).Name,
                MonitorsIds = informer.MonitorsIds
            };
        }

        [HttpCommand("/api/informers/list")]
        private object apiGetInformers(HttpRequestParams request)
        {
            return Get()
                .Select(BuildInformerWebModel)
                .Where(x => x != null)
                .ToArray();
        }
        [HttpCommand("/api/informers/get")]
        private object apiGetInformer(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            return BuildInformerWebModel(Get(id));
        }

        [HttpCommand("/api/informers/add")]
        private object apiAddInformer(HttpRequestParams request)
        {
            var name = request.GetRequiredString("name");
            var sensorDisplayId = request.GetRequiredGuid("sensorDisplayId");

            using (var session = Context.OpenSession())
            {
                Informer informer = new Informer()
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    SensorDisplayId = sensorDisplayId,
                    MonitorsIds = "[]"
                };

                session.Save(informer);
                session.Flush();
            }

            return null;
        }
        [HttpCommand("/api/informers/setname")]
        private object apiSetInformerName(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            var name = request.GetRequiredString("name");

            using (var session = Context.OpenSession())
            {
                var informer = session.Load<Informer>(id);
                informer.Name = name;
                session.Flush();
            }

            return null;
        }
        [HttpCommand("/api/informers/setconfiguration")]
        private object apiSetInformerConfiguration(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            var monitorsIds = request.GetRequiredString("monitorsIds");

            using (var session = Context.OpenSession())
            {
                var informer = session.Load<Informer>(id);
                informer.MonitorsIds = monitorsIds;
                session.Flush();
            }

            return null;
        }
        [HttpCommand("/api/informers/delete")]
        private object apiDeleteInformer(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("Id");
            Delete(id);
            return null;
        }
        #endregion
    }
}
