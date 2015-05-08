using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using SmartHub.Core.Plugins;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Attributes;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.MySensors.Data;
using SmartHub.Plugins.MySensors.GatewayProxies;
using SmartHub.Plugins.SignalR;
using SmartHub.Plugins.Timer;
using SmartHub.Plugins.WebUI.Attributes;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace SmartHub.Plugins.MySensors
{
    [AppSection("Сеть MySensors", SectionType.System, "/webapp/mysensors/module.js", "SmartHub.Plugins.MySensors.Resources.js.module.js", TileTypeFullName = "SmartHub.Plugins.MySensors.MySensorsTile")]
    //[JavaScriptResource("/webapp/mysensors/views.js", "SmartHub.Plugins.MySensors.Resources.js.views.js")]
    [HttpResource("/webapp/mysensors/templates.html", "SmartHub.Plugins.MySensors.Resources.js.templates.html")]
    [CssResource("/webapp/mysensors/css/style.css", "SmartHub.Plugins.MySensors.Resources.css.style.css", AutoLoad = true)]
    [Plugin]
    public class MySensorsPlugin : PluginBase
    {
        #region Fields
        private bool isSerialGateway = true;
        private IGatewayProxy gatewayProxy;
        private SignalRPlugin signalServer;
        private string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        #endregion

        #region Import
        [ImportMany("7CDDD153-64E0-4050-8533-C47C1BACBC6B")]
        public Action<SensorMessage>[] SensorMessageHandlers { get; set; }
        #endregion

        #region Plugin overrides
        public override void InitDbModel(ModelMapper mapper)
        {
            mapper.Class<Setting>(cfg => cfg.Table("MySensors_Settings"));
            mapper.Class<Node>(cfg => cfg.Table("MySensors_Nodes"));
            mapper.Class<Sensor>(cfg => cfg.Table("MySensors_Sensors"));
            mapper.Class<BatteryLevel>(cfg => cfg.Table("MySensors_BatteryLevels"));
            mapper.Class<SensorValue>(cfg => cfg.Table("MySensors_SensorValues"));
        }
        public override void InitPlugin()
        {
            if (GetSetting("UnitSystem") == null)
                Save(new Setting() { Id = Guid.NewGuid(), Name = "UnitSystem", Value = "M" });
            //if (GetSetting("SerialPortName") == null)
            //    Save(new Setting() { Id = Guid.NewGuid(), Name = "SerialPortName", Value = "" });

            gatewayProxy = isSerialGateway ? (IGatewayProxy)new SerialGatewayProxy() : (IGatewayProxy)new EthernetGatewayProxy();
            gatewayProxy.Connected += gatewayProxy_Connected;
            gatewayProxy.MessageReceived += gatewayProxy_MessageReceived;
            gatewayProxy.Disconnected += gatewayProxy_Disconnected;

            signalServer = Context.GetPlugin<SignalRPlugin>();
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

        public void SetSensorValue(Sensor sensor, SensorValueType type, float value)
        {
            if (gatewayProxy != null && sensor != null)
                gatewayProxy.Send(new SensorMessage(sensor.NodeNo, sensor.SensorNo, SensorMessageType.Set, false, (byte)type, value.ToString()));
        }

        #region DB actions
        private Setting GetSetting(string name)
        {
            using (var session = Context.OpenSession())
                return session.Query<Setting>().FirstOrDefault(setting => setting.Name == name);
        }
        private Node GetNode(byte nodeID)
        {
            using (var session = Context.OpenSession())
                return session.Query<Node>().FirstOrDefault(node => node.NodeNo == nodeID);
        }
        public Sensor GetSensor(byte nodeID, byte sensorID)
        {
            using (var session = Context.OpenSession())
                return session.Query<Sensor>().FirstOrDefault(sensor => sensor.NodeNo == nodeID && sensor.SensorNo == sensorID);
        }
        public SensorValue GetSensorValue(Sensor sensor)
        {
            using (var session = Context.OpenSession())
                return session.Query<SensorValue>().Where(sv => sv.NodeNo == sensor.NodeNo && sv.SensorNo == sensor.SensorNo).OrderByDescending(sv => sv.TimeStamp).FirstOrDefault();
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
        [Timer_3_sec_Elapsed]
        private void Timer_1_sec_Elapsed(DateTime now)
        {
            //int a = 0;
            //int b = a;
        }

        [Timer_5_sec_Elapsed]
        private void Timer_5_sec_Elapsed(DateTime now)
        {
            signalServer.Broadcast(new { MsgId = "Test", Value = now });
        }

        private void gatewayProxy_Connected(object sender, EventArgs e)
        {
            Logger.Info("Connected.");
            Debug.WriteLine("Connected.");
        }
        private void gatewayProxy_MessageReceived(IGatewayProxy sender, SensorMessageEventArgs args)
        {
            SensorMessage message = args.Message;
            Debug.WriteLine(message.ToString());

            bool isNodeMessage = message.NodeID == 0 || message.SensorID == 255;
            Node node = GetNode(message.NodeID);
            Sensor sensor = GetSensor(message.NodeID, message.SensorID); // if message.SensorID == 255 it returns null

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
                                NodeNo = message.NodeID,
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
                        Run(SensorMessageHandlers, x => x(message));
                        signalServer.Broadcast(new { MsgId = "NodePresentation", Value = BuildNodeModel(node) });
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
                                    SensorNo = message.SensorID,
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
                            Run(SensorMessageHandlers, x => x(message));
                            signalServer.Broadcast(new { MsgId = "SensorPresentation", Value = BuildSensorModel(sensor) });
                        }
                    }
                    break;
                #endregion

                #region Set
                case SensorMessageType.Set: // sent from or to a sensor when a sensor value should be updated
                    if (sensor != null)
                    {
                        SensorValue sv = new SensorValue()
                        {
                            Id = Guid.NewGuid(),
                            NodeNo = message.NodeID,
                            SensorNo = message.SensorID,
                            TimeStamp = DateTime.Now,
                            Type = (SensorValueType)message.SubType,
                            Value = float.Parse(message.Payload.Replace(",", decimalSeparator).Replace(".", decimalSeparator))
                        };

                        Save(sv);
                        Run(SensorMessageHandlers, x => x(message));
                        signalServer.Broadcast(new { MsgId = "SensorValue", Value = sv });
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
                                    NodeNo = message.NodeID,
                                    TimeStamp = DateTime.Now,
                                    Level = byte.Parse(message.Payload)
                                };

                                Save(bl);
                                Run(SensorMessageHandlers, x => x(message));
                                signalServer.Broadcast(new { MsgId = "BatteryLevel", Value = bl });
                            }
                            break;
                        case InternalValueType.Time:
                            gatewayProxy.Send(new SensorMessage(message.NodeID, message.SensorID, SensorMessageType.Internal, false, (byte)InternalValueType.Time, GetTimeForSensors().ToString()));
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
                            gatewayProxy.Send(new SensorMessage(message.NodeID, 255, SensorMessageType.Internal, false, (byte)InternalValueType.Config, GetSetting("UnitSystem").Value));
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
                                Run(SensorMessageHandlers, x => x(message));
                                signalServer.Broadcast(new { MsgId = "NodePresentation", Value = BuildNodeModel(node) });
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
            Logger.Error("Disconnected.");
            Debug.WriteLine("Disconnected.");
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
        private object BuildNodeModel(Node node)
        {
            if (node == null)
                return null;

            using (var session = Context.OpenSession())
                return new
                {
                    Id = node.Id,
                    NodeNo = node.NodeNo,
                    TypeName = node.TypeName,
                    ProtocolVersion = node.ProtocolVersion,
                    SketchName = node.SketchName,
                    SketchVersion = node.SketchVersion,
                    Name = node.Name,
                    BatteryLevel = session.Query<BatteryLevel>().Where(bl => bl.NodeNo == node.NodeNo).OrderByDescending(bl => bl.TimeStamp).FirstOrDefault()
                };
        }
        private object BuildSensorModel(Sensor sensor)
        {
            if (sensor == null)
                return null;

            //SensorValue lastSv = null;
            //using (var session = Context.OpenSession())
            //{
            //    var svs = session.Query<SensorValue>().Where(sv => sv.NodeNo == sensor.NodeNo && sv.SensorNo == sensor.SensorNo);
            //    var lastTimeStamp = svs.Select(vv => vv.TimeStamp).Max();
            //    lastSv = svs.Any() ? svs.FirstOrDefault(v => v.TimeStamp == lastTimeStamp) : null;
            //}
            //return new
            //{
            //    Id = sensor.Id,
            //    NodeNo = sensor.NodeNo,
            //    SensorNo = sensor.SensorNo,
            //    TypeName = sensor.TypeName,
            //    ProtocolVersion = sensor.ProtocolVersion,
            //    Name = sensor.Name,
            //    SensorValue = lastSv
            //};

            using (var session = Context.OpenSession())
                return new
                {
                    Id = sensor.Id,
                    NodeNo = sensor.NodeNo,
                    SensorNo = sensor.SensorNo,
                    TypeName = sensor.TypeName,
                    ProtocolVersion = sensor.ProtocolVersion,
                    Name = sensor.Name,
                    SensorValue = session.Query<SensorValue>().Where(sv => sv.NodeNo == sensor.NodeNo && sv.SensorNo == sensor.SensorNo).OrderByDescending(sv => sv.TimeStamp).FirstOrDefault()
                };
        }
        #endregion

        #region Web API
        [HttpCommand("/api/mysensors/nodes")]
        private object GetNodes(HttpRequestParams request)
        {
            using (var session = Context.OpenSession())
                return session.Query<Node>().Select(BuildNodeModel).Where(x => x != null).ToArray();
        }
        [HttpCommand("/api/mysensors/nodes/setname")]
        private object SetNodeName(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("Id");
            var name = request.GetString("Name");

            using (var session = Context.OpenSession())
            {
                var node = session.Get<Node>(id);
                node.Name = name;
                session.Flush();
            }

            signalServer.Broadcast(new {
                MsgId = "NodeNameChanged",
                Value = new {
                    Id = id,
                    Name = name
                }
            });

            return null;
        }
        [HttpCommand("/api/mysensors/nodes/delete")]
        private object DeleteNode(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("Id");

            using (var session = Context.OpenSession())
            {
                var node = session.Load<Node>(id);
                session.Delete(node);
                session.Flush();
            }

            signalServer.Broadcast(new {
                MsgId = "NodeDeleted",
                Value = new { Id = id }
            });

            return null;
        }

        [HttpCommand("/api/mysensors/sensors")]
        public object GetSensors(HttpRequestParams request)
        {
            using (var session = Context.OpenSession())
                return session.Query<Sensor>().Select(BuildSensorModel).Where(x => x != null).ToArray();
        }
        [HttpCommand("/api/mysensors/sensors/setname")]
        private object SetSensorName(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("Id");
            var name = request.GetString("Name");

            using (var session = Context.OpenSession())
            {
                var sensor = session.Get<Sensor>(id);
                sensor.Name = name;
                session.Flush();
            }

            signalServer.Broadcast(new {
                MsgId = "SensorNameChanged",
                Value = new {
                    Id = id,
                    Name = name
                }
            });

            return null;
        }
        [HttpCommand("/api/mysensors/sensors/delete")]
        private object DeleteSensor(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("Id");

            using (var session = Context.OpenSession())
            {
                var sensor = session.Load<Sensor>(id);
                session.Delete(sensor);
                session.Flush();
            }

            signalServer.Broadcast(new {
                MsgId = "SensorDeleted",
                Value = new { Id = id }
            });

            return null;
        }

        [HttpCommand("/api/mysensors/unitsystem")]
        private object GetUnitSystem(HttpRequestParams request)
        {
            return GetSetting("UnitSystem");
        }
        [HttpCommand("/api/mysensors/setunitsystem")]
        private object SetUnitSystem(HttpRequestParams request)
        {
            var value = request.GetRequiredString("Value");

            using (var session = Context.OpenSession())
            {
                var setting = session.Query<Setting>().FirstOrDefault(s => s.Name == "UnitSystem");
                setting.Value = value;
                session.Flush();
            }

            //var setting = GetSetting("UnitSystem");
            //setting.Value = value;
            //SaveOrUpdate(setting);

            signalServer.Broadcast(new {
                MsgId = "UnitSystemChanged",
                Value = value
            });

            return null;
        }

        [HttpCommand("/api/mysensors/batterylevels")]
        private object GetBatteryLevels(HttpRequestParams request)
        {
            using (var session = Context.OpenSession())
                return session.Query<BatteryLevel>().ToArray();
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
