﻿using MySensors.Controllers.Automation;
using MySensors.Controllers.Communication;
using MySensors.Controllers.Data;
using MySensors.Controllers.GatewayProxies;
using MySensors.Controllers.Scripting;
using MySensors.Controllers.Scripting.Compilers;
using MySensors.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using System.Text;

namespace MySensors.Controllers
{
    public class Controller
    {
        #region Fields
        private DatabaseService dbService;
        private IGatewayProxy gatewayProxy;
        private Communicator communicator;
        private ScriptingEngine scriptEngine;

        private ObservableCollection<Node> nodes = new ObservableCollection<Node>();
        private ObservableCollection<Setting> settings = new ObservableCollection<Setting>();
        private ObservableCollection<AutomationModule> modules = new ObservableCollection<AutomationModule>();
        #endregion

        #region Properties
        public bool IsStarted
        {
            get
            {
                return dbService.IsStarted && communicator.IsStarted;// && gatewayProxy.IsStarted;
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
            communicator.NetworkMessageProcessor += communicator_NetworkMessageProcessor;

            scriptEngine = new ScriptingEngine(new CSharpCompiler());
        }
        #endregion

        #region Public methods
        public bool Start()
        {
            StartDatabase();
            //StartGatewayProxy();
            StartCommunicator();

            return IsStarted;
        }
        public void Stop()
        {
            communicator.Stop();
            //gatewayProxy.Stop();
            dbService.Stop();
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
                            nodes.Add(node);
                            dbService.Insert(node);
                        }
                        else
                        {
                            node.Type = (SensorType)message.SubType;
                            node.ProtocolVersion = message.Payload;
                            dbService.Update(node);
                        }
                        communicator.Broadcast(CreateNodePresentationMessage(node));
                    }
                    else
                    {
                        if (sensor == null)
                        {
                            sensor = new Sensor(message.NodeID, message.SensorID, (SensorType)message.SubType, message.Payload);
                            node.Sensors.Add(sensor);
                            dbService.Insert(sensor);
                        }
                        else
                        {
                            sensor.Type = (SensorType)message.SubType;
                            sensor.ProtocolVersion = message.Payload;
                            dbService.Update(sensor);
                        }
                        communicator.Broadcast(CreateSensorPresentationMessage(sensor));
                    }
                    break;
                #endregion

                #region Set
                case SensorMessageType.Set: // sent from or to a sensor when a sensor value should be updated
                    if (sensor != null)
                    {
                        SensorValue sv = new SensorValue(message.NodeID, message.SensorID, DateTime.Now, (SensorValueType)message.SubType, float.Parse(message.Payload.Replace('.', ',')));
                        sensor.Values.Add(sv);
                        dbService.Insert(sv);
                        communicator.Broadcast(CreateSensorValueMessage(sv));
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
                                node.BatteryLevels.Add(bl);
                                dbService.Insert(bl);
                                communicator.Broadcast(CreateNodeBatteryLevelMessage(bl));
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
                                communicator.Broadcast(CreateNodePresentationMessage(node));
                            }
                            break;
                        case InternalValueType.SketchVersion:
                            if (node != null)
                            {
                                node.SketchVersion = message.Payload;
                                dbService.Update(node);
                                communicator.Broadcast(CreateNodePresentationMessage(node));
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
        private NetworkMessage communicator_NetworkMessageProcessor(NetworkMessage request)
        {
            return ProcessNetworkMessage(request);
        }
        #endregion

        #region Private methods
        private Node GetNode(byte nodeID)
        {
            return nodes.Where(node => node.ID == nodeID).FirstOrDefault();
        }
        private Sensor GetSensor(byte nodeID, byte sensorID)
        {
            Node node = GetNode(nodeID);
            if (node == null)
                return null;

            return node.Sensors.Where(sensor => sensor.NodeID == nodeID && sensor.ID == sensorID).FirstOrDefault();
        }
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

                    settings.Clear();
                    foreach (var stng in stngs)
                        settings.Add(stng);

                    modules.Clear();
                    foreach (var mdl in mdls)
                        modules.Add(mdl);
                }

                if (Log != null)
                    Log(this, dbService.IsStarted ? "Success." : "Failed.", true, dbService.IsStarted ? LogLevel.Success : LogLevel.Error);
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
            var query = nodes.OrderBy(node => node.ID).ToList();

            byte id = 1;
            for (byte i = 1; i <= query.Count; i++)
                if (query[i].ID > i)
                {
                    id = i;
                    break;
                }

            if (id < 255)
            {
                Node result = new Node(id);
                nodes.Add(result);
                dbService.Insert(result);

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

        #region Network message processing
        private NetworkMessage ProcessNetworkMessage(NetworkMessage msg)
        {
            NetworkMessage response = null;

            if (msg != null)
                switch (msg.ID)
                {
                    #region Settings
                    case NetworkMessageID.Settings:
                        if (msg.ParametersCount == 0) // client asks for settings
                            response = CreateSettingsMessage();
                        else // client sets options
                        {
                            WebTheme = msg["WebTheme"];
                            UnitSystem = msg["UnitSystem"];

                            communicator.Broadcast(CreateSettingsMessage());
                        }
                        break;
                    #endregion

                    #region Version
                    case NetworkMessageID.Version:
                        if (msg.ParametersCount == 0) // client asks for version
                            response = CreateVersionMessage();
                        break;
                    #endregion

                    #region GetNodes
                    case NetworkMessageID.GetNodes:
                        if (msg.ParametersCount == 0)
                            response = CreateGetNodesMessage();
                        break;
                    #endregion

                    #region GetModules
                    case NetworkMessageID.GetModules:
                        if (msg.ParametersCount == 0)
                            response = CreateGetModulesMessage();
                        break;
                    #endregion

                    #region AddModule
                    case NetworkMessageID.AddModule:
                        AutomationModule module = new AutomationModule("Untitled", "", "");
                        dbService.Insert(module);
                        modules.Add(module);
                        communicator.Broadcast(CreateGetModulesMessage());
                        break;
                    #endregion

                    #region SetModule
                    case NetworkMessageID.SetModule:
                        if (msg.ParametersCount == 1)
                        {
                            dynamic obj = JsonConvert.DeserializeObject(msg["Module"]);
                            //foreach (dynamic item in obj.Metadata)
                            //    convertedObject.MetaData[i++].Id = Guid.Parse(item.Id.ToString());

                            AutomationModule m = GetModule(Guid.Parse(obj.ID.ToString()));
                            m.Name = obj.Name;
                            m.Description = obj.Description;
                            string s = obj.Script;
                            m.Script = new string(Encoding.ASCII.GetChars(Convert.FromBase64String(s)));

                            dbService.Update(m);

                            communicator.Broadcast(CreateGetModulesMessage());
                        }
                        break;
                    #endregion

                    #region Sensor message
                    case NetworkMessageID.SensorMessage:
                        gatewayProxy.Send(SensorMessage.FromRawMessage(msg["Msg"].Replace("-", ";")));
                        break;
                    #endregion

                    default: break;
                }

            return response;
        }

        private NetworkMessage CreateMsgMessage(string text, string type)
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.Message);
            msg["Msg"] = text;
            msg["Type"] = type;
            return msg;
        }
        private NetworkMessage CreateSettingsMessage()
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.Settings);
            msg["WebTheme"] = WebTheme;
            msg["UnitSystem"] = UnitSystem;
            return msg;
        }
        private NetworkMessage CreateVersionMessage()
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.Version);
            msg["Version"] = Version;
            return msg;
        }
        private NetworkMessage CreateGetNodesMessage()
        {
            //List<Node> coll = new List<Node>();
            //for (byte i = 20; i < 50; i++)
            //{
            //    Node node = new Node(i)
            //    {
            //        Type = SensorType.ArduinoNode,
            //        ProtocolVersion = "1.4",
            //        SketchName = "Sketch " + i,
            //        SketchVersion = "1.0"
            //    };

            //    for (byte p = 100; p > 80; p--)
            //        node.BatteryLevels.Add(new BatteryLevel(node.ID, DateTime.Now.AddHours(100 - p), p));

            //    for (byte j = 0; j < 25; j++)
            //    {
            //        Sensor sensor = new Sensor(node.ID, j) { Type = (SensorType)j, ProtocolVersion = "1.4" };

            //        for (byte p = 0; p < 15; p++)
            //            sensor.Values.Add(new SensorValue(node.ID, j, DateTime.Now.AddHours(p), (SensorValueType)p, p));

            //        node.Sensors.Add(sensor);
            //    }

            //    coll.Add(node);
            //}
            //NetworkMessage msg = new NetworkMessage(NetworkMessageID.GetNodes);
            //msg["Nodes"] = JsonConvert.SerializeObject(coll, Formatting.Indented);
            //return msg;


            NetworkMessage msg = new NetworkMessage(NetworkMessageID.GetNodes);
            msg["Nodes"] = JsonConvert.SerializeObject(nodes, Formatting.Indented);
            return msg;
        }
        private NetworkMessage CreateGetModulesMessage()
        {
            var obj = modules.Select(module => new {
                ID = module.ID,
                Name = module.Name,
                Description = module.Description,
                Script = Convert.ToBase64String(Encoding.ASCII.GetBytes(module.Script))
            });

            NetworkMessage msg = new NetworkMessage(NetworkMessageID.GetModules);
            msg["Modules"] = JsonConvert.SerializeObject(obj);
            return msg;
        }

        private NetworkMessage CreateNodePresentationMessage(Node node)
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.NodePresentation);
            msg["Node"] = JsonConvert.SerializeObject(node);
            return msg;
        }
        private NetworkMessage CreateSensorPresentationMessage(Sensor sensor)
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.SensorPresentation);
            msg["Sensor"] = JsonConvert.SerializeObject(sensor);
            return msg;
        }
        private NetworkMessage CreateNodeBatteryLevelMessage(BatteryLevel bl)
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.BatteryLevel);
            msg["Level"] = JsonConvert.SerializeObject(bl);
            return msg;
        }
        private NetworkMessage CreateSensorValueMessage(SensorValue sv)
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.SensorValue);
            msg["Value"] = JsonConvert.SerializeObject(sv);
            return msg;
        }
        #endregion
    }
}
