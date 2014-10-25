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
            Node result = nodes.Where(node => node.NodeID == nodeID).FirstOrDefault();
            
            if (result == null)
            {
                result = new Node(nodeID);
                nodes.Add(result);
            }

            return result;
        }
        public Sensor GetSensor(byte nodeID, byte sensorID)
        {
            Sensor result = sensors.Where(sensor => sensor.NodeID == nodeID && sensor.SensorID == sensorID).FirstOrDefault();

            if (result == null)
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
                    }
                    else
                    {
                        sensor.Type = (SensorType)message.SubType;
                        sensor.ProtocolVersion = message.Payload;
                    }
                    break;
                #endregion

                #region Set
                case MessageType.Set: // sent from or to a sensor when a sensor value should be updated
                    //saveValue(message.NodeID, message.SensorID, (SensorValueType)message.SubType, message.Payload, db);
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
                        case InternalValueType.BatteryLevel:
                            //saveBatteryLevel(sender, payload, db);
                            break;
                        case InternalValueType.Time:
                            //sendTime(sender, sensor, gw);
                            break;
                        case InternalValueType.Version:
                            break;
                        case InternalValueType.IDRequest:
                            //sendNextAvailableSensorId(db, gw);
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
                            break;
                        case InternalValueType.SketchVersion:
                            node.SketchVersion = message.Payload;
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

    }
}
