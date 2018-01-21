using Newtonsoft.Json;
using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.ApiListener;
using SmartHub.UWP.Plugins.ApiListener.Attributes;
using SmartHub.UWP.Plugins.Scripts;
using SmartHub.UWP.Plugins.Scripts.Attributes;
using SmartHub.UWP.Plugins.Things.Models;
using SmartHub.UWP.Plugins.UI.Attributes;
using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace SmartHub.UWP.Plugins.Things
{
    [AppSectionItem("Things", AppSectionType.System, null, "Represents devices and their lines", "/api/things/ui/settings")]
    public class ThingsPlugin : PluginBase
    {
        #region Fields
        private Task taskValues;
        private CancellationTokenSource ctsValues;
        private bool isTaskValuesActive = false;
        #endregion

        #region Imports
        //[ImportMany]
        //public Action<WemosMessage>[] WemosMessageHandlers
        //{
        //    get; set;
        //}

        //[ScriptEvent("things.XXX")]
        //public ScriptEventHandlerDelegate[] OnXXXForScripts
        //{
        //    get; set;
        //}
        //private void NotifyXXXForScripts()
        //{
        //    this.RaiseScriptEvent(x => x.OnXXXForScripts);
        //}
        #endregion

        #region Exports
        //[Export(typeof(Action<DateTime>)), RunPeriodically(Interval = 1)]
        //public Action<DateTime> TimerElapsed => (dt =>
        //{
        //    foreach (var controller in Context.GetPlugin<WemosPlugin>().controllers)
        //        controller.ProcessTimer(dt);
        //});

        //[ScriptCommand("wemosSend")]
        //public ScriptCommand scriptSendCommand => (args =>
        //{
        //    var nodeID = int.Parse(args[0].ToString());
        //    var lineID = int.Parse(args[1].ToString());
        //    var messageType = int.Parse(args[2].ToString());
        //    var messageSubtype = int.Parse(args[3].ToString());
        //    var value = args[4].ToString();

        //    return Context.GetPlugin<WemosPlugin>().SendAsync(new WemosMessage(nodeID, lineID, (WemosMessageType)messageType, messageSubtype).Set(value));
        //});

        //[ScriptCommand("wemosSetLineValue")]
        //public ScriptCommand scriptSetLineValue => (args =>
        //{
        //    var nodeID = int.Parse(args[0].ToString());
        //    var lineID = int.Parse(args[1].ToString());
        //    var value = args[2].ToString();

        //    return Context.GetPlugin<WemosPlugin>().SetLineValueAsync(GetLine(nodeID, lineID), value);
        //});

        //[ScriptCommand("wemosDeleteLinesValues")]
        //public ScriptCommand scriptClearLinesValuesCommand => (args =>
        //{
        //    var lastDayToPreserve = int.Parse(args[0].ToString());
        //    //return Context.GetPlugin<WemosPlugin>().DeleteSensorValues(DateTime.Now.AddDays(-lastDayToPreserve));
        //    return null;
        //});
        #endregion

        #region Plugin overrides
        public override void InitDbModel()
        {
            var db = Context.StorageGet();

            //db.CreateTable<Device>();
            //db.CreateTable<Line>();
            //db.CreateTable<LineValue>();



            //db.CreateTable<WemosLineMonitor>();
            ////db.CreateTable<WemosLineController>();
            //db.CreateTable<WemosController>();
            ////db.CreateTable<WemosZone>();
        }
        public override void StartPlugin()
        {
            //StartValuesTask();
        }
        public override void StopPlugin()
        {
            //StopValuesTask();
        }
        #endregion

        #region Public methods
        public void RegisterDevice(Device device)
        {
            if (device != null)
            {
                var item = GetDevice(device.ID);

                if (item != null)
                {
                    item.Type = device.Type;
                    item.IPAddress = device.IPAddress;

                    Context.StorageSaveOrUpdate(item);
                }
                else
                    Context.StorageSave(device);
            }
        }
        public List<Device> GetDevices()
        {
            return Context.StorageGet().Table<Device>().ToList();
        }
        public Device GetDevice(string id)
        {
            return Context.StorageGet().Table<Device>().SingleOrDefault(d => d.ID == id);
        }

        public void RegisterLine(Line line)
        {
            if (line != null)
            {
                var item = GetLine(line.ID);

                if (item != null)
                {
                    item.DeviceID = line.DeviceID;
                    item.LineID = line.LineID;
                    item.Type = line.Type;

                    Context.StorageSaveOrUpdate(item);
                }
                else
                    Context.StorageSave(line);
            }
        }
        public List<Line> GetLines()
        {
            return Context.StorageGet().Table<Line>()
                .Select(l => { l.TimeStamp = l.TimeStamp.ToLocalTime(); return l; }) // time in DB is in UTC; convert to local
                .ToList();
        }
        public Line GetLine(string id)
        {
            return Context.StorageGet().Table<Line>()
                .Select(l => { l.TimeStamp = l.TimeStamp.ToLocalTime(); return l; })
                .FirstOrDefault(l => l.ID == id);
        }



        public List<LineValue> GetLineValues(string lineID, int count)
        {
            return Context.StorageGet().Table<LineValue>()
                .Where(v => v.LineID == lineID)// && v.TimeStamp > DateTime.Now.AddDays(-1))
                .OrderByDescending(v => v.TimeStamp)
                .Take(count)
                .OrderBy(v => v.TimeStamp)
                .Select(v => { v.TimeStamp = v.TimeStamp.ToLocalTime(); return v; }) // time in DB is in UTC; convert to local time
                .ToList();
        }
        public LineValue GetLineLastValue(string lineID)
        {
            return Context.StorageGet().Table<LineValue>()
                .Where(v => v.LineID == lineID)
                .OrderByDescending(v => v.TimeStamp)
                //.Take(1)
                .Select(v => { v.TimeStamp = v.TimeStamp.ToLocalTime(); return v; }) // time in DB is in UTC; convert to local time
                .FirstOrDefault();
        }

        public void UpdateLineValue(string lineID, float value)
        {
            var line = GetLine(lineID);

            if (line != null)
            {
                // save value:
                var lv = new LineValue()
                {
                    LineID = lineID,
                    TimeStamp = DateTime.UtcNow,
                    Value = line.Factor * value + line.Offset // tune value
                };
                Context.StorageSave(lv);

                // update line:
                line.TimeStamp = lv.TimeStamp;
                line.Value = lv.Value;
                Context.StorageSaveOrUpdate(line);

                // process:
                //Run(WemosMessageHandlers, method => method(message));

                //NotifyForSignalR(new { MsgId = "MySensorsTileContent", Data = BuildTileContent() }); // update MySensors tile
                //NotifyMessageReceivedForScripts(message);
                //NotifyForSignalR(new { MsgId = "SensorValue", Data = sv }); // notify Web UI
            }
        }

        public static bool IsValueFromLine(LineValue val, string lineID)
        {
            return val != null && lineID == val.LineID;
        }
        #endregion

        #region Private methods
        private void StartValuesTask()
        {
            if (!isTaskValuesActive)
            {
                ctsValues = new CancellationTokenSource();

                taskValues = Task.Factory.StartNew(async () =>
                {
                    while (!ctsValues.IsCancellationRequested)
                        if (isTaskValuesActive)
                        {
                            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                            {
                                //for (int i = 0; i < Context.GetPlugin<LinesPlugin>().oldControllers.Count; i++)
                                //    Context.GetPlugin<LinesPlugin>().oldControllers[i].ProcessTimer(DateTime.Now);

                                //var controllers = Context.GetPlugin<WemosPlugin>().controllers;
                                //for (int i = 0; i < controllers.Count; i++)
                                //    await controllers[i].ProcessAsync();
                            });

                            await Task.Delay(50);
                        }

                }, ctsValues.Token);

                isTaskValuesActive = true;
            }
        }
        private void StopValuesTask()
        {
            if (isTaskValuesActive)
            {
                ctsValues?.Cancel();
                isTaskValuesActive = false;
            }
        }
        #endregion

        #region Remote API

        #region Devices
        [ApiMethod("/api/things/devices")]
        public ApiMethod apiGetDevices => (args =>
        {
            return Context.GetPlugin<ThingsPlugin>().GetDevices();
        });

        [ApiMethod("/api/things/devices/update")]
        public ApiMethod apiUpdateDevice => (args =>
        {
            var item = JsonConvert.DeserializeObject<Device>(args[0].ToString());

            if (item != null)
            {
                Context.StorageSaveOrUpdate(item);
                //NotifyForSignalR(new { MsgId = "DeviceNameChanged", Data = new { Id = id, Name = name } });
                return true;
            }

            return false;
        });
        #endregion

        #region Lines
        [ApiMethod("/api/things/lines")]
        public ApiMethod apiGetLines => (args =>
        {
            return Context.GetPlugin<ThingsPlugin>().GetLines();
        });

        [ApiMethod("/api/things/lines/update")]
        public ApiMethod apiUpdateLine => (args =>
        {
            var item = JsonConvert.DeserializeObject<Line>(args[0].ToString());

            if (item != null)
            {
                Context.StorageSaveOrUpdate(item);
                //NotifyForSignalR(new { MsgId = "SensorNameChanged", Data = new { Id = id, Name = name } });
                return true;
            }

            return false;
        });

        [ApiMethod("/api/things/line/values")]
        public ApiMethod apiGetLineValues => (args =>
        {
            var id = args[0].ToString();
            var count = int.Parse(args[1].ToString());

            return Context.GetPlugin<ThingsPlugin>().GetLineValues(id, count);
        });
        #endregion





        #region Web UI
        [ApiMethod("/api/things/ui/settings")]
        public ApiMethod apiGetUISettings => (args =>
        {
            var stream = GetType().GetTypeInfo().Assembly.GetManifestResourceStream("SmartHub.UWP.Plugins.Things.UIWeb.Settings.html");
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        });
        #endregion

        #endregion
    }
}
