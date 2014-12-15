using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using SmartNetwork.Core.Plugins;
using SmartNetwork.Plugins.MySensors.Core;
using SmartNetwork.Plugins.MySensors.Data;
using SmartNetwork.Plugins.MySensors.GatewayProxies;
using SmartNetwork.Plugins.Timer;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;

namespace SmartNetwork.Plugins.MySensors
{
    [Plugin]
    public class MySensorsPlugin : PluginBase
    {
        #region Fields
        private bool isSerial = true;
        private IGatewayProxy gatewayProxy;
        // for cashing:
        private List<Setting> settings;
        private List<Node> nodes;
        private List<Sensor> sensors;
        #endregion

        #region Properties
        [ImportMany("7CDDD153-64E0-4050-8533-C47C1BACBC6B")]
        public Action<SensorMessage>[] OnSensorMessage { get; set; }
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
            gatewayProxy = isSerial ? (IGatewayProxy)new SerialGatewayProxy() : (IGatewayProxy)new EthernetGatewayProxy();
            gatewayProxy.MessageReceived += gatewayProxy_MessageReceived;
        }
        public override void StartPlugin()
        {
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

            using (var session = Context.OpenSession())
            {
                settings = session.Query<Setting>().ToList();
                nodes = session.Query<Node>().ToList();
                sensors = session.Query<Sensor>().ToList();
            }

            if (GetSetting("UnitSystem") == null)
                Save(new Setting() { Id = Guid.NewGuid(), Name = "UnitSystem", Value = "M" });

            if (!gatewayProxy.IsStarted)
            {
                Logger.Info("Connecting to gateway...");

                try
                {
                    gatewayProxy.Start();
                }
                catch (Exception) { }

                Logger.Info(gatewayProxy.IsStarted ? "Success." : "Failed.");
            }
        }
        public override void StopPlugin()
        {
            gatewayProxy.Stop();

            nodes = null;
            sensors = null;
        }
        #endregion

        #region DB actions
        public Setting GetSetting(string name)
        {
            return settings.FirstOrDefault(setting => setting.Name == name);
        }
        public Node GetNode(byte nodeID)
        {
            return nodes.FirstOrDefault(node => node.NodeNo == nodeID);
        }
        public Sensor GetSensor(byte nodeID, byte sensorID)
        {
            return sensors.FirstOrDefault(sensor => sensor.NodeNo == nodeID && sensor.SensorNo == sensorID);
        }

        public void Save(object item)
        {
            using (var session = Context.OpenSession())
            {
                session.Save(item);
                session.Flush();
            }
        }
        public void SaveOrUpdate(object item)
        {
            using (var session = Context.OpenSession())
            {
                session.SaveOrUpdate(item);
                session.Flush();
            }
        }
        #endregion


        #region Event handlers
        [OnTimerElapsed]
        private void OnTimerElapsed(DateTime now)
	    {
            //int a = 0;
            //int b = a;
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
                            nodes.Add(node);
                        }
                        else
                        {
                            node.Type = (SensorType)message.SubType;
                            node.ProtocolVersion = message.Payload;

                            SaveOrUpdate(node);
                        }
                        Run(OnSensorMessage, x => x(message));
                        //communicator.Broadcast(new NetworkMessage(NetworkMessageID.NodePresentation, JsonConvert.SerializeObject(node)));
                    }
                    else
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
                                sensors.Add(sensor);
                            }
                            else
                            {
                                sensor.Type = (SensorType)message.SubType;
                                sensor.ProtocolVersion = message.Payload;

                                SaveOrUpdate(sensor);
                            }
                            Run(OnSensorMessage, x => x(message));
                            //communicator.Broadcast(new NetworkMessage(NetworkMessageID.SensorPresentation, JsonConvert.SerializeObject(sensor)));
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
                            Value = float.Parse(message.Payload.Replace('.', ','))
                        };

                        Save(sv);
                        Run(OnSensorMessage, x => x(message));
                        //communicator.Broadcast(new NetworkMessage(NetworkMessageID.SensorValue, JsonConvert.SerializeObject(sv)));
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
                                Run(OnSensorMessage, x => x(message));
                                //communicator.Broadcast(new NetworkMessage(NetworkMessageID.BatteryLevel, JsonConvert.SerializeObject(bl)));
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
                                Run(OnSensorMessage, x => x(message));
                                //communicator.Broadcast(new NetworkMessage(NetworkMessageID.NodePresentation, JsonConvert.SerializeObject(node)));
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
        #endregion

        #region Private methods
        private void GetNextAvailableNodeID()
        {
            var nds = nodes.OrderBy(node => node.Id).ToList();

            byte id = 1;
            for (byte i = 0; i < nds.Count; i++)
                if (nds[i].NodeNo > i + 1)
                {
                    id = (byte)(i + 1);
                    break;
                }

            if (id < 255)
            {
                Node result = new Node { Id = Guid.NewGuid(), NodeNo = id };
                Save(result);
                nodes.Add(result);

                gatewayProxy.Send(new SensorMessage(255, 255, SensorMessageType.Internal, false, (byte)InternalValueType.IDResponse, id.ToString()));
            }
        }
        private int GetTimeForSensors() // seconds since 1970
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local); // from 1970/1/1 00:00:00 to now
            DateTime dtNow = DateTime.Now;
            TimeSpan result = dtNow.Subtract(dt);
            int seconds = Convert.ToInt32(result.TotalSeconds);
            return seconds;
        }
        #endregion

        //private List<AlarmTime> times;

        //private void LoadTimes()
        //{
        //    if (times == null)
        //    {
        //        using (var session = Context.OpenSession())
        //        {
        //            times = session.Query<AlarmTime>()
        //                .Fetch(a => a.UserScript)
        //                .Where(t => t.Enabled)
        //                .ToList();

        //            Logger.Info("loaded {0} alarm times", times.Count);
        //        }
        //    }
        //}
    }
}
