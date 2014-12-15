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
using System.Threading;

namespace SmartNetwork.Plugins.MySensors
{
    [Plugin]
    public class MySensorsPlugin : PluginBase
    {
        #region Fields
        private bool isSerial = true;
        private IGatewayProxy gatewayProxy;
        private List<Node> nodes;
        private List<Sensor> sensors;
        #endregion

        [ImportMany("7CDDD153-64E0-4050-8533-C47C1BACBC6B")]
        public Action<SensorMessage>[] OnSensorMessage { get; set; }


        public override void InitDbModel(ModelMapper mapper)
        {
            mapper.Class<Node>(cfg => cfg.Table("MySensors_Nodes"));
            mapper.Class<Sensor>(cfg => cfg.Table("MySensors_Sensors"));
            mapper.Class<BatteryLevel>(cfg => cfg.Table("MySensors_BatteryLevels"));
            mapper.Class<SensorValue>(cfg => cfg.Table("MySensors_SensorValues"));
        }
        public override void InitPlugin()
        {
            //nodes = GetNodes();

            //using (var session = Context.OpenSession())
            //{
                //var nds = new ObservableCollection<Node>(dbService.GetAllNodes());
                //var bls = dbService.GetAllBatteryLevels();
                //var sensors = dbService.GetAllSensors();
                //var svs = dbService.GetAllSensorValues();
                //var stngs = dbService.GetAllSettings();
                //var mdls = dbService.GetAllModules();

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


            gatewayProxy = isSerial ? (IGatewayProxy)new SerialGatewayProxy() : (IGatewayProxy)new EthernetGatewayProxy();
            gatewayProxy.MessageReceived += gatewayProxy_MessageReceived;
        }
        public override void StartPlugin()
        {
            using (var session = Context.OpenSession())
            {
                nodes = session.Query<Node>().ToList();
                sensors = session.Query<Sensor>().ToList();
            }

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


        public Node GetNode(byte nodeID)
        {
            return nodes.FirstOrDefault(node => node.Id == nodeID);
        }
        public void SaveNode(Node node)
        {
            using (var session = Context.OpenSession())
            {
                session.SaveOrUpdate(node);
                session.Flush();
            }
        }
        public Sensor GetSensor(byte nodeID, byte sensorID)
        {
            return sensors.FirstOrDefault(sensor => sensor.NodeId == nodeID && sensor.Id == sensorID);
        }
        public void SaveSensor(Sensor sensor)
        {
            using (var session = Context.OpenSession())
            {
                session.SaveOrUpdate(sensor);
                session.Flush();
            }
        }
        public void SaveBatteryLevel(BatteryLevel bl)
        {
            using (var session = Context.OpenSession())
            {
                session.Save(bl);
                session.Flush();
            }
        }
        public void SaveSensorValue(SensorValue sv)
        {
            using (var session = Context.OpenSession())
            {
                session.Save(sv);
                session.Flush();
            }
        }

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
                                Id = message.NodeID,
                                Type = (SensorType)message.SubType,
                                ProtocolVersion = message.Payload
                            };
                            SaveNode(node);
                            nodes.Add(node);
                        }
                        else
                        {
                            node.Type = (SensorType)message.SubType;
                            node.ProtocolVersion = message.Payload;
                            SaveNode(node);
                        }
                        //communicator.Broadcast(new NetworkMessage(NetworkMessageID.NodePresentation, JsonConvert.SerializeObject(node)));
                        Run(OnSensorMessage, x => x(message));
                    }
                    else
                    {
                        if (node != null)
                        {
                            if (sensor == null)
                            {
                                sensor = new Sensor()
                                {
                                    NodeId = node.Id,
                                    Id = message.SensorID,
                                    Type = (SensorType)message.SubType,
                                    ProtocolVersion = message.Payload
                                };
                                SaveSensor(sensor);
                                sensors.Add(sensor);
                            }
                            else
                            {
                                sensor.Type = (SensorType)message.SubType;
                                sensor.ProtocolVersion = message.Payload;
                                SaveSensor(sensor);
                            }
                            //communicator.Broadcast(new NetworkMessage(NetworkMessageID.SensorPresentation, JsonConvert.SerializeObject(sensor)));
                            Run(OnSensorMessage, x => x(message));
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
                            NodeId = message.NodeID,
                            SensorId = message.SensorID,
                            TimeStamp = DateTime.Now,
                            Type = (SensorValueType)message.SubType,
                            Value = float.Parse(message.Payload.Replace('.', ','))
                        };
                        SaveSensorValue(sv);
                        //sensor.Values.Add(sv);
                        //communicator.Broadcast(new NetworkMessage(NetworkMessageID.SensorValue, JsonConvert.SerializeObject(sv)));
                        Run(OnSensorMessage, x => x(message));
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
                                    NodeId = node.Id,
                                    TimeStamp = DateTime.Now,
                                    Level = byte.Parse(message.Payload)
                                };
                                SaveBatteryLevel(bl);
                                //node.BatteryLevels.Add(bl);
                                //communicator.Broadcast(new NetworkMessage(NetworkMessageID.BatteryLevel, JsonConvert.SerializeObject(bl)));
                                Run(OnSensorMessage, x => x(message));
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
                            gatewayProxy.Send(new SensorMessage(message.NodeID, 255, SensorMessageType.Internal, false, (byte)InternalValueType.Config, "M"/*UnitSystem*/));
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

                                SaveNode(node);
                                //communicator.Broadcast(new NetworkMessage(NetworkMessageID.NodePresentation, JsonConvert.SerializeObject(node)));
                                Run(OnSensorMessage, x => x(message));
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
                if (nds[i].Id > i + 1)
                {
                    id = (byte)(i + 1);
                    break;
                }

            if (id < 255)
            {
                Node result = new Node { Id = id };
                SaveNode(result);
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
