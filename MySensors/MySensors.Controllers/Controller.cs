using MySensors.Controllers.Automation;
using MySensors.Controllers.Communication;
using MySensors.Controllers.Core;
using MySensors.Controllers.Data;
using MySensors.Controllers.GatewayProxies;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MySensors.Controllers
{
    public class Controller
    {
        #region Fields
        private DatabaseService dbService;
        private IGatewayProxy gatewayProxy;
        private Communicator communicator;

        private ObservableCollection<Node> nodes = new ObservableCollection<Node>();
        private ObservableCollection<Setting> settings = new ObservableCollection<Setting>();
        private ObservableCollection<AutomationModule> modules = new ObservableCollection<AutomationModule>();
        #endregion

        #region Properties
        public bool IsStarted
        {
            get
            {
                return dbService.IsStarted && communicator.IsStarted && gatewayProxy.IsStarted;
            }
        }

        public string WebTheme
        {
            get
            {
                var item = settings.Where(s => s.Name == "WebTheme").FirstOrDefault();
                if (item == null)
                {
                    item = new Setting("WebTheme", "default");
                    dbService.Insert(item);
                }

                return item.Value;
            }
            private set
            {
                var item = settings.Where(s => s.Name == "WebTheme").FirstOrDefault();
                if (item == null)
                {
                    item = new Setting("WebTheme", "default");
                    dbService.Insert(item);
                }

                if (item.Value != value)
                {
                    item.Value = value;
                    dbService.Update(item);
                }
            }
        }
        public string UnitSystem //"M" or "I"
        {
            get
            {
                var item = settings.Where(s => s.Name == "UnitSystem").FirstOrDefault();
                if (item == null)
                {
                    item = new Setting("UnitSystem", "M");
                    dbService.Insert(item);
                }

                return item.Value;
            }
            private set
            {
                var item = settings.Where(s => s.Name == "UnitSystem").FirstOrDefault();
                if (item == null)
                {
                    item = new Setting("UnitSystem", "M");
                    dbService.Insert(item);
                }

                if (item.Value != value)
                {
                    item.Value = value;
                    dbService.Update(item);
                }
            }
        }
        public string Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }
        #endregion

        #region Events
        public event LogEventHandler Log;
        #endregion

        #region Constructor
        public Controller(bool isSerial = true)
        {
            dbService = new DatabaseService();

            gatewayProxy = isSerial ? (IGatewayProxy)new SerialGatewayProxy() : (IGatewayProxy)new EthernetGatewayProxy();
            gatewayProxy.MessageReceived += gatewayProxy_MessageReceived;

            communicator = new Communicator();
            communicator.NetworkMessageProcessor += communicator_MessageReceived;
        }
        #endregion

        #region Public methods
        public bool Start()
        {
            if (!IsStarted)
            {
                StartDatabase();
                StartGatewayProxy();
                StartCommunicator();
            }

            return IsStarted;
        }
        public void Stop()
        {
            if (IsStarted)
            {
                communicator.Stop();
                gatewayProxy.Stop();
                dbService.Stop();
            }
        }

        public Node GetNode(byte nodeID)
        {
            return nodes.Where(node => node.ID == nodeID).FirstOrDefault();
        }
        public Sensor GetSensor(byte nodeID, byte sensorID)
        {
            Node node = GetNode(nodeID);
            if (node == null)
                return null;

            return node.Sensors.Where(sensor => sensor.NodeID == nodeID && sensor.ID == sensorID).FirstOrDefault();
        }

        public void SetSensorValue(Sensor sensor, SensorValueType valueType, object value)
        {
            if (sensor != null && value != null)
                gatewayProxy.Send(new SensorMessage(sensor.NodeID, sensor.ID, SensorMessageType.Set, false, (byte)valueType, value.ToString()));
        }
        #endregion

        #region Event handlers
        private void gatewayProxy_MessageReceived(IGatewayProxy sender, SensorMessageEventArgs args)
        {
            if (!IsStarted)
                return;

            SensorMessage message = args.Message;

            // for debug!!!
            if (Log != null)
                Log(this, message.ToString(), true, LogLevel.Normal);

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
                            node = new Node(message.NodeID, (SensorType)message.SubType, message.Payload);
                            dbService.Insert(node);
                            nodes.Add(node);
                        }
                        else
                        {
                            node.Type = (SensorType)message.SubType;
                            node.ProtocolVersion = message.Payload;
                            dbService.Update(node);
                        }
                        communicator.Broadcast(new NetworkMessage(NetworkMessageID.NodePresentation, JsonConvert.SerializeObject(node)));
                    }
                    else
                    {
                        if (sensor == null)
                        {
                            sensor = new Sensor(message.NodeID, message.SensorID, (SensorType)message.SubType, message.Payload);
                            dbService.Insert(sensor);
                            node.Sensors.Add(sensor);
                        }
                        else
                        {
                            sensor.Type = (SensorType)message.SubType;
                            sensor.ProtocolVersion = message.Payload;
                            dbService.Update(sensor);
                        }
                        communicator.Broadcast(new NetworkMessage(NetworkMessageID.SensorPresentation, JsonConvert.SerializeObject(sensor)));
                    }
                    break;
                #endregion

                #region Set
                case SensorMessageType.Set: // sent from or to a sensor when a sensor value should be updated
                    if (sensor != null)
                    {
                        SensorValue sv = new SensorValue(message.NodeID, message.SensorID, DateTime.Now, (SensorValueType)message.SubType, float.Parse(message.Payload.Replace('.', ',')));
                        dbService.Insert(sv);
                        sensor.Values.Add(sv);
                        communicator.Broadcast(new NetworkMessage(NetworkMessageID.SensorValue, JsonConvert.SerializeObject(sv)));
                    }
                    break;
                #endregion

                #region Request
                case SensorMessageType.Request: // requests a variable value (usually from an actuator destined for controller)
                    break;
                #endregion

                #region Internal
                case SensorMessageType.Internal: // special internal message
                    switch ((InternalValueType)message.SubType)
                    {
                        case InternalValueType.BatteryLevel: // int, in %
                            if (node != null)
                            {
                                BatteryLevel bl = new BatteryLevel(node.ID, DateTime.Now, byte.Parse(message.Payload));
                                dbService.Insert(bl);
                                node.BatteryLevels.Add(bl);
                                communicator.Broadcast(new NetworkMessage(NetworkMessageID.BatteryLevel, JsonConvert.SerializeObject(bl)));
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
                            gatewayProxy.Send(new SensorMessage(message.NodeID, 255, SensorMessageType.Internal, false, (byte)InternalValueType.Config, UnitSystem));
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
                            if (node != null)
                            {
                                node.SketchName = message.Payload;
                                dbService.Update(node);
                                communicator.Broadcast(new NetworkMessage(NetworkMessageID.NodePresentation, JsonConvert.SerializeObject(node)));
                            }
                            break;
                        case InternalValueType.SketchVersion:
                            if (node != null)
                            {
                                node.SketchVersion = message.Payload;
                                dbService.Update(node);
                                communicator.Broadcast(new NetworkMessage(NetworkMessageID.NodePresentation, JsonConvert.SerializeObject(node)));
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
        private NetworkMessage communicator_MessageReceived(NetworkMessage request)
        {
            NetworkMessage response = null;

            if (request != null)
                switch (request.ID)
                {
                    #region Settings
                    case NetworkMessageID.Settings:
                        if (request.Parameters.Count == 0)
                            response = new NetworkMessage(NetworkMessageID.Settings, WebTheme, UnitSystem);
                        else
                        {
                            WebTheme = request[0];
                            UnitSystem = request[1];

                            communicator.Broadcast(new NetworkMessage(NetworkMessageID.Settings, WebTheme, UnitSystem));
                        }
                        break;
                    #endregion

                    #region Version
                    case NetworkMessageID.Version: response = new NetworkMessage(NetworkMessageID.Version, Version); break;
                    #endregion

                    #region GetNodes
                    case NetworkMessageID.GetNodes: response = new NetworkMessage(NetworkMessageID.GetNodes, JsonConvert.SerializeObject(nodes, Formatting.Indented)); break;
                    #endregion

                    #region DeleteNode
                    case NetworkMessageID.DeleteNode:
                        if (request.Parameters.Count == 1)
                        {
                            Node node = GetNode(byte.Parse(request[0]));
                            dbService.Delete(node);
                            nodes.Remove(node);
                            communicator.Broadcast(new NetworkMessage(NetworkMessageID.DeleteNode, node.ID.ToString()));
                        }
                        break;
                    #endregion

                    #region GetModules
                    case NetworkMessageID.GetModules: response = new NetworkMessage(NetworkMessageID.GetModules, SerializeModules()); break;
                    #endregion

                    #region AddModule
                    case NetworkMessageID.AddModule:
                        AutomationModule newModule = new AutomationModule("Untitled", "", "", "");
                        dbService.Insert(newModule);
                        modules.Add(newModule);
                        response = new NetworkMessage(NetworkMessageID.AddModule, SerializeModules(newModule.ID));
                        break;
                    #endregion

                    #region SetModule
                    case NetworkMessageID.SetModule:
                        if (request.Parameters.Count == 1)
                        {
                            dynamic obj = JsonConvert.DeserializeObject(request[0]);
                            string s;

                            AutomationModule module = GetModule(Guid.Parse(obj.ID.ToString()));
                            module.Name = obj.Name;
                            module.Description = obj.Description;
                            s = obj.Script;
                            module.Script = new string(Encoding.ASCII.GetChars(Convert.FromBase64String(s)));
                            s = obj.View;
                            module.View = new string(Encoding.ASCII.GetChars(Convert.FromBase64String(s)));

                            dbService.Update(module);
                            communicator.Broadcast(new NetworkMessage(NetworkMessageID.SetModule, SerializeModules(module.ID)));

                            string errors = RunAutomationService(module);
                            if (!string.IsNullOrEmpty(errors))
                                response = new NetworkMessage(NetworkMessageID.Message, errors, "Error");
                        }
                        break;
                    #endregion

                    #region DeleteModule
                    case NetworkMessageID.DeleteModule:
                        if (request.Parameters.Count == 1)
                        {
                            AutomationModule module = GetModule(Guid.Parse(request[0]));
                            dbService.Delete(module);
                            modules.Remove(module);
                            communicator.Broadcast(new NetworkMessage(NetworkMessageID.DeleteModule, module.ID.ToString()));
                        }
                        break;
                    #endregion

                    #region Sensor message (client sets actuator value)
                    case NetworkMessageID.SensorMessage: gatewayProxy.Send(SensorMessage.FromRawMessage(request[0].Replace("-", ";"))); break;
                    #endregion

                    default: break;
                }

            return response;
        }
        #endregion

        #region Private methods
        private AutomationModule GetModule(Guid id)
        {
            return modules.Where(module => module.ID == id).FirstOrDefault();
        }

        private void StartDatabase()
        {
            if (!dbService.IsStarted)
            {
                if (Log != null)
                    Log(this, "Starting database... ", false, LogLevel.Normal);

                try
                {
                    dbService.Start();
                }
                catch (Exception) { }


                if (dbService.IsStarted)
                {
                    var nds = new ObservableCollection<Node>(dbService.GetAllNodes());
                    var bls = dbService.GetAllBatteryLevels();
                    var sensors = dbService.GetAllSensors();
                    var svs = dbService.GetAllSensorValues();
                    var stngs = dbService.GetAllSettings();
                    var mdls = dbService.GetAllModules();

                    nodes.Clear();
                    foreach (var node in nds)
                    {
                        nodes.Add(node);

                        node.BatteryLevels.Clear();
                        foreach (var bl in bls.Where(bl => bl.NodeID == node.ID))
                            node.BatteryLevels.Add(bl);

                        node.Sensors.Clear();
                        foreach (var sensor in sensors.Where(sensor => sensor.NodeID == node.ID))
                        {
                            node.Sensors.Add(sensor);

                            sensor.Values.Clear();
                            foreach (var sv in svs.Where(sv => sv.NodeID == sensor.NodeID && sv.ID == sensor.ID))
                                sensor.Values.Add(sv);
                        }
                    }
                    // for test!!!
                    //for (byte i = 20; i < 30; i++)
                    //{
                    //    Node node = new Node(i)
                    //    {
                    //        Type = SensorType.ArduinoNode,
                    //        ProtocolVersion = "1.4.1",
                    //        SketchName = "Sketch " + i,
                    //        SketchVersion = "1.0"
                    //    };

                    //    for (byte p = 100; p >= 80; p--)
                    //        node.BatteryLevels.Add(new BatteryLevel(node.ID, DateTime.Now.AddHours(-p), p));
                    //    node.BatteryLevels.Add(new BatteryLevel(node.ID, DateTime.Now, 0));

                    //    for (byte j = 0; j < 25; j++)
                    //    {
                    //        Sensor sensor = new Sensor(node.ID, j)
                    //        {
                    //            Type = (SensorType)j,
                    //            ProtocolVersion = "1.4.1"
                    //        };

                    //        for (byte p = 0; p < 15; p++)
                    //            sensor.Values.Add(new SensorValue(node.ID, j, DateTime.Now.AddHours(p), (SensorValueType)p, p));

                    //        node.Sensors.Add(sensor);
                    //    }

                    //    nodes.Add(node);
                    //}


                    settings.Clear();
                    foreach (var stng in stngs)
                        settings.Add(stng);

                    modules.Clear();
                    foreach (var m in mdls)
                        modules.Add(m);
                }

                if (Log != null)
                    Log(this, dbService.IsStarted ? "Success." : "Failed.", true, dbService.IsStarted ? LogLevel.Success : LogLevel.Error);


                //foreach (var m in modules)
                //    RunAutomationService(m);
            }
        }
        private void StartCommunicator()
        {
            if (!communicator.IsStarted)
            {
                if (Log != null)
                    Log(this, "Starting communicator... ", false, LogLevel.Normal);

                try
                {
                    communicator.Start(8080, 12000);
                }
                catch (Exception) { }

                if (Log != null)
                    Log(this, communicator.IsStarted ? "Success." : "Failed.", true, communicator.IsStarted ? LogLevel.Success : LogLevel.Error);
            }
        }
        private void StartGatewayProxy()
        {
            if (!gatewayProxy.IsStarted)
            {
                if (Log != null)
                    Log(this, "Connecting to gateway... ", false, LogLevel.Normal);

                try
                {
                    gatewayProxy.Start();
                }
                catch (Exception) { }

                if (Log != null)
                    Log(this, gatewayProxy.IsStarted ? "Success." : "Failed.", true, gatewayProxy.IsStarted ? LogLevel.Success : LogLevel.Error);
            }
        }

        private void GetNextAvailableNodeID()
        {
            var nds = nodes.OrderBy(node => node.ID).ToList();

            byte id = 1;
            for (byte i = 0; i < nds.Count; i++)
                if (nds[i].ID > i + 1)
                {
                    id = (byte)(i + 1);
                    break;
                }

            if (id < 255)
            {
                Node result = new Node(id);
                dbService.Insert(result);
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

        private string RunAutomationService(AutomationModule module)
        {
            string errors = module.StartService(this);
            if (!string.IsNullOrEmpty(errors) && Log != null)
                Log(this, errors, true, LogLevel.Error);

            return errors;
        }

        private string SerializeModules(Guid? moduleID = null)
        {
            object obj;

            if (moduleID.HasValue)
            {
                obj = modules.Where(module => module.ID == moduleID.Value).Select(module => new
                {
                    ID = module.ID,
                    Name = module.Name,
                    Description = module.Description,
                    Script = Convert.ToBase64String(Encoding.ASCII.GetBytes(module.Script)),
                    View = Convert.ToBase64String(Encoding.ASCII.GetBytes(module.View))
                });
            }
            else
            {
                obj = modules.Select(module => new
                {
                    ID = module.ID,
                    Name = module.Name,
                    Description = module.Description,
                    Script = Convert.ToBase64String(Encoding.ASCII.GetBytes(module.Script)),
                    View = Convert.ToBase64String(Encoding.ASCII.GetBytes(module.View))
                });
            }

            return JsonConvert.SerializeObject(obj);
        }
        #endregion
    }
}
