using MySensors.Core.Connectors;
using MySensors.Core.Messaging;
using MySensors.Core.Nodes;
using MySensors.Core.Services.Data;
using MySensors.Core.Services.DNS;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace MySensors.Core
{
    public delegate void ControllerComponentStartEventHandler(Controller sender, string text, string textLine);

    public class Controller
    {
        #region Fields
        private DatabaseService dbService;
        private NameService nameService;
        private IGatewayConnector connector;
        //private WebServer webServer;

        private bool isDBServiceStarted = false;
        private bool isConnectorStarted = false;
        private bool isNameServiceStarted = false;
        private bool isWebServerStarted = false;

        private ObservableCollection<Node> nodes = new ObservableCollection<Node>();
        private ObservableCollection<Sensor> sensors = new ObservableCollection<Sensor>();
        #endregion

        #region Properties
        public IGatewayConnector GatewayConnector
        {
            get { return connector; }
        }
        #endregion

        #region Events
        public event ControllerComponentStartEventHandler ComponentStartEvent;
        #endregion

        #region Constructor
        public Controller(bool isSerial = true)
        {
            dbService = new DatabaseService();

            connector = isSerial ? (IGatewayConnector)new SerialGatewayConnector() : (IGatewayConnector)new EthernetGatewayConnector();
            connector.MessageReceived += connector_MessageReceived;

            nodes.CollectionChanged += nodes_CollectionChanged;
            sensors.CollectionChanged += sensors_CollectionChanged;

            nameService = new NameService();
            //webServer = new WebServer();
        }
        #endregion

        #region Public methods
        public bool Start()
        {
            if (!isDBServiceStarted)
            {
                if (ComponentStartEvent != null)
                    ComponentStartEvent(this, "Starting database... ", null);
                isDBServiceStarted = dbService.Start();
                if (ComponentStartEvent != null)
                    ComponentStartEvent(this, null, isDBServiceStarted ? "Success." : "Failed.");
            }

            //if (!isConnectorStarted)
            //{
            //    if (ComponentStartEvent != null)
            //        ComponentStartEvent(this, "Connecting to gateway... ", null);
            //    isConnectorStarted = connector.Connect();
            //    if (ComponentStartEvent != null)
            //        ComponentStartEvent(this, null, isConnectorStarted ? "Success." : "Failed.");
            //}

            //if (!isNameServiceStarted)
            //{
            //    if (ComponentStartEvent != null)
            //        ComponentStartEvent(this, "Starting name service... ", null);
            //    isNameServiceStarted = nameService.AddName("mysensors", NameService.NameType.Unique, NameService.MsSuffix.Default); // register on the local network
            //    if (ComponentStartEvent != null)
            //        ComponentStartEvent(this, null, isNameServiceStarted ? "Success." : "Failed.");
            //}





            return isDBServiceStarted;// && isConnectorStarted;// && isNameServiceStarted;// && isWebServerStarted;
        }
        public void Stop()
        {
            connector.Disconnect();

            isConnectorStarted = false;
            isNameServiceStarted = false;
            isWebServerStarted = false;
        }

        public Node GetNode(byte nodeID)
        {
            Node result = nodes.Where(node => node.ID == nodeID).FirstOrDefault();
            
            if (result == null)
            {
                result = new Node(nodeID);
                nodes.Add(result);
                dbService.Insert(result);
            }

            return result;
        }
        public Sensor GetSensor(byte nodeID, byte sensorID)
        {
            Sensor result = sensors.Where(sensor => sensor.NodeID == nodeID && sensor.ID == sensorID).FirstOrDefault();

            if (result == null)
            {
                result = new Sensor(nodeID, sensorID);
                sensors.Add(result);
                dbService.Insert(result);
            }

            return result;
        }
        #endregion

        #region Event handlers
        private void nodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
        }
        private void sensors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
        }
        private void connector_MessageReceived(IGatewayConnector sender, Message message)
        {
            Console.WriteLine();
            Console.WriteLine(message.ToString());

            bool isNode = message.NodeID == 0 || message.SensorID == 255;
            Node node = !isNode ? null : GetNode(message.NodeID);
            Sensor sensor = !isNode ? GetSensor(message.NodeID, message.SensorID) : null;

            switch (message.Type)
            {
                #region Presentation
                case MessageType.Presentation: // sent by a nodes when they present attached sensors.
                    if (node != null)
                    {
                        node.Type = (SensorType)message.SubType;
                        node.ProtocolVersion = message.Payload;
                        dbService.Update(node);
                    }
                    else
                    {
                        sensor.Type = (SensorType)message.SubType;
                        sensor.ProtocolVersion = message.Payload;
                        dbService.Update(sensor);
                    }
                    break;
                #endregion

                #region Set
                case MessageType.Set: // sent from or to a sensor when a sensor value should be updated
                    SensorValue sv = new SensorValue(message.NodeID, message.SensorID, DateTime.Now, (SensorValueType)message.SubType, float.Parse(message.Payload));
                    sensor.Values.Add(sv);
                    dbService.Insert(sv);
                    break;
                #endregion

                #region Request
                case MessageType.Request: // requests a variable value (usually from an actuator destined for controller)
                    break;
                #endregion

                #region Internal
                case MessageType.Internal: // special internal message
                    switch ((InternalValueType)message.SubType)
                    {
                        case InternalValueType.BatteryLevel: // int, in %
                            BatteryLevel bl = new BatteryLevel(message.NodeID, DateTime.Now, byte.Parse(message.Payload));
                            node.BatteryLevels.Add(bl);
                            dbService.Insert(bl);
                            break;
                        case InternalValueType.Time:
                            connector.Send(new Message(message.NodeID, message.SensorID, MessageType.Internal, false, (byte)InternalValueType.Time, DateTime.Now.TimeOfDay.ToString()));
                            break;
                        case InternalValueType.Version:
                            break;
                        case InternalValueType.IDRequest:
                            GetNextAvailableSensorID();
                            break;
                        case InternalValueType.IDResponse:
                            break;
                        case InternalValueType.InclusionMode:
                            break;
                        case InternalValueType.Config:
                            connector.Send(new Message(message.NodeID, 255, MessageType.Internal, false, (byte)InternalValueType.Config, "M")); //"M" or "I"
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
                            node.SketchName = message.Payload;
                            dbService.Update(node);
                            break;
                        case InternalValueType.SketchVersion:
                            node.SketchVersion = message.Payload;
                            dbService.Update(node);
                            break;
                        case InternalValueType.Reboot:
                            break;
                        case InternalValueType.GatewayReady:
                            break;
                    }
                    break;
                #endregion

                #region Stream
                case MessageType.Stream: //used for OTA firmware updates
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
            //    connector.Send(new Message(node.NodeID, 255, MessageType.Internal, false, (byte)InternalValueType.Reboot, ""));
        }
        #endregion

        #region Private methods
        private void GetNextAvailableSensorID()
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

                connector.Send(new Message(255, 255, MessageType.Internal, false, (byte)InternalValueType.IDResponse, id.ToString()));
            }
        }
        #endregion
    }
}
