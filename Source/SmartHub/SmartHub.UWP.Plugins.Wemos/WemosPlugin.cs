using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.ApiListener;
using SmartHub.UWP.Plugins.ApiListener.Attributes;
using SmartHub.UWP.Plugins.UI.Attributes;
using SmartHub.UWP.Plugins.Wemos.Core;
using SmartHub.UWP.Plugins.Wemos.Models;
using SmartHub.UWP.Plugins.Wemos.Transporting;
using SmartHub.UWP.Plugins.Wemos.UI;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Windows.Networking;

namespace SmartHub.UWP.Plugins.Wemos
{
    [Plugin]
    [AppSectionItem("Wemos", AppSectionType.Applications, typeof(Main), "Wemos modules (D1 Mini, D1, ESP8266) management")]
    [AppSectionItem("Wemos", AppSectionType.System, typeof(Settings), "Wemos modules (D1 Mini, D1, ESP8266) network")]
    public class WemosPlugin : PluginBase
    {
        #region Fields
        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private WemosTransport transport = new WemosTransport();
        #endregion

        //public event WemosMessageEventHandler MessageReceived;

        #region Imports
        [ImportMany]
        public Action<WemosMessage>[] WemosMessageHandlers { get; set; }
        private void NotifyMessageReceivedForPlugins(WemosMessage msg)
        {
            Run(WemosMessageHandlers, x => x(msg));
        }
        #endregion

        #region Plugin ovverrides
        public override void InitDbModel()
        {
            using (var db = Context.OpenConnection())
            {
                db.CreateTable<WemosSetting>();
                db.CreateTable<WemosNode>();
                db.CreateTable<WemosLine>();
                db.CreateTable<WemosNodeBatteryValue>();
                db.CreateTable<WemosLineValue>();

                db.CreateTable<WemosMonitor>();
            }
        }
        public override void InitPlugin()
        {
            if (GetSetting("UnitSystem") == null)
                Save(new WemosSetting() { Name = "UnitSystem", Value = "M" });

            transport.MessageReceived += OnMessageReceived;
        }
        public override async void StartPlugin()
        {
            await transport.Open();
            await RequestPresentation();
        }
        public override void StopPlugin()
        {
            transport.Close();
        }
        #endregion

        #region API
        public async Task Send(WemosMessage data, bool isBrodcast = false)
        {
            await transport.Send(data, isBrodcast);
        }
        public async Task RequestPresentation(int nodeID = -1, int lineID = -1)
        {
            await Send(new WemosMessage(nodeID, lineID, WemosMessageType.Presentation, 0), true);
        }
        public async Task RequestLineValue(WemosLine line)
        {
            if (line != null)
                await Send(new WemosMessage(line.NodeID, line.LineID, WemosMessageType.Get, (int) line.Type));
        }
        //public async Task SetLineValue(WemosLine line, float value)
        //{
        //    if (line != null)
        //    {
        //        var lastSV = GetLastSensorValue(line);
        //        if (lastSV == null || (lastSV.Value != value))
        //            await Send(new WemosMessage(line.NodeID, line.LineID, WemosMessageType.Set, (int) line.Type).Set(value));
        //    }
        //}
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

        public static bool IsMessageFromLine(WemosMessage msg, WemosLine line)
        {
            return msg != null && line != null && line.NodeID == msg.NodeID && line.LineID == msg.LineID;
        }

        public List<WemosNode> GetNodes()
        {
            using (var db = Context.OpenConnection())
                return db.Table<WemosNode>().ToList();
        }
        public List<WemosLine> GetLines()
        {
            using (var db = Context.OpenConnection())
                return db.Table<WemosLine>().ToList();
        }
        public WemosNode GetNode(int nodeID)
        {
            using (var db = Context.OpenConnection())
            {
                //db.TraceListener = new DebugTraceListener(); // activate tracing
                return db.Table<WemosNode>().Where(n => n.NodeID == nodeID).FirstOrDefault();
            }
        }
        public WemosLine GetLine(int nodeID, int lineID)
        {
            using (var db = Context.OpenConnection())
                return db.Table<WemosLine>().Where(l => l.NodeID == nodeID && l.LineID == lineID).FirstOrDefault();
        }
        public WemosLine GetLine(int id)
        {
            using (var db = Context.OpenConnection())
                return db.Table<WemosLine>().Where(l => l.ID == id).FirstOrDefault();
        }
        public List<WemosLineValue> GetLineValues(int id, int count)
        {
            var line = GetLine(id);

            using (var db = Context.OpenConnection())
                return db.Table<WemosLineValue>()
                    .Where(v => v.NodeID == line.NodeID && v.LineID == line.LineID)// && v.TimeStamp > DateTime.Now.AddDays(-1))
                    .OrderByDescending(v => v.TimeStamp)
                    .Take(count)
                    .OrderBy(v => v.TimeStamp)
                    .ToList();
        }

        public List<WemosMonitor> GetMonitors()
        {
            using (var db = Context.OpenConnection())
                return db.Table<WemosMonitor>().ToList();
        }
        public WemosMonitor GetMonitor(int id)
        {
            using (var db = Context.OpenConnection())
                return db.Table<WemosMonitor>().Where(m => m.ID == id).FirstOrDefault();
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
        #endregion

        #region Event handlers
        private async void OnMessageReceived(object sender, WemosMessageEventArgs e, HostName remoteAddress)
        {
            if (e.Message != null)
            {
                //MessageReceived?.Invoke(this, e, remoteAddress); // TODO: temporary!!!
                await ProcessMessage(e.Message, remoteAddress);
            }
        }
        #endregion

        #region Private methods
        private async Task ProcessMessage(WemosMessage message, HostName remoteAddress)
        {
            WemosNode node = GetNode(message.NodeID);
            WemosLine line = GetLine(message.NodeID, message.LineID);

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
                            Save(node);
                        }
                        else
                        {
                            node.Type = (WemosLineType) message.SubType;
                            node.ProtocolVersion = message.GetFloat();
                            node.IPAddress = remoteAddress.CanonicalName;
                            SaveOrUpdate(node);
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
                                Save(line);
                            }
                            else
                            {
                                line.Type = (WemosLineType) message.SubType;
                                line.ProtocolVersion = message.GetFloat();
                                SaveOrUpdate(line);
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
                        //NotifyMessageCalibrationForPlugins(message); // before saving to DB plugins may adjust the sensor value due to their calibration params

                        WemosLineValue sv = new WemosLineValue()
                        {
                            NodeID = message.NodeID,
                            LineID = message.LineID,
                            TimeStamp = DateTime.UtcNow,
                            Type = (WemosLineType) message.SubType,
                            Value = message.GetFloat()
                        };
                        Save(sv);

                        line.LastTimeStamp = sv.TimeStamp;
                        line.LastValue = sv.Value;
                        SaveOrUpdate(line);

                        //NotifyForSignalR(new { MsgId = "MySensorsTileContent", Data = BuildTileContent() }); // update MySensors tile
                        NotifyMessageReceivedForPlugins(message);
                        //NotifyMessageReceivedForScripts(message);
                        //NotifyForSignalR(new { MsgId = "SensorValue", Data = sv }); // notify Web UI
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
                                    Value = (int)message.GetInteger()
                                };
                                Save(bl);

                                node.LastTimeStamp = bl.TimeStamp;
                                node.LastBatteryValue = bl.Value;
                                node.IPAddress = remoteAddress.CanonicalName;
                                SaveOrUpdate(node);

                                //NotifyMessageReceivedForPlugins(message);
                                //NotifyMessageReceivedForScripts(message);
                                //NotifyForSignalR(new { MsgId = "BatteryValue", Data = bl });
                            }
                            break;
                        case WemosInternalMessageType.Time: // seconds since 1970
                            var sec = Convert.ToInt64(DateTime.Now.Subtract(unixEpoch).TotalSeconds);
                            await Send(new WemosMessage(message.NodeID, message.LineID, WemosMessageType.Internal, (int) WemosInternalMessageType.Time).Set(sec));
                            break;
                        case WemosInternalMessageType.Version:
                            if (node != null)
                            {
                                node.ProtocolVersion = message.GetFloat();
                                SaveOrUpdate(node);
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

                                SaveOrUpdate(node);

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

        #region DB
        private WemosSetting GetSetting(string name)
        {
            using (var db = Context.OpenConnection())
                return db.Table<WemosSetting>().Where(setting => setting.Name == name).FirstOrDefault();
        }
        private void Save(object item)
        {
            using (var db = Context.OpenConnection())
                db.Insert(item);
        }
        private void SaveOrUpdate(object item)
        {
            using (var db = Context.OpenConnection())
                db.InsertOrReplace(item);
        }
        private void Delete(object item)
        {
            using (var db = Context.OpenConnection())
                db.Delete(item);
        }
        #endregion

        #region Remote API
        [ApiCommand(CommandName = "/api/wemos/nodes"), Export(typeof(ApiCommand))]
        public ApiCommand apiGetNodes => ((parameters) =>
        {
            return Context.GetPlugin<WemosPlugin>().GetNodes();
        });

        [ApiCommand(CommandName = "/api/wemos/lines"), Export(typeof(ApiCommand))]
        public ApiCommand apiGetLines => ((parameters) =>
        {
            return Context.GetPlugin<WemosPlugin>().GetLines();
        });

        [ApiCommand(CommandName = "/api/wemos/nodes/setname"), Export(typeof(ApiCommand))]
        public ApiCommand apiSetNodeName => ((parameters) =>
        {
            var id = int.Parse(parameters[0].ToString());
            var name = parameters[1] as string;

            var node = GetNode(id);
            node.Name = name;
            SaveOrUpdate(node);

            //NotifyForSignalR(new { MsgId = "NodeNameChanged", Data = new { Id = id, Name = name } });

            return true;
        });

        [ApiCommand(CommandName = "/api/wemos/lines/setname"), Export(typeof(ApiCommand))]
        public ApiCommand apiSetLineName => ((parameters) =>
        {
            var nodeID = int.Parse(parameters[0].ToString());
            var lineID = int.Parse(parameters[1].ToString());
            var name = parameters[2] as string;

            var line = GetLine(nodeID, lineID);
            line.Name = name;
            SaveOrUpdate(line);

            //NotifyForSignalR(new { MsgId = "SensorNameChanged", Data = new { Id = id, Name = name } });

            return true;
        });

        [ApiCommand(CommandName = "/api/wemos/lines/getname"), Export(typeof(ApiCommand))]
        public ApiCommand apiGetLineName => ((parameters) =>
        {
            var id = int.Parse(parameters[1].ToString());

            return GetLine(id).Name;
        });

        [ApiCommand(CommandName = "/api/wemos/line/values"), Export(typeof(ApiCommand))]
        public ApiCommand apiGetLineValues => ((parameters) =>
        {
            var id = int.Parse(parameters[0].ToString());
            var count = int.Parse(parameters[1].ToString());

            return GetLineValues(id, count);
        });

        [ApiCommand(CommandName = "/api/wemos/monitors"), Export(typeof(ApiCommand))]
        public ApiCommand apiGetMonitors => ((parameters) =>
        {
            return Context.GetPlugin<WemosPlugin>().GetMonitors().Select(m => new WemosMonitorDto(m)
            {
                LineName = GetLine(m.LineID).Name,
                LineType = GetLine(m.LineID).Type
            }).ToList();
        });

        [ApiCommand(CommandName = "/api/wemos/monitor"), Export(typeof(ApiCommand))]
        public ApiCommand apiGetMonitor => ((parameters) =>
        {
            var id = int.Parse(parameters[0].ToString());

            var monitor = GetMonitor(id);
            var line = GetLine(monitor.LineID);
            return new WemosMonitorDto(monitor) { LineName = line.Name, LineType = line.Type };
        });

        [ApiCommand(CommandName = "/api/wemos/monitors/add"), Export(typeof(ApiCommand))]
        public ApiCommand apiAddMonitor => ((parameters) =>
        {
            var name = parameters[0] as string;
            var lineID = int.Parse(parameters[1].ToString());

            WemosMonitor monitor = new WemosMonitor()
            {
                Name = name,
                LineID = lineID,
                Configuration = "{}"
            };

            Save(monitor);

            //NotifyForSignalR(new { MsgId = "MonitorAdded", Data = BuildMonitorWebModel(ctrl) });

            var line = GetLine(monitor.LineID);
            var m = new WemosMonitorDto(monitor);
            m.LineName = line.Name;
            m.LineType = line.Type;

            return m;
        });

        [ApiCommand(CommandName = "/api/wemos/monitors/setnames"), Export(typeof(ApiCommand))]
        public ApiCommand apiSetMonitorNames => ((parameters) =>
        {
            var id = int.Parse(parameters[0].ToString());
            var name = parameters[1] as string;
            var nameForInformer = parameters[2] as string;

            var monitor = GetMonitor(id);
            monitor.Name = name;
            monitor.NameForInformer = nameForInformer;
            SaveOrUpdate(monitor);

            return true;
        });

        [ApiCommand(CommandName = "/api/wemos/monitors/delete"), Export(typeof(ApiCommand))]
        public ApiCommand apiDeleteMonitor => ((parameters) =>
        {
            var id = int.Parse(parameters[0].ToString());

            var monitor = GetMonitor(id);
            Delete(monitor);

            return true;
        });



        #endregion
    }
}
