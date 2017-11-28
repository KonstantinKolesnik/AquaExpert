using SmartHub.UWP.Core.Communication.Stream;
using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.ApiListener;
using SmartHub.UWP.Plugins.ApiListener.Attributes;
using SmartHub.UWP.Plugins.Timer.Attributes;
using SmartHub.UWP.Plugins.UI.Attributes;
using SmartHub.UWP.Plugins.Wemos.Controllers;
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
        //private List<WemosControllerBase> controllers = new List<WemosControllerBase>();
        private List<WemosController> controllers = new List<WemosController>();


        private Task taskListen;
        private CancellationTokenSource ctsListen;
        private bool isListenActive = false;
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
        #endregion

        #region Exports
        [Export(typeof(Action<DateTime>)), RunPeriodically(Interval = 5)]
        public Action<DateTime> TimerElapsed => ((dt) =>
        {
            //foreach (var controller in Context.GetPlugin<WemosPlugin>().controllers)
            //    controller.ProcessTimer(dt);
        });
        #endregion

        #region Plugin ovverrides
        public override void InitDbModel()
        {
            using (var db = Context.StorageOpen())
            {
                db.CreateTable<WemosSetting>();
                db.CreateTable<WemosNode>();
                db.CreateTable<WemosLine>();
                db.CreateTable<WemosNodeBatteryValue>();
                db.CreateTable<WemosLineValue>();

                db.CreateTable<WemosMonitor>();
                db.CreateTable<WemosController>();
                //db.CreateTable<WemosZone>();
            }
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

            //foreach (var controller in GetControllers().Select(model => WemosControllerBase.FromModel(model)).Where(c => c != null))
            //{
            //    controllers.Add(controller);
            //    controller.Init(Context);
            //}
            foreach (var controller in GetControllers())
            {
                controller.Init(Context);
                controllers.Add(controller);
            }
        }
        public override async void StartPlugin()
        {
            await udpClient.Start(localService, remoteMulticastAddress);

            await RequestPresentation();

            foreach (var line in GetLines())
                await RequestLineValue(line);

            foreach (var controller in controllers)
                controller.Start();

            StartListen();
        }
        public override async void StopPlugin()
        {
            await udpClient.Stop();

            StopListen();
        }
        #endregion

        #region Public methods

        #region Send/Receive
        public async Task Send(WemosMessage data)
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

        public async Task RequestPresentation(int nodeID = -1, int lineID = -1)
        {
            await Send(new WemosMessage(nodeID, lineID, WemosMessageType.Presentation, 0));
        }
        public async Task RequestLineValue(WemosLine line)
        {
            if (line != null)
                await Send(new WemosMessage(line.NodeID, line.LineID, WemosMessageType.Get, (int) line.Type));
        }
        public async Task SetLineValue(WemosLine line, float value)
        {
            if (line != null)
                await Send(new WemosMessage(line.NodeID, line.LineID, WemosMessageType.Set, (int) line.Type).Set(value));
        }
        public async Task SetLineValue(WemosLine line, string value)
        {
            if (line != null)
                await Send(new WemosMessage(line.NodeID, line.LineID, WemosMessageType.Set, (int) line.Type).Set(value));
        }
        public async Task RebootNode(WemosNode node)
        {
            if (node != null)
                await Send(new WemosMessage(node.NodeID, -1, WemosMessageType.Internal, (int) WemosInternalMessageType.Reboot));
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
                case WemosLineType.Barometer: return "mm Hg";
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
            using (var db = Context.StorageOpen())
                return db.Table<WemosNode>().ToList();
        }
        public WemosNode GetNode(int nodeID)
        {
            using (var db = Context.StorageOpen())
            {
                //db.TraceListener = new DebugTraceListener(); // activate tracing
                return db.Table<WemosNode>().FirstOrDefault(n => n.NodeID == nodeID);
            }
        }

        public List<WemosLine> GetLines()
        {
            using (var db = Context.StorageOpen())
                return db.Table<WemosLine>()
                    .Select(l => { l.LastTimeStamp = l.LastTimeStamp.ToLocalTime(); return l; }) // time in DB is in UTC; convert to local
                    .ToList();
        }
        public WemosLine GetLine(int nodeID, int lineID)
        {
            using (var db = Context.StorageOpen())
                return db.Table<WemosLine>().FirstOrDefault(l => l.NodeID == nodeID && l.LineID == lineID);
        }
        public WemosLine GetLine(int id)
        {
            using (var db = Context.StorageOpen())
                return db.Table<WemosLine>().FirstOrDefault(l => l.ID == id);
        }
        public List<WemosLineValue> GetLineValues(int id, int count)
        {
            var line = GetLine(id);

            using (var db = Context.StorageOpen())
                return db.Table<WemosLineValue>()
                    .Where(v => v.NodeID == line.NodeID && v.LineID == line.LineID)// && v.TimeStamp > DateTime.Now.AddDays(-1))
                    .OrderByDescending(v => v.TimeStamp)
                    .Take(count)
                    .OrderBy(v => v.TimeStamp)
                    .Select(v => { v.TimeStamp = v.TimeStamp.ToLocalTime(); return v; }) // time in DB is in UTC; convert to local
                    .ToList();
        }

        public List<WemosMonitor> GetMonitors()
        {
            using (var db = Context.StorageOpen())
                return db.Table<WemosMonitor>().ToList();
        }
        public WemosMonitor GetMonitor(int id)
        {
            using (var db = Context.StorageOpen())
                return db.Table<WemosMonitor>().FirstOrDefault(m => m.ID == id);
        }

        public List<WemosController> GetControllers()
        {
            using (var db = Context.StorageOpen())
                return db.Table<WemosController>().ToList();
        }
        public WemosController GetController(int id)
        {
            //using (var db = Context.StorageOpen())
            //    return db.Table<WemosController>().FirstOrDefault(c => c.ID == id);

            return controllers.FirstOrDefault(c => c.ID == id);
        }
        //public WemosControllerBase GetControllerBase(int id)
        //{
        //    return controllers.FirstOrDefault(c => c.Model.ID == id);
        //}
        //public void AddController(WemosControllerBase ctrl)
        //{
        //    if (ctrl != null)
        //        controllers.Add(ctrl);
        //}
        public void AddController(WemosController ctrl)
        {
            if (ctrl != null)
                controllers.Add(ctrl);
        }
        //public void RemoveController(WemosControllerBase ctrl)
        //{
        //    if (ctrl != null)
        //        controllers.Remove(ctrl);
        //}
        public void RemoveController(WemosController ctrl)
        {
            if (ctrl != null)
                controllers.Remove(ctrl);
        }


        public WemosSetting GetSetting(string name)
        {
            using (var db = Context.StorageOpen())
                return db.Table<WemosSetting>().Where(setting => setting.Name == name).FirstOrDefault();
        }
        #endregion

        #region Private methods
        private void StartListen()
        {
            if (!isListenActive)
            {
                ctsListen = new CancellationTokenSource();

                taskListen = Task.Factory.StartNew(() =>
                {
                    while (!ctsListen.IsCancellationRequested)
                        if (isListenActive)
                        {
                            foreach (var controller in Context.GetPlugin<WemosPlugin>().controllers)
                                controller.ProcessTimer(DateTime.UtcNow);

                            //var str = await ReadAsync(ctsListen);
                            //if (!string.IsNullOrEmpty(str)) // event arrived
                            //    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { Received?.Invoke(this, new StringEventArgs(str)); });
                        }

                }, ctsListen.Token);

                isListenActive = true;
            }
        }
        private void StopListen()
        {
            if (isListenActive)
            {
                ctsListen?.Cancel();
                isListenActive = false;
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
                                    Type = (WemosLineType) message.SubType,
                                    ProtocolVersion = message.GetFloat()
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
                            Value = message.GetFloat() + line.Tune // tune value
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
                            await Send(new WemosMessage(message.NodeID, message.LineID, WemosMessageType.Internal, (int) WemosInternalMessageType.Time).Set(sec));
                            break;
                        case WemosInternalMessageType.Version:
                            if (node != null)
                            {
                                node.ProtocolVersion = message.GetFloat();
                                Context.StorageSaveOrUpdate(node);
                            }
                            break;
                        case WemosInternalMessageType.Config:
                            await Send(new WemosMessage(message.NodeID, -1, WemosMessageType.Internal, (int) WemosInternalMessageType.Config).Set(GetSetting("UnitSystem").Value));
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

        #region Nodes
        [ApiMethod(MethodName = "/api/wemos/nodes"), Export(typeof(ApiMethod))]
        public ApiMethod apiGetNodes => ((parameters) =>
        {
            return Context.GetPlugin<WemosPlugin>().GetNodes();
        });

        [ApiMethod(MethodName = "/api/wemos/nodes/setname"), Export(typeof(ApiMethod))]
        public ApiMethod apiSetNodeName => ((parameters) =>
        {
            var id = int.Parse(parameters[0].ToString());
            var name = parameters[1] as string;

            var item = Context.GetPlugin<WemosPlugin>().GetNode(id);
            item.Name = name;
            Context.StorageSaveOrUpdate(item);

            //NotifyForSignalR(new { MsgId = "NodeNameChanged", Data = new { Id = id, Name = name } });

            return true;
        });
        #endregion

        #region Lines
        [ApiMethod(MethodName = "/api/wemos/lines"), Export(typeof(ApiMethod))]
        public ApiMethod apiGetLines => ((parameters) =>
        {
            return Context.GetPlugin<WemosPlugin>().GetLines();
        });

        [ApiMethod(MethodName = "/api/wemos/lines/setname"), Export(typeof(ApiMethod))]
        public ApiMethod apiSetLineName => ((parameters) =>
        {
            var nodeID = int.Parse(parameters[0].ToString());
            var lineID = int.Parse(parameters[1].ToString());
            var name = parameters[2] as string;

            var item = Context.GetPlugin<WemosPlugin>().GetLine(nodeID, lineID);
            item.Name = name;
            Context.StorageSaveOrUpdate(item);

            //NotifyForSignalR(new { MsgId = "SensorNameChanged", Data = new { Id = id, Name = name } });

            return true;
        });

        [ApiMethod(MethodName = "/api/wemos/line/values"), Export(typeof(ApiMethod))]
        public ApiMethod apiGetLineValues => ((parameters) =>
        {
            var id = int.Parse(parameters[0].ToString());
            var count = int.Parse(parameters[1].ToString());

            return Context.GetPlugin<WemosPlugin>().GetLineValues(id, count);
        });
        #endregion

        #region Monitors
        [ApiMethod(MethodName = "/api/wemos/monitors"), Export(typeof(ApiMethod))]
        public ApiMethod apiGetMonitors => ((parameters) =>
        {
            return Context.GetPlugin<WemosPlugin>().GetMonitors().Select(m => new WemosMonitorDto(m) { LineName = GetLine(m.LineID).Name, LineType = GetLine(m.LineID).Type }).ToList();
        });

        [ApiMethod(MethodName = "/api/wemos/monitor"), Export(typeof(ApiMethod))]
        public ApiMethod apiGetMonitor => ((parameters) =>
        {
            var id = int.Parse(parameters[0].ToString());

            var model = Context.GetPlugin<WemosPlugin>().GetMonitor(id);
            var line = Context.GetPlugin<WemosPlugin>().GetLine(model.LineID);
            return new WemosMonitorDto(model) { LineName = line.Name, LineType = line.Type };
        });

        [ApiMethod(MethodName = "/api/wemos/monitors/add"), Export(typeof(ApiMethod))]
        public ApiMethod apiAddMonitor => ((parameters) =>
        {
            var name = parameters[0] as string;
            var lineID = int.Parse(parameters[1].ToString());

            var model = new WemosMonitor()
            {
                Name = name,
                LineID = lineID,
                Configuration = "{}"
            };

            Context.StorageSave(model);

            //NotifyForSignalR(new { MsgId = "MonitorAdded", Data = BuildMonitorWebModel(ctrl) });

            var line = Context.GetPlugin<WemosPlugin>().GetLine(model.LineID);
            return new WemosMonitorDto(model) { LineName = line.Name, LineType = line.Type };
        });

        [ApiMethod(MethodName = "/api/wemos/monitors/setnames"), Export(typeof(ApiMethod))]
        public ApiMethod apiSetMonitorNames => ((parameters) =>
        {
            var id = int.Parse(parameters[0].ToString());
            var name = parameters[1] as string;
            var nameForInformer = parameters[2] as string;

            var model = Context.GetPlugin<WemosPlugin>().GetMonitor(id);
            if (model != null)
            {
                model.Name = name;
                model.NameForInformer = nameForInformer;
                Context.StorageSaveOrUpdate(model);

                return true;
            }

            return false;
        });

        [ApiMethod(MethodName = "/api/wemos/monitors/setconfig"), Export(typeof(ApiMethod))]
        public ApiMethod apiSetMonitorConfiguration => ((parameters) =>
        {
            var id = int.Parse(parameters[0].ToString());
            var config = parameters[1].ToString();

            var model = Context.GetPlugin<WemosPlugin>().GetMonitor(id);
            if (model != null)
            {
                model.SerializeConfiguration(config);
                Context.StorageSaveOrUpdate(model);

                return true;
            }

            return false;
        });

        [ApiMethod(MethodName = "/api/wemos/monitors/delete"), Export(typeof(ApiMethod))]
        public ApiMethod apiDeleteMonitor => ((parameters) =>
        {
            var id = int.Parse(parameters[0].ToString());

            Context.StorageDelete(GetMonitor(id));

            return true;
        });
        #endregion

        #region Controllers
        [ApiMethod(MethodName = "/api/wemos/controllers"), Export(typeof(ApiMethod))]
        public ApiMethod apiGetControllers => ((parameters) =>
        {
            return Context.GetPlugin<WemosPlugin>().GetControllers();
        });

        [ApiMethod(MethodName = "/api/wemos/controllers/add"), Export(typeof(ApiMethod))]
        public ApiMethod apiAddController => ((parameters) =>
        {
            var name = parameters[0] as string;
            var type = (WemosControllerType)int.Parse(parameters[1].ToString());

            //var model = new WemosController()
            //{
            //    Name = name,
            //    Type = type,
            //    IsAutoMode = false
            //};
            //var ctrl = WemosControllerBase.FromModel(model);
            ////var ctrl = WemosControllerBase.Create(type, name);
            //if (ctrl != null)
            //{
            //    Context.StorageSave(ctrl.Model);
            //    Context.GetPlugin<WemosPlugin>().AddController(ctrl);
            //    ctrl.Init(Context);
            //    ctrl.Start();
            //    //NotifyForSignalR(new { MsgId = "ControllerAdded", Data = BuildControllerWebModel(ctrl) });
            //    return ctrl.Model;
            //}


            var ctrl = new WemosController()
            {
                Name = name,
                Type = type,
                IsAutoMode = false
            };
            Context.StorageSave(ctrl);
            Context.GetPlugin<WemosPlugin>().AddController(ctrl);
            ctrl.Init(Context);
            ctrl.Start();
            //NotifyForSignalR(new { MsgId = "ControllerAdded", Data = BuildControllerWebModel(ctrl) });

            return ctrl;
        });

        [ApiMethod(MethodName = "/api/wemos/controllers/setname"), Export(typeof(ApiMethod))]
        public ApiMethod apiSetControllerName => ((parameters) =>
        {
            var id = int.Parse(parameters[0].ToString());
            var name = parameters[1] as string;

            //var ctrl = Context.GetPlugin<WemosPlugin>().GetControllerBase(id);
            //if (ctrl != null)
            //{
            //    ctrl.Model.Name = name;
            //    Context.StorageSaveOrUpdate(ctrl.Model);
            //    return true;
            //}

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
        public ApiMethod apiSetControllerAutoMode => ((parameters) =>
        {
            var id = int.Parse(parameters[0].ToString());
            var isAutoMode = (bool)parameters[1];

            //var ctrl = Context.GetPlugin<WemosPlugin>().GetControllerBase(id);
            //if (ctrl != null)
            //{
            //    ctrl.Model.IsAutoMode = isAutoMode;
            //    Context.StorageSaveOrUpdate(ctrl.Model);
            //    return true;
            //}

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
        public ApiMethod apiSetControllerConfiguration => ((parameters) =>
        {
            var id = int.Parse(parameters[0].ToString());
            var config = parameters[1].ToString();

            //var ctrl = Context.GetPlugin<WemosPlugin>().GetControllerBase(id);
            //if (ctrl != null)
            //{
            //    ctrl.Model.Configuration = config;
            //    Context.StorageSaveOrUpdate(ctrl.Model);
            //    return true;
            //}

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
        public ApiMethod apiDeleteController => ((parameters) =>
        {
            var id = int.Parse(parameters[0].ToString());

            //var model = Context.GetPlugin<WemosPlugin>().GetController(id);
            //if (model != null)
            //{
            //    Context.StorageDelete(model);
            //    var ctrl = Context.GetPlugin<WemosPlugin>().GetControllerBase(id);
            //    Context.GetPlugin<WemosPlugin>().RemoveController(ctrl);
            //    return true;
            //}

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
