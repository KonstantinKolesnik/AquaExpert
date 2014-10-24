using MySensors.Core.Connectors;
using MySensors.Core.Messaging;
using MySensors.Core.Nodes;
using MySensors.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MySensors.Core
{
    public delegate void ControllerComponentStartEventHandler(Controller sender, ControllerComponent component, bool? result);

    public class Controller
    {
        #region Fields
        private NameService nameService;
        private IGatewayConnector connector;
        //private WebServer webServer;

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
            connector = isSerial ? (IGatewayConnector)new SerialGatewayConnector() : (IGatewayConnector)new EthernetGatewayConnector();
            connector.MessageReceived += connector_MessageReceived;

            nameService = new NameService();
            //webServer = new WebServer();
        }
        #endregion

        #region Public methods
        public bool Start()
        {
            if (!isConnectorStarted)
            {
                if (ComponentStartEvent != null)
                    ComponentStartEvent(this, ControllerComponent.GatewayConnector, null);
                isConnectorStarted = connector.Connect();
                if (ComponentStartEvent != null)
                    ComponentStartEvent(this, ControllerComponent.GatewayConnector, isConnectorStarted);
            }

            //if (!isNameServiceStarted)
            //{
            //    if (ComponentStartEvent != null)
            //        ComponentStartEvent(this, ControllerComponent.NameService, null);
            //    isNameServiceStarted = nameService.AddName("mysensors", NameService.NameType.Unique, NameService.MsSuffix.Default); // register on the local network
            //    if (ComponentStartEvent != null)
            //        ComponentStartEvent(this, ControllerComponent.NameService, isNameServiceStarted);
            //}





            return isConnectorStarted;// && isNameServiceStarted;// && isWebServerStarted;
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
            Node result;
            var res = nodes.Where(node => node.NodeID == nodeID);
            if (res.Any())
                result = res.First();
            else
            {
                result = new Node(nodeID);
                nodes.Add(result);
            }

            return result;
        }
        public Sensor GetSensor(byte nodeID, byte sensorID)
        {
            Sensor result;
            var res = sensors.Where(sensor => sensor.NodeID == nodeID && sensor.SensorID == sensorID);
            if (res.Any())
                result = res.First();
            else
            {
                result = new Sensor(nodeID, sensorID);
                sensors.Add(result);
            }

            return result;
        }
        #endregion

        #region Event handlers
        private void connector_MessageReceived(IGatewayConnector sender, Message message)
        {
            Console.WriteLine();
            Console.WriteLine(message.ToString());
            //Console.WriteLine();

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
                        node.IsAckNeeded = message.IsAckNeeded;
                        node.Version = message.Payload;
                    }
                    else
                    {
                        sensor.Type = (SensorType)message.SubType;
                        sensor.IsAckNeeded = message.IsAckNeeded;
                        sensor.Version = message.Payload;
                    }
                    break;
                #endregion

                #region Set
                case MessageType.Set: // sent from or to a sensor when a sensor value should be updated

                    break;
                #endregion

                #region Request
                case MessageType.Request: // requests a variable value (usually from an actuator destined for controller)
                    break;
                #endregion

                #region Internal
                case MessageType.Internal: // special internal message
                    //BatteryLevel =              0,      // Use this to report the battery level (in percent 0-100).
                    //Time =                      1,      // Sensors can request the current time from the Controller using this message. The time will be reported as the seconds since 1970
                    //Version =                   2,      // Sensors report their library version at startup using this message type
                    //IDRequest =             	3,      // Use this to request a unique node id from the controller.
                    //IDResponse =                4,      // Id response back to sensor. Payload contains sensor id.
                    //InclusionMode =             5,      // Start/stop inclusion mode of the Controller (1=start, 0=stop).
                    //Config =                    6,      // Config request from node. Reply with (M)etric or (I)mperal back to sensor.
                    //FindParent =                7,      // When a sensor starts up, it broadcast a search request to all neighbor nodes. They reply with a FindParentResponse.
                    //FindParentResponse =        8,      // Reply message type to I_FIND_PARENT request.
                    //LogMessage =                9,      // Sent by the gateway to the Controller to trace-log a message
                    //Children =                  10,     // A message that can be used to transfer child sensors (from EEPROM routing table) of a repeating node.
                    //SketchName =                11,     // Optional sketch name that can be used to identify sensor in the Controller GUI
                    //SketchVersion =             12,     // Optional sketch version that can be reported to keep track of the version of sensor in the Controller GUI.
                    //Reboot =                    13,     // Used by OTA firmware updates. Request for node to reboot.
                    //GatewayReady =              14,     // Send by gateway to controller when startup is complete.
                
                
                    switch ((InternalValueType)message.SubType)
                    {
                        case InternalValueType.SketchName:
                            node.SketchName = message.Payload;
                            break;
                        case InternalValueType.SketchVersion:
                            node.SketchVersion = message.Payload;
                            break;
                        case InternalValueType.GatewayReady:

                            break;
                        case InternalValueType.Config:
                            message.Payload = "I";
                            connector.Send(message);
                            break;

                    }
                    break;
                #endregion

                #region Stream
                case MessageType.Stream: //used for OTA firmware updates
                    break;
                #endregion

                default:
                    break;
            }
        }
        #endregion




        //private void RegisterNode(byte nodeID)
        //{

        //}
    }
}
