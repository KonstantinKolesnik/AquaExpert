using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using SmartHub.Core.Plugins;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Attributes;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.MySensors.Data;
using SmartHub.Plugins.MySensors.GatewayProxies;
using SmartHub.Plugins.Scripts;
using SmartHub.Plugins.Scripts.Attributes;
using SmartHub.Plugins.SignalR;
using SmartHub.Plugins.WebUI.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SmartHub.Plugins.MySensors
{
    [AppSection("Сеть MySensors", SectionType.System, "/webapp/mysensors/module.js", "SmartHub.Plugins.MySensors.Resources.js.module.js", TileTypeFullName = "SmartHub.Plugins.MySensors.MySensorsTile")]
    [JavaScriptResource("/webapp/mysensors/module-model.js", "SmartHub.Plugins.MySensors.Resources.js.module-model.js")]
    [JavaScriptResource("/webapp/mysensors/module-view.js", "SmartHub.Plugins.MySensors.Resources.js.module-view.js")]
    [HttpResource("/webapp/mysensors/module.html", "SmartHub.Plugins.MySensors.Resources.js.module.html")]
    [CssResource("/webapp/mysensors/css/style.css", "SmartHub.Plugins.MySensors.Resources.css.style.css", AutoLoad = true)]

    [Plugin]
    public class MySensorsPlugin : PluginBase
    {
        #region Fields
        private bool isSerialGateway = true;
        private IGatewayProxy gatewayProxy;
        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        #endregion

        #region Import
        [ImportMany("46CD89C9-08A2-4E3C-84EE-826DA51127CF")]
        public Action[] ConnectedHandlers { get; set; }
        private void NotifyConnectedForPlugins()
        {
            Run(ConnectedHandlers, x => x());
        }

        [ImportMany("7CDDD153-64E0-4050-8533-C47C1BACBC6B")]
        public Action<SensorMessage>[] SensorMessageHandlers { get; set; }
        private void NotifyMessageReceivedForPlugins(SensorMessage msg)
        {
            Run(SensorMessageHandlers, x => x(msg));
        }

        [ImportMany("6536BBC2-3B62-4D01-B786-E5A4A2D7A095")]
        public Action<SensorMessage>[] SensorMessageCalibrationHandlers { get; set; }
        private void NotifyMessageCalibrationForPlugins(SensorMessage msg)
        {
            Run(SensorMessageCalibrationHandlers, x => x(msg));
        }

        [ImportMany("8E26E0FB-657F-4DEA-BF9D-A9E93A5632DC")]
        public Action[] DisconnectedHandlers { get; set; }
        private void NotifyDisconnectedForPlugins()
        {
            Run(DisconnectedHandlers, x => x());
        }
        #endregion

        #region Script commands & events
        [ScriptCommand("mySensorsSendCommand")]
        public void SendCommand(int nodeNo, int sensorNo, int commandType, int valueType, float value)
        {
            if (gatewayProxy != null)
                gatewayProxy.Send(new SensorMessage((byte)nodeNo, (byte)sensorNo, (SensorMessageType)commandType, false, (byte)valueType, value));
        }

        [ScriptEvent("mySensors.connected")]
        public ScriptEventHandlerDelegate[] OnConnectedForScripts { get; set; }
        private void NotifyConnectedForScripts()
        {
            this.RaiseScriptEvent(x => x.OnConnectedForScripts);
        }

        [ScriptEvent("mySensors.messageReceived")]
        public ScriptEventHandlerDelegate[] OnMessageReceivedForScripts { get; set; }
        private void NotifyMessageReceivedForScripts(SensorMessage msg)
        {
            this.RaiseScriptEvent(x => x.OnMessageReceivedForScripts, msg.NodeNo, msg.SensorNo, msg.Type, msg.SubType, msg.Payload);
        }

        [ScriptEvent("mySensors.disconnected")]
        public ScriptEventHandlerDelegate[] OnDisconnectedForScripts { get; set; }
        private void NotifyDisconnectedForScripts()
        {
            this.RaiseScriptEvent(x => x.OnDisconnectedForScripts);
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
            mapper.Class<MySensorsSetting>(cfg => cfg.Table("MySensors_Settings"));
            mapper.Class<Node>(cfg => cfg.Table("MySensors_Nodes"));
            mapper.Class<Sensor>(cfg => cfg.Table("MySensors_Sensors"));
            mapper.Class<BatteryLevel>(cfg => cfg.Table("MySensors_BatteryLevels"));
            mapper.Class<SensorValue>(cfg => cfg.Table("MySensors_SensorValues"));
        }
        public override void InitPlugin()
        {
            if (GetSetting("UnitSystem") == null)
                Save(new MySensorsSetting() { Id = Guid.NewGuid(), Name = "UnitSystem", Value = "M" });
            //if (GetSetting("SerialPortName") == null)
            //    Save(new Setting() { Id = Guid.NewGuid(), Name = "SerialPortName", Value = "" });

            gatewayProxy = isSerialGateway ? (IGatewayProxy)new SerialGatewayProxy() : (IGatewayProxy)new EthernetGatewayProxy();
            gatewayProxy.Connected += gatewayProxy_Connected;
            gatewayProxy.MessageReceived += gatewayProxy_MessageReceived;
            gatewayProxy.Disconnected += gatewayProxy_Disconnected;
        }
        public override void StartPlugin()
        {
            if (!gatewayProxy.IsStarted)
            {
                Logger.Info("Connecting to gateway...");
                Debug.WriteLine("Connecting to gateway...");

                try
                {
                    gatewayProxy.Start();
                }
                catch (Exception) { }
            }
        }
        public override void StopPlugin()
        {
            gatewayProxy.Stop();
        }
        #endregion

        #region Public methods
        public void RequestSensorValue(Sensor sensor, SensorValueType type)
        {
            if (gatewayProxy != null && sensor != null)
                gatewayProxy.Send(new SensorMessage(sensor.NodeNo, sensor.SensorNo, SensorMessageType.Request, false, (byte)type, ""));
        }
        public void SetSensorValue(Sensor sensor, SensorValueType type, float value)
        {
            if (gatewayProxy != null && sensor != null)
                gatewayProxy.Send(new SensorMessage(sensor.NodeNo, sensor.SensorNo, SensorMessageType.Set, false, (byte)type, value));
        }
        
        public Node GetNode(byte nodeNo)
        {
            using (var session = Context.OpenSession())
                return session.Query<Node>().FirstOrDefault(node => node.NodeNo == nodeNo);
        }
        public Sensor GetSensor(byte nodeNo, byte sensorNo)
        {
            using (var session = Context.OpenSession())
                return session.Query<Sensor>().FirstOrDefault(sensor => sensor.NodeNo == nodeNo && sensor.SensorNo == sensorNo);
        }
        public Sensor GetSensor(Guid sensorID)
        {
            using (var session = Context.OpenSession())
                return session.Query<Sensor>().FirstOrDefault(sensor => sensor.Id == sensorID);
        }
        public List<Sensor> GetSensorsByType(SensorType type)
        {
            using (var session = Context.OpenSession())
                return session.Query<Sensor>().Where(s => s.Type == type).ToList();
        }
        public SensorValue GetLastSensorValue(Sensor sensor)
        {
            if (sensor == null)
                return null;

            using (var session = Context.OpenSession())
                return session.Query<SensorValue>()
                    .Where(sv => sv.NodeNo == sensor.NodeNo && sv.SensorNo == sensor.SensorNo)
                    .OrderByDescending(sv => sv.TimeStamp)
                    .FirstOrDefault();
        }
        public bool IsMessageFromSensor(SensorMessage msg, Sensor sensor)
        {
            return msg != null && sensor != null && sensor.NodeNo == msg.NodeNo && sensor.SensorNo == msg.SensorNo;
        }
        public object BuildSensorWebModel(Sensor sensor)
        {
            if (sensor == null)
                return null;

            SensorValue lastSV = null;
            using (var session = Context.OpenSession())
                lastSV = session.Query<SensorValue>().Where(sv => sv.NodeNo == sensor.NodeNo && sv.SensorNo == sensor.SensorNo).OrderByDescending(sv => sv.TimeStamp).FirstOrDefault();

            return new
            {
                Id = sensor.Id,
                Name = sensor.Name,
                NodeNo = sensor.NodeNo,
                SensorNo = sensor.SensorNo,
                TypeName = sensor.TypeName,
                ProtocolVersion = sensor.ProtocolVersion,
                SensorValueValue = lastSV == null ? (float?)null : lastSV.Value,
                SensorValueTimeStamp = lastSV == null ? (DateTime?)null : lastSV.TimeStamp
            };
        }
        public void RebootNode(Node node)
        {
            if (gatewayProxy != null && node != null)
                gatewayProxy.Send(new SensorMessage(node.NodeNo, 255, SensorMessageType.Internal, false, (byte)InternalValueType.Reboot, ""));
        }

        public string BuildTileContent()
        {
            SensorValue lastSV = null;
            using (var session = Context.OpenSession())
                lastSV = session.Query<SensorValue>().OrderByDescending(sv => sv.TimeStamp).FirstOrDefault();

            StringBuilder sb = new StringBuilder();
            if (lastSV != null)
            {
                sb.AppendFormat("<span>{0:dd.MM.yyyy}</span>&nbsp;&nbsp;<span style='font-size:0.9em; font-style:italic;'>{0:HH:mm:ss}</span>", lastSV.TimeStamp);
                sb.AppendFormat("<div>[{0}][{1}] {2}: {3}</div>", lastSV.NodeNo, lastSV.SensorNo, lastSV.Type.ToString(), lastSV.Value);
            }
            return sb.ToString();
        }
        public string BuildSignalRReceiveHandler()
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

            sb.Append("if (data.MsgId == 'MySensorsTileContent') { ");
            sb.Append("model.tileModel.set({ 'content': data.Data }); ");
            sb.Append("}");
            return sb.ToString();
        }
        #endregion

        #region Private methods
        private void GetNextAvailableNodeID()
        {
            using (var session = Context.OpenSession())
            {
                var nds = session.Query<Node>().OrderBy(node => node.NodeNo).ToList();

                byte id = 1;
                for (byte i = 0; i < nds.Count; i++)
                    if (nds[i].NodeNo > i + 1)
                    {
                        id = (byte)(i + 1);
                        break;
                    }
                    else
                        id++;

                if (id < 255)
                {
                    Node node = new Node { Id = Guid.NewGuid(), NodeNo = id };
                    Save(node);

                    gatewayProxy.Send(new SensorMessage(255, 255, SensorMessageType.Internal, false, (byte)InternalValueType.IDResponse, id.ToString()));
                }
            }
        }
        private int GetTimeForSensors() // seconds since 1970
        {
            DateTime dtNow = DateTime.Now;

            TimeSpan result = dtNow.Subtract(unixEpoch);
            return Convert.ToInt32(result.TotalSeconds);
        }
        private object BuildNodeWebModel(Node node)
        {
            if (node == null)
                return null;

            BatteryLevel lastBL = null;
            using (var session = Context.OpenSession())
                lastBL = session.Query<BatteryLevel>().Where(bl => bl.NodeNo == node.NodeNo).OrderByDescending(bl => bl.TimeStamp).FirstOrDefault();

            return new
            {
                Id = node.Id,
                Name = node.Name,
                NodeNo = node.NodeNo,
                TypeName = node.TypeName,
                ProtocolVersion = node.ProtocolVersion,
                SketchName = node.SketchName,
                SketchVersion = node.SketchVersion,
                BatteryLevelLevel = lastBL == null ? (byte?)null : lastBL.Level,
                BatteryLevelTimeStamp = lastBL == null ? (DateTime?)null : lastBL.TimeStamp
            };
        }
        private object BuildSensorSummaryWebModel(Sensor sensor)
        {
            if (sensor == null)
                return null;

            return new
            {
                Id = sensor.Id,
                Name = sensor.Name
            };
        }
        private SensorValue SaveSensorValueToDB(SensorMessage message)
        {
            SensorValue sv = new SensorValue()
            {
                Id = Guid.NewGuid(),
                NodeNo = message.NodeNo,
                SensorNo = message.SensorNo,
                TimeStamp = DateTime.UtcNow,
                Type = (SensorValueType)message.SubType,
                Value = message.PayloadFloat
            };

            Save(sv);

            return sv;
        }
        
        private MySensorsSetting GetSetting(string name)
        {
            using (var session = Context.OpenSession())
                return session.Query<MySensorsSetting>().FirstOrDefault(setting => setting.Name == name);
        }
        private void Save(object item)
        {
            using (var session = Context.OpenSession())
            {
                try
                {
                    session.Save(item);
                    session.Flush();
                }
                catch (Exception) {}
            }
        }
        private void SaveOrUpdate(object item)
        {
            using (var session = Context.OpenSession())
            {
                session.SaveOrUpdate(item);
                session.Flush();
            }
        }
        private void Delete(object item)
        {
            using (var session = Context.OpenSession())
            {
                session.Delete(item);
                session.Flush();
            }
        }
        #endregion

        #region Event handlers
        private void gatewayProxy_Connected(object sender, EventArgs e)
        {
            Logger.Info("Connected.");
            NotifyConnectedForPlugins();
            NotifyConnectedForScripts();
        }
        private void gatewayProxy_MessageReceived(IGatewayProxy sender, SensorMessageEventArgs args)
        {
            SensorMessage message = args.Message;

            Debug.WriteLine(message.ToString());

            bool isNodeMessage = message.NodeNo == 0 || message.SensorNo == 255;
            Node node = GetNode(message.NodeNo);
            Sensor sensor = GetSensor(message.NodeNo, message.SensorNo); // if message.SensorID == 255 it returns null

            switch (message.Type)
            {
                #region Presentation
                case SensorMessageType.Presentation: // sent by a nodes when they present attached sensors.
                    if (isNodeMessage)
                    {
                        if (node == null)
                        {
                            node = new Node
                            {
                                Id = Guid.NewGuid(),
                                NodeNo = message.NodeNo,
                                Type = (SensorType)message.SubType,
                                ProtocolVersion = message.Payload
                            };

                            Save(node);
                        }
                        else
                        {
                            node.Type = (SensorType)message.SubType;
                            node.ProtocolVersion = message.Payload;

                            SaveOrUpdate(node);
                        }

                        NotifyMessageReceivedForPlugins(message);
                        NotifyMessageReceivedForScripts(message);
                        NotifyForSignalR(new { MsgId = "NodePresentation", Data = BuildNodeWebModel(node) });
                    }
                    else // sensor message
                    {
                        if (node != null)
                        {
                            if (sensor == null)
                            {
                                sensor = new Sensor()
                                {
                                    Id = Guid.NewGuid(),
                                    NodeNo = node.NodeNo,
                                    SensorNo = message.SensorNo,
                                    Type = (SensorType)message.SubType,
                                    ProtocolVersion = message.Payload
                                };

                                Save(sensor);
                            }
                            else
                            {
                                sensor.Type = (SensorType)message.SubType;
                                sensor.ProtocolVersion = message.Payload;

                                SaveOrUpdate(sensor);
                            }

                            NotifyMessageReceivedForPlugins(message);
                            NotifyMessageReceivedForScripts(message);
                            NotifyForSignalR(new { MsgId = "SensorPresentation", Data = BuildSensorWebModel(sensor) });
                        }
                    }
                    break;
                #endregion

                #region Set
                case SensorMessageType.Set: // sent from or to a sensor when a sensor value should be updated
                    if (sensor != null)
                    {
                        NotifyMessageCalibrationForPlugins(message); // before saving to DB plugins may adjust the sensor value due to their calibration params
                        var sv = SaveSensorValueToDB(message);
                        NotifyForSignalR(new { MsgId = "MySensorsTileContent", Data = BuildTileContent() });
                        
                        NotifyMessageReceivedForPlugins(message);
                        NotifyMessageReceivedForScripts(message);
                        NotifyForSignalR(new { MsgId = "SensorValue", Data = sv });
                    }
                    break;
                #endregion

                #region Request
                case SensorMessageType.Request: // requests a variable value (usually from an actuator destined for controller)
                    break;
                #endregion

                #region Internal
                case SensorMessageType.Internal: // special internal message
                    InternalValueType ivt = (InternalValueType)message.SubType;

                    switch (ivt)
                    {
                        case InternalValueType.BatteryLevel: // int, in %
                            if (node != null)
                            {
                                BatteryLevel bl = new BatteryLevel()
                                {
                                    Id = Guid.NewGuid(),
                                    NodeNo = message.NodeNo,
                                    TimeStamp = DateTime.UtcNow,
                                    Level = byte.Parse(message.Payload)
                                };

                                Save(bl);

                                NotifyMessageReceivedForPlugins(message);
                                NotifyMessageReceivedForScripts(message);
                                NotifyForSignalR(new { MsgId = "BatteryLevel", Data = bl });
                            }
                            break;
                        case InternalValueType.Time:
                            gatewayProxy.Send(new SensorMessage(message.NodeNo, message.SensorNo, SensorMessageType.Internal, false, (byte)InternalValueType.Time, GetTimeForSensors().ToString()));
                            break;
                        case InternalValueType.Version:
                            break;
                        case InternalValueType.IDRequest:
                            GetNextAvailableNodeID();
                            break;
                        case InternalValueType.IDResponse:
                            break;
                        case InternalValueType.InclusionMode:
                            break;
                        case InternalValueType.Config:
                            gatewayProxy.Send(new SensorMessage(message.NodeNo, 255, SensorMessageType.Internal, false, (byte)InternalValueType.Config, GetSetting("UnitSystem").Value));
                            break;
                        case InternalValueType.FindParent:
                            break;
                        case InternalValueType.FindParentResponse:
                            break;
                        case InternalValueType.LogMessage:
                            break;
                        case InternalValueType.Children:
                            break;
                        case InternalValueType.SketchName:
                        case InternalValueType.SketchVersion:
                            if (node != null)
                            {
                                if (ivt == InternalValueType.SketchName)
                                    node.SketchName = message.Payload;
                                else
                                    node.SketchVersion = message.Payload;

                                SaveOrUpdate(node);

                                NotifyMessageReceivedForPlugins(message);
                                NotifyMessageReceivedForScripts(message);
                                NotifyForSignalR(new { MsgId = "NodePresentation", Data = BuildNodeWebModel(node) });
                            }
                            break;
                        case InternalValueType.Reboot:
                            break;
                        case InternalValueType.GatewayReady:
                            break;
                    }
                    break;
                #endregion

                #region Stream
                case SensorMessageType.Stream: //used for OTA firmware updates
                    switch ((StreamValueType)message.SubType)
                    {
                        case StreamValueType.FirmwareConfigRequest:
                            //var fwtype = pullWord(payload, 0);
                            //var fwversion = pullWord(payload, 2);
                            //sendFirmwareConfigResponse(sender, fwtype, fwversion, db, gw);
                            break;
                        case StreamValueType.FirmwareConfigResponse:
                            break;
                        case StreamValueType.FirmwareRequest:
                            break;
                        case StreamValueType.FirmwareResponse:
                            //var fwtype = pullWord(payload, 0);
                            //var fwversion = pullWord(payload, 2);
                            //var fwblock = pullWord(payload, 4);
                            //sendFirmwareResponse(sender, fwtype, fwversion, fwblock, db, gw);
                            break;
                        case StreamValueType.Sound:
                            break;
                        case StreamValueType.Image:
                            break;
                    }
                    break;
                #endregion
            }

            //if (node != null && node.Reboot)
            //    gatewayProxy.Send(new Message(node.NodeID, 255, MessageType.Internal, false, (byte)InternalValueType.Reboot, ""));
        }
        private void gatewayProxy_Disconnected(object sender, EventArgs e)
        {
            Logger.Info("Disconnected.");
            NotifyDisconnectedForPlugins();
            NotifyDisconnectedForScripts();
        }
        #endregion

        #region Web API
        [HttpCommand("/api/mysensors/nodes")]
        private object apiGetNodes(HttpRequestParams request)
        {
            using (var session = Context.OpenSession())
                return session.Query<Node>().Select(BuildNodeWebModel).Where(x => x != null).ToArray();
        }
        [HttpCommand("/api/mysensors/nodes/setname")]
        private object apiSetNodeName(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("Id");
            var name = request.GetString("Name");

            using (var session = Context.OpenSession())
            {
                var node = session.Get<Node>(id);
                node.Name = name;
                session.Flush();
            }

            NotifyForSignalR(new
            {
                MsgId = "NodeNameChanged",
                Data = new
                {
                    Id = id,
                    Name = name
                }
            });

            return null;
        }
        [HttpCommand("/api/mysensors/nodes/delete")]
        private object apiDeleteNode(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("Id");

            using (var session = Context.OpenSession())
            {
                var node = session.Get<Node>(id);
                session.Delete(node);
                session.Flush();
            }

            NotifyForSignalR(new
            {
                MsgId = "NodeDeleted",
                Data = new { Id = id }
            });

            return null;
        }

        [HttpCommand("/api/mysensors/sensors")]
        private object apiGetSensors(HttpRequestParams request)
        {
            using (var session = Context.OpenSession())
                return session.Query<Sensor>().Select(BuildSensorWebModel).Where(x => x != null).ToArray();
        }
        [HttpCommand("/api/mysensors/sensorsByType")]
        private object apiGetSensorsByType(HttpRequestParams request)
        {
            var type = (SensorType)request.GetRequiredInt32("type");

            return GetSensorsByType(type)
                .Select(BuildSensorSummaryWebModel)
                .Where(x => x != null)
                .ToArray();
        }
        [HttpCommand("/api/mysensors/sensor")]
        private object apiGetSensor(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            return BuildSensorWebModel(GetSensor(id));
        }
        [HttpCommand("/api/mysensors/sensors/setname")]
        private object apiSetSensorName(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("Id");
            var name = request.GetString("Name");

            using (var session = Context.OpenSession())
            {
                var sensor = session.Get<Sensor>(id);
                sensor.Name = name;
                session.Flush();
            }

            NotifyForSignalR(new
            {
                MsgId = "SensorNameChanged",
                Data = new
                {
                    Id = id,
                    Name = name
                }
            });

            return null;
        }
        [HttpCommand("/api/mysensors/sensors/delete")]
        private object apiDeleteSensor(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("Id");

            using (var session = Context.OpenSession())
            {
                var sensor = session.Get<Sensor>(id);
                session.Delete(sensor);
                session.Flush();
            }

            NotifyForSignalR(new { MsgId = "SensorDeleted", Data = new { Id = id } });

            return null;
        }

        [HttpCommand("/api/mysensors/unitsystem")]
        private object apiGetUnitSystem(HttpRequestParams request)
        {
            return GetSetting("UnitSystem");
        }
        [HttpCommand("/api/mysensors/setunitsystem")]
        private object apiSetUnitSystem(HttpRequestParams request)
        {
            var value = request.GetRequiredString("Value");

            using (var session = Context.OpenSession())
            {
                var setting = session.Query<MySensorsSetting>().FirstOrDefault(s => s.Name == "UnitSystem");
                setting.Value = value;
                session.Flush();
            }

            //var setting = GetSetting("UnitSystem");
            //setting.Value = value;
            //SaveOrUpdate(setting);

            NotifyForSignalR(new { MsgId = "UnitSystemChanged", Data = value });

            return null;
        }

        [HttpCommand("/api/mysensors/allbatterylevels")]
        private object apiGetAllBatteryLevels(HttpRequestParams request)
        {
            using (var session = Context.OpenSession())
                return session.Query<BatteryLevel>().ToArray();
        }

        [HttpCommand("/api/mysensors/allsensorvalues")]
        private object apiGetAllSensorValues(HttpRequestParams request)
        {
            using (var session = Context.OpenSession())
                return session.Query<SensorValue>().ToArray();
        }
        [HttpCommand("/api/mysensors/sensorvalues")]
        private object apiGetSensorValues(HttpRequestParams request)
        {
            var nodeNo = request.GetRequiredInt32("nodeNo");
            var sensorNo = request.GetRequiredInt32("sensorNo");
            var hours = request.GetRequiredInt32("hours");

            DateTime dt = DateTime.UtcNow.AddHours(-hours);

            using (var session = Context.OpenSession())
                return session.Query<SensorValue>().Where(sv => sv.NodeNo == nodeNo && sv.SensorNo == sensorNo && sv.TimeStamp >= dt).ToArray();
        }
        #endregion
    }
}


/*
//using (var session = Context.OpenSession())
//{
//var list = session.Query<UserScript>().ToArray();

//// создаем новй объект UserScript
//var newScript = new UserScript
//{
//    Id = Guid.NewGuid(),
//    Name = "script name",
//    Body = "script body"
//};

//// сохраняем его в БД
//session.Save(newScript);

//// ищем в БД объект с именем "test"
//var scriptForDelete = session.Query<UserScript>().FirstOrDefault(s => s.Name == "test");

//// удаляем его из БД
//session.Delete(scriptForDelete);
//session.Flush();
//}
*/
