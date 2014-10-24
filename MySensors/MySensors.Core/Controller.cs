using MySensors.Core.Connectors;
using MySensors.Core.Messaging;
using MySensors.Core.Nodes;
using MySensors.Core.Services;
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

            if (!isNameServiceStarted)
            {
                if (ComponentStartEvent != null)
                    ComponentStartEvent(this, ControllerComponent.NameService, null);
                isNameServiceStarted = nameService.AddName("mysensors", NameService.NameType.Unique, NameService.MsSuffix.Default); // register on the local network
                if (ComponentStartEvent != null)
                    ComponentStartEvent(this, ControllerComponent.NameService, isNameServiceStarted);
            }





            return isConnectorStarted && isNameServiceStarted;// && isWebServerStarted;
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
            //Console.WriteLine(message.ToString());
            //Console.WriteLine();

            byte nodeID = message.NodeID;
            byte sensorID = message.SensorID;
            bool isSensor = sensorID != 255;

            switch (message.Type)
            {
                case MessageType.Presentation: // sent by a nodes when they present attached sensors.
                    SensorType type = (SensorType)message.SubType;
                    if (!isSensor)
                        GetNode(nodeID).Type = type;
                    else
                        GetSensor(nodeID, sensorID).Type = type;
                    break;

                case MessageType.Set: // sent from or to a sensor when a sensor value should be updated

                    break;

                case MessageType.Request: // requests a variable value (usually from an actuator destined for controller)
                    break;

                case MessageType.Internal: // special internal message
                    InternalValueType ivt = (InternalValueType)message.SubType;
                    switch (ivt)
                    {
                        case InternalValueType.SketchName:

                            break;
                        case InternalValueType.SketchVersion:

                            break;



                    }
                    break;

                case MessageType.Stream: //used for OTA firmware updates
                    break;

                default:
                    break;
            }
        }
        #endregion




        private void RegisterNode(byte nodeID)
        {

        }
    }
}
