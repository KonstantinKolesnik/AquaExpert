using Newtonsoft.Json;
using SmartHub.UWP.Core.Communication.Stream;
using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.ApiListener;
using SmartHub.UWP.Plugins.ApiListener.Attributes;
using SmartHub.UWP.Plugins.Scripts;
using SmartHub.UWP.Plugins.Scripts.Attributes;
using SmartHub.UWP.Plugins.UI.Attributes;
using SmartHub.UWP.Plugins.Wemos.Controllers.Models;
using SmartHub.UWP.Plugins.Wemos.Core.Messages;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using SmartHub.UWP.Plugins.Wemos.Monitors;
using SmartHub.UWP.Plugins.Wemos.Monitors.Models;
using SmartHub.UWP.Plugins.Wemos.UI;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Networking;
using Windows.UI.Core;

namespace SmartHub.UWP.Plugins.Wemos
{
    [Plugin]
    [AppSectionItem("Wemos", AppSectionType.Applications, typeof(MainPage), "Wemos modules")]
    [AppSectionItem("Wemos", AppSectionType.System, typeof(SettingsPage), "Wemos modules")]
    public class WemosPlugin : PluginBase
    {
        #region Fields
        private const string localService = "11111";
        private const string remoteService = "22222";
        private const string remoteMulticastAddress = "224.3.0.5";
        //private const string broadcastAddress = "255.255.255.255";
        private UdpClient udpClient = new UdpClient();

        private List<WemosController> controllers = new List<WemosController>();
        private Task taskControllers;
        private CancellationTokenSource ctsControllers;
        private bool isTaskControllersActive = false;
        #endregion

        #region Imports
        [ImportMany]
        public Action<WemosMessage>[] WemosMessageHandlers
        {
            get; set;
        }
        private void NotifyMessageReceivedForPlugins(WemosMessage msg)
        {
            Run(WemosMessageHandlers, x => x(msg));
        }




        //[ScriptEvent("mySensors.connected")]
        //public ScriptEventHandlerDelegate[] OnConnectedForScripts
        //{
        //    get; set;
        //}
        //private void NotifyConnectedForScripts()
        //{
        //    this.RaiseScriptEvent(x => x.OnConnectedForScripts);
        //}
        #endregion

        #region Exports
        //[Export(typeof(Action<DateTime>)), RunPeriodically(Interval = 1)]
        //public Action<DateTime> TimerElapsed => ((dt) =>
        //{
        //    foreach (var controller in Context.GetPlugin<WemosPlugin>().controllers)
        //        controller.ProcessTimer(dt);
        //});

        [ScriptCommand(MethodName = "wemosSendCommand"), Export(typeof(ScriptCommand))]
        public ScriptCommand scriptSendCommand => ((args) =>
        {
            var nodeID = int.Parse(args[0].ToString());
            var lineID = int.Parse(args[1].ToString());
            var messageType = int.Parse(args[2].ToString());
            var messageSubtype = int.Parse(args[3].ToString());
            //var value = float.Parse(args[4].ToString());
            var value = args[4].ToString();

            return Context.GetPlugin<WemosPlugin>().SendAsync(new WemosMessage(nodeID, lineID, (WemosMessageType)messageType, messageSubtype).Set(value));
        });

        //[ScriptCommand(MethodName = "wemosClearLinesValuesCommand"), Export(typeof(ScriptCommand))]
        //public ScriptCommand scriptClearLinesValuesCommand => ((args) =>
        //{
        //    var lastDayToPreserve = int.Parse(args[0].ToString());

        //    return Context.GetPlugin<WemosPlugin>().DeleteSensorValues(DateTime.Now.AddDays(-lastDayToPreserve));
        //});
        #endregion

        #region Plugin ovverrides
        public override void InitDbModel()
        {
            var db = Context.StorageGet();

            db.CreateTable<WemosSetting>();
            db.CreateTable<WemosNode>();
            db.CreateTable<WemosLine>();
            db.CreateTable<WemosNodeBatteryValue>();
            db.CreateTable<WemosLineValue>();

            db.CreateTable<WemosMonitor>();
            db.CreateTable<WemosController>();
            //db.CreateTable<WemosZone>();
        }
        public override void InitPlugin()
        {
            if (GetSetting("UnitSystem") == null)
                Context.StorageSave(new WemosSetting() { Name = "UnitSystem", Value = "M" });

            udpClient.MessageReceived += async (sender, e) =>
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    foreach (var msg in WemosMessage.FromDto(e.Data))
                        await ProcessMessage(msg, e.RemoteAddress);
                });
            };

            foreach (var controller in GetControllers())
            {
                controller.Init(Context);
                controllers.Add(controller);
            }
        }
        public override async void StartPlugin()
        {
            await udpClient.Start(localService, remoteMulticastAddress);

            await RequestPresentationAsync();

            foreach (var line in GetLines())
                await RequestLineValueAsync(line);

            foreach (var controller in controllers)
                controller.Start();

            StartControllersTask();
        }
        public override async void StopPlugin()
        {
            await udpClient.Stop();

            StopControllersTask();
        }
        #endregion

        #region Public methods

        #region Send/Receive
        public async Task SendAsync(WemosMessage data)
        {
            try
            {
                if (data != null)
                    await udpClient.Send(remoteMulticastAddress, remoteService, data.ToDto());
            }
            catch (Exception ex)
            {
            }
        }

        public async Task RequestPresentationAsync(int nodeID = -1, int lineID = -1)
        {
            await SendAsync(new WemosMessage(nodeID, lineID, WemosMessageType.Presentation, 0));
        }
        public async Task RequestLineValueAsync(WemosLine line)
        {
            if (line != null)
                await SendAsync(new WemosMessage(line.NodeID, line.LineID, WemosMessageType.Get, (int) line.Type));
        }
        public async Task SetLineValueAsync(WemosLine line, float value)
        {
            if (line != null)
                await SendAsync(new WemosMessage(line.NodeID, line.LineID, WemosMessageType.Set, (int) line.Type).Set(value));
        }
        public async Task SetLineValueAsync(WemosLine line, string value)
        {
            if (line != null)
                await SendAsync(new WemosMessage(line.NodeID, line.LineID, WemosMessageType.Set, (int) line.Type).Set(value));
        }
        public async Task RebootNodeAsync(WemosNode node)
        {
            if (node != null)
                await SendAsync(new WemosMessage(node.NodeID, -1, WemosMessageType.Internal, (int) WemosInternalMessageType.Reboot));
        }
        #endregion

        public static bool IsMessageFromLine(WemosMessage msg, WemosLine line)
        {
            return msg != null && line != null && line.NodeID == msg.NodeID && line.LineID == msg.LineID;
        }
        public static bool IsValueFromLine(WemosLineValue val, WemosLine line)
        {
            return val != null && line != null && line.NodeID == val.NodeID && line.LineID == val.LineID;
        }
        public static string LineTypeToUnits(WemosLineType lt)
        {
            switch (lt)
            {
                case WemosLineType.Switch: return "";
                case WemosLineType.Temperature: return "°C";
                case WemosLineType.Humidity: return "%";
                case WemosLineType.Barometer: return "Pa"; // mm Hg
                case WemosLineType.Weight: return "kg";
                case WemosLineType.Voltage: return "V";
                case WemosLineType.Current: return "A";
                case WemosLineType.Power: return "Wt";
                case WemosLineType.Rain: return "";
                case WemosLineType.UV: return "";
                case WemosLineType.Distance: return "m";
                case WemosLineType.LightLevel: return "lux";
                case WemosLineType.IR: return "";
                case WemosLineType.AirQuality: return "";
                case WemosLineType.Vibration: return "";
                case WemosLineType.Ph: return "";
                case WemosLineType.ORP: return "";

                default: return "";
            }
        }

        public List<WemosNode> GetNodes()
        {
            return Context.StorageGet().Table<WemosNode>().ToList();
        }
        public WemosNode GetNode(int nodeID)
        {
            //Context.StorageOpen().TraceListener = new DebugTraceListener(); // activate tracing
            return Context.StorageGet().Table<WemosNode>().FirstOrDefault(n => n.NodeID == nodeID);
        }

        public List<WemosLine> GetLines()
        {
            return Context.StorageGet().Table<WemosLine>()
                .Select(l => { l.LastTimeStamp = l.LastTimeStamp.ToLocalTime(); return l; }) // time in DB is in UTC; convert to local
                .ToList();
        }
        public WemosLine GetLine(int nodeID, int lineID)
        {
            return Context.StorageGet().Table<WemosLine>().FirstOrDefault(l => l.NodeID == nodeID && l.LineID == lineID);
        }
        public WemosLine GetLine(string id)
        {
            return Context.StorageGet().Table<WemosLine>().FirstOrDefault(l => l.ID == id);
        }
        public List<WemosLineValue> GetLineValues(string id, int count)
        {
            var line = GetLine(id);

            return Context.StorageGet().Table<WemosLineValue>()
                .Where(v => v.NodeID == line.NodeID && v.LineID == line.LineID)// && v.TimeStamp > DateTime.Now.AddDays(-1))
                .OrderByDescending(v => v.TimeStamp)
                .Take(count)
                .OrderBy(v => v.TimeStamp)
                .Select(v => { v.TimeStamp = v.TimeStamp.ToLocalTime(); return v; }) // time in DB is in UTC; convert to local
                .ToList();
        }

        public List<WemosMonitor> GetMonitors()
        {
            return Context.StorageGet().Table<WemosMonitor>().ToList();
        }
        public WemosMonitor GetMonitor(string id)
        {
            return Context.StorageGet().Table<WemosMonitor>().FirstOrDefault(m => m.ID == id);
        }

        public List<WemosController> GetControllers()
        {
            return Context.StorageGet().Table<WemosController>().ToList();
        }
        public WemosController GetController(int id)
        {
            //return Context.StorageOpen().Table<WemosController>().FirstOrDefault(c => c.ID == id);
            return controllers.FirstOrDefault(c => c.ID == id);
        }
        public void AddController(WemosController ctrl)
        {
            if (ctrl != null)
                controllers.Add(ctrl);
        }
        public void RemoveController(WemosController ctrl)
        {
            if (ctrl != null)
                controllers.Remove(ctrl);
        }

        public WemosSetting GetSetting(string name)
        {
            return Context.StorageGet().Table<WemosSetting>().Where(setting => setting.Name == name).FirstOrDefault();
        }
        #endregion

        #region Private methods
        private void StartControllersTask()
        {
            if (!isTaskControllersActive)
            {
                ctsControllers = new CancellationTokenSource();

                taskControllers = Task.Factory.StartNew(async () =>
                {
                    while (!ctsControllers.IsCancellationRequested)
                        if (isTaskControllersActive)
                        {
                            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                for (int i = 0; i < Context.GetPlugin<WemosPlugin>().controllers.Count; i++)
                                    Context.GetPlugin<WemosPlugin>().controllers[i].ProcessTimer(DateTime.Now);
                            });

                            await Task.Delay(50);
                        }

                }, ctsControllers.Token);

                isTaskControllersActive = true;
            }
        }
        private void StopControllersTask()
        {
            if (isTaskControllersActive)
            {
                ctsControllers?.Cancel();
                isTaskControllersActive = false;
            }
        }

        private async Task ProcessMessage(WemosMessage message, HostName remoteAddress)
        {
            if (message == null)
                return;

            var node = GetNode(message.NodeID);
            var line = GetLine(message.NodeID, message.LineID);

            switch (message.Type)
            {
                #region Presentation
                case WemosMessageType.Presentation: // sent by a nodes when they present attached sensors.
                    if (message.LineID == -1) // node
                    {
                        if (node == null)
                        {
                            node = new WemosNode
                            {
                                NodeID = message.NodeID,
                                Name = $"Node {message.NodeID}",
                                Type = (WemosLineType) message.SubType,
                                ProtocolVersion = message.GetFloat(),
                                IPAddress = remoteAddress.CanonicalName
                            };
                            Context.StorageSave(node);
                        }
                        else
                        {
                            node.Type = (WemosLineType) message.SubType;
                            node.ProtocolVersion = message.GetFloat();
                            node.IPAddress = remoteAddress.CanonicalName;
                            Context.StorageSaveOrUpdate(node);
                        }

                        //NotifyMessageReceivedForPlugins(message);
                        //NotifyMessageReceivedForScripts(message);
                        //NotifyForSignalR(new { MsgId = "NodePresentation", Data = BuildNodeRichWebModel(node) });
                    }
                    else // line
                    {
                        if (node != null)
                        {
                            if (line == null)
                            {
                                line = new WemosLine()
                                {
                                    NodeID = node.NodeID,
                                    LineID = message.LineID,
                                    Name = $"Line {message.NodeID}-{message.LineID}",
                                    Type = (WemosLineType)message.SubType,
                                    ProtocolVersion = message.GetFloat(),
                                    Factor = 1,
                                    Tune = 0
                                };
                                Context.StorageSave(line);
                            }
                            else
                            {
                                line.Type = (WemosLineType) message.SubType;
                                line.ProtocolVersion = message.GetFloat();
                                Context.StorageSaveOrUpdate(line);
                            }

                            //NotifyMessageReceivedForPlugins(message);
                            //NotifyMessageReceivedForScripts(message);
                            //NotifyForSignalR(new { MsgId = "SensorPresentation", Data = BuildSensorRichWebModel(line) });
                        }
                    }
                    break;
                #endregion

                #region Report
                case WemosMessageType.Report:
                    if (line != null)
                    {
                        // save value:
                        var lv = new WemosLineValue()
                        {
                            NodeID = message.NodeID,
                            LineID = message.LineID,
                            TimeStamp = DateTime.UtcNow,
                            Type = (WemosLineType) message.SubType,
                            Value = line.Factor * message.GetFloat() + line.Tune // tune value
                        };
                        Context.StorageSave(lv);

                        // update line:
                        line.LastTimeStamp = lv.TimeStamp;
                        line.LastValue = lv.Value;
                        Context.StorageSaveOrUpdate(line);

                        // process:
                        //NotifyForSignalR(new { MsgId = "MySensorsTileContent", Data = BuildTileContent() }); // update MySensors tile
                        NotifyMessageReceivedForPlugins(message);
                        //NotifyMessageReceivedForScripts(message);
                        //NotifyForSignalR(new { MsgId = "SensorValue", Data = sv }); // notify Web UI

                        foreach (var controller in controllers)
                            controller.ProcessMessage(lv);
                    }
                    break;
                #endregion

                #region Set
                case WemosMessageType.Set: // sent to a sensor when a sensor value should be updated
                    break;
                #endregion

                #region Request
                case WemosMessageType.Get: // requests a variable value (usually from an actuator destined for controller)
                    break;
                #endregion

                #region Internal
                case WemosMessageType.Internal:
                    var imt = (WemosInternalMessageType) message.SubType;
                    switch (imt)
                    {
                        case WemosInternalMessageType.BatteryLevel: // int, in %
                            if (node != null)
                            {
                                WemosNodeBatteryValue bl = new WemosNodeBatteryValue()
                                {
                                    NodeID = message.NodeID,
                                    TimeStamp = DateTime.Now,
                                    Value = (int) message.GetInteger()
                                };
                                Context.StorageSave(bl);

                                node.LastTimeStamp = bl.TimeStamp;
                                node.LastBatteryValue = bl.Value;
                                node.IPAddress = remoteAddress.CanonicalName;
                                Context.StorageSaveOrUpdate(node);

                                //NotifyMessageReceivedForPlugins(message);
                                //NotifyMessageReceivedForScripts(message);
                                //NotifyForSignalR(new { MsgId = "BatteryValue", Data = bl });
                            }
                            break;
                        case WemosInternalMessageType.Time:
                            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                            var sec = Convert.ToInt64(DateTime.Now.Subtract(unixEpoch).TotalSeconds); // seconds since 1970
                            await SendAsync(new WemosMessage(message.NodeID, message.LineID, WemosMessageType.Internal, (int) WemosInternalMessageType.Time).Set(sec));
                            break;
                        case WemosInternalMessageType.Version:
                            if (node != null)
                            {
                                node.ProtocolVersion = message.GetFloat();
                                Context.StorageSaveOrUpdate(node);
                            }
                            break;
                        case WemosInternalMessageType.Config:
                            await SendAsync(new WemosMessage(message.NodeID, -1, WemosMessageType.Internal, (int) WemosInternalMessageType.Config).Set(GetSetting("UnitSystem").Value));
                            break;
                        case WemosInternalMessageType.FirmwareName:
                        case WemosInternalMessageType.FirmwareVersion:
                            if (node != null)
                            {
                                if (imt == WemosInternalMessageType.FirmwareName)
                                    node.FirmwareName = message.GetString();
                                else
                                    node.FirmwareVersion = message.GetFloat();

                                Context.StorageSaveOrUpdate(node);

                                //NotifyMessageReceivedForPlugins(message);
                                //NotifyMessageReceivedForScripts(message);
                                //NotifyForSignalR(new { MsgId = "NodePresentation", Data = BuildNodeRichWebModel(node) });
                            }
                            break;
                    }
                    break;
                    #endregion

                #region Stream
                //case WemosMessageType.Stream: //used for OTA firmware updates
                //    switch ((StreamValueType) message.SubType)
                //    {
                //        case StreamValueType.FirmwareConfigRequest:
                //            var fwtype = pullWord(payload, 0);
                //            var fwversion = pullWord(payload, 2);
                //            sendFirmwareConfigResponse(sender, fwtype, fwversion, db, gw);
                //            break;
                //        case StreamValueType.FirmwareConfigResponse:
                //            break;
                //        case StreamValueType.FirmwareRequest:
                //            break;
                //        case StreamValueType.FirmwareResponse:
                //            var fwtype = pullWord(payload, 0);
                //            var fwversion = pullWord(payload, 2);
                //            var fwblock = pullWord(payload, 4);
                //            sendFirmwareResponse(sender, fwtype, fwversion, fwblock, db, gw);
                //            break;
                //        case StreamValueType.Sound:
                //            break;
                //        case StreamValueType.Image:
                //            break;
                //    }
                //    break;
                #endregion
            }
        }
        #endregion

        #region Remote API

        [ApiMethod(MethodName = "/api/wemos/presentation"), Export(typeof(ApiMethod))]
        public ApiMethod apiPresentation => ((args) =>
        {
            return Context.GetPlugin<WemosPlugin>().RequestPresentationAsync();
        });

        #region Nodes
        [ApiMethod(MethodName = "/api/wemos/nodes"), Export(typeof(ApiMethod))]
        public ApiMethod apiGetNodes => ((args) =>
        {
            return Context.GetPlugin<WemosPlugin>().GetNodes();
        });

        [ApiMethod(MethodName = "/api/wemos/nodes/update"), Export(typeof(ApiMethod))]
        public ApiMethod apiUpdateNode => ((args) =>
        {
            var item = JsonConvert.DeserializeObject<WemosNode>(args[0].ToString());
            Context.StorageSaveOrUpdate(item);
            //NotifyForSignalR(new { MsgId = "NodeNameChanged", Data = new { Id = id, Name = name } });

            return true;
        });
        #endregion

        #region Lines
        [ApiMethod(MethodName = "/api/wemos/lines"), Export(typeof(ApiMethod))]
        public ApiMethod apiGetLines => ((args) =>
        {
            return Context.GetPlugin<WemosPlugin>().GetLines();
        });

        [ApiMethod(MethodName = "/api/wemos/lines/update"), Export(typeof(ApiMethod))]
        public ApiMethod apiUpdateLine => ((args) =>
        {
            var item = JsonConvert.DeserializeObject<WemosLine>(args[0].ToString());
            Context.StorageSaveOrUpdate(item);
            //NotifyForSignalR(new { MsgId = "SensorNameChanged", Data = new { Id = id, Name = name } });

            return true;
        });

        [ApiMethod(MethodName = "/api/wemos/line/values"), Export(typeof(ApiMethod))]
        public ApiMethod apiGetLineValues => ((args) =>
        {
            var id = args[0].ToString();
            var count = int.Parse(args[1].ToString());

            return Context.GetPlugin<WemosPlugin>().GetLineValues(id, count);
        });
        #endregion

        #region Monitors
        [ApiMethod(MethodName = "/api/wemos/monitors"), Export(typeof(ApiMethod))]
        public ApiMethod apiGetMonitors => ((args) =>
        {
            return Context.GetPlugin<WemosPlugin>().GetMonitors().Select(m => new WemosMonitorDto(m)
            {
                LineName = Context.GetPlugin<WemosPlugin>().GetLine(m.LineID).Name,
                LineType = Context.GetPlugin<WemosPlugin>().GetLine(m.LineID).Type
            }).ToList();
        });

        [ApiMethod(MethodName = "/api/wemos/monitor"), Export(typeof(ApiMethod))]
        public ApiMethod apiGetMonitor => ((args) =>
        {
            var id = args[0].ToString();

            var model = Context.GetPlugin<WemosPlugin>().GetMonitor(id);
            var line = Context.GetPlugin<WemosPlugin>().GetLine(model.LineID);

            return new WemosMonitorDto(model) { LineName = line.Name, LineType = line.Type };
        });

        [ApiMethod(MethodName = "/api/wemos/monitors/add"), Export(typeof(ApiMethod))]
        public ApiMethod apiAddMonitor => ((args) =>
        {
            var lineID = args[0].ToString();
            var min = float.Parse(args[1].ToString());
            var max = float.Parse(args[2].ToString());

            var model = new WemosMonitor()
            {
                LineID = lineID,
                Min = min,
                Max = max
                //Configuration = "{}"
            };

            Context.StorageSave(model);

            //NotifyForSignalR(new { MsgId = "MonitorAdded", Data = BuildMonitorWebModel(ctrl) });

            var line = Context.GetPlugin<WemosPlugin>().GetLine(model.LineID);
            return new WemosMonitorDto(model) { LineName = line.Name, LineType = line.Type };
        });

        [ApiMethod(MethodName = "/api/wemos/monitors/update"), Export(typeof(ApiMethod))]
        public ApiMethod apiUpdateMonitor => ((args) =>
        {
            var item = JsonConvert.DeserializeObject<WemosMonitor>(args[0].ToString());

            if (item != null)
            {
                Context.StorageSaveOrUpdate(item);
                return true;
            }

            return false;
        });

        [ApiMethod(MethodName = "/api/wemos/monitors/delete"), Export(typeof(ApiMethod))]
        public ApiMethod apiDeleteMonitor => ((args) =>
        {
            var id = args[0].ToString();

            Context.StorageDelete(Context.GetPlugin<WemosPlugin>().GetMonitor(id));

            return true;
        });
        #endregion

        #region Controllers
        [ApiMethod(MethodName = "/api/wemos/controllers"), Export(typeof(ApiMethod))]
        public ApiMethod apiGetControllers => ((args) =>
        {
            return Context.GetPlugin<WemosPlugin>().GetControllers();
        });

        [ApiMethod(MethodName = "/api/wemos/controllers/add"), Export(typeof(ApiMethod))]
        public ApiMethod apiAddController => ((args) =>
        {
            var name = args[0] as string;
            var type = (WemosControllerType)int.Parse(args[1].ToString());

            var ctrl = new WemosController()
            {
                Name = name,
                Type = type,
                IsAutoMode = false,
                Configuration = null
            };
            ctrl.Init(Context);

            Context.StorageSave(ctrl);
            Context.GetPlugin<WemosPlugin>().AddController(ctrl);

            ctrl.Start();
            //NotifyForSignalR(new { MsgId = "ControllerAdded", Data = BuildControllerWebModel(ctrl) });

            return ctrl;
        });

        [ApiMethod(MethodName = "/api/wemos/controllers/setname"), Export(typeof(ApiMethod))]
        public ApiMethod apiSetControllerName => ((args) =>
        {
            var id = int.Parse(args[0].ToString());
            var name = args[1] as string;

            var ctrl = Context.GetPlugin<WemosPlugin>().GetController(id);
            if (ctrl != null)
            {
                ctrl.Name = name;
                Context.StorageSaveOrUpdate(ctrl);
                return true;
            }

            return false;
        });

        [ApiMethod(MethodName = "/api/wemos/controllers/setautomode"), Export(typeof(ApiMethod))]
        public ApiMethod apiSetControllerAutoMode => ((args) =>
        {
            var id = int.Parse(args[0].ToString());
            var isAutoMode = (bool)args[1];

            var ctrl = Context.GetPlugin<WemosPlugin>().GetController(id);
            if (ctrl != null)
            {
                ctrl.IsAutoMode = isAutoMode;
                Context.StorageSaveOrUpdate(ctrl);
                return true;
            }

            return false;
        });

        [ApiMethod(MethodName = "/api/wemos/controllers/setconfig"), Export(typeof(ApiMethod))]
        public ApiMethod apiSetControllerConfiguration => ((args) =>
        {
            var id = int.Parse(args[0].ToString());
            var config = args[1].ToString();

            var ctrl = Context.GetPlugin<WemosPlugin>().GetController(id);
            if (ctrl != null)
            {
                ctrl.Configuration = config;
                Context.StorageSaveOrUpdate(ctrl);
                return true;
            }

            return false;
        });

        [ApiMethod(MethodName = "/api/wemos/controllers/delete"), Export(typeof(ApiMethod))]
        public ApiMethod apiDeleteController => ((args) =>
        {
            var id = int.Parse(args[0].ToString());

            var ctrl = Context.GetPlugin<WemosPlugin>().GetController(id);
            if (ctrl != null)
            {
                Context.StorageDelete(ctrl);
                Context.GetPlugin<WemosPlugin>().RemoveController(ctrl);
                return true;
            }

            return false;
        });
        #endregion

        #endregion
    }
}
