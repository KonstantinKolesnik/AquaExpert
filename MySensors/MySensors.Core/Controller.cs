using Griffin.WebServer;
using Griffin.WebServer.Files;
using MySensors.Core.Connectors;
using MySensors.Core.Messaging;
using MySensors.Core.Nodes;
using MySensors.Core.Services.Data;
using MySensors.Core.Services.DNS;
using MySensors.Core.Services.Web;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace MySensors.Core
{
    public delegate void LogEventHandler(Controller sender, string text, string textLine);

    public class Controller
    {
        #region Fields
        private DatabaseService dbService;
        //private NameService nameService;
        private IGatewayConnector connector;
        //private WebServer webServer;

        private bool isDBServiceStarted = false;
        private bool isConnectorStarted = false;
        //private bool isNameServiceStarted = false;
        private bool isWebServerStarted = false;

        private ObservableCollection<Node> nodes = new ObservableCollection<Node>();
        private ObservableCollection<Sensor> sensors = new ObservableCollection<Sensor>();
        #endregion

        #region Properties
        public bool IsStarted
        {
            get
            {
                return isDBServiceStarted && isConnectorStarted && isWebServerStarted;
            }
        }
        #endregion

        #region Events
        public event LogEventHandler Log;
        #endregion

        #region Constructor
        public Controller(bool isSerial = true)
        {
            dbService = new DatabaseService();

            connector = isSerial ? (IGatewayConnector)new SerialGatewayConnector() : (IGatewayConnector)new EthernetGatewayConnector();
            connector.MessageReceived += connector_MessageReceived;

            nodes.CollectionChanged += nodes_CollectionChanged;
            sensors.CollectionChanged += sensors_CollectionChanged;

            //nameService = new NameService();
            //webServer = new WebServer();
        }
        #endregion

        #region Public methods
        public bool Start()
        {
            // start database service:
            if (!isDBServiceStarted)
            {
                if (Log != null)
                    Log(this, "Starting database... ", null);
                
                isDBServiceStarted = dbService.Start();
                
                if (isDBServiceStarted)
                {
                    nodes = new ObservableCollection<Node>(dbService.GetAllNodes());
                    sensors = new ObservableCollection<Sensor>(dbService.GetAllSensors());

                    var bls = dbService.GetAllBatteryLevels();
                    foreach (var node in nodes)
                        node.BatteryLevels = new ObservableCollection<BatteryLevel>(bls.Where(bl => bl.NodeID == node.ID));

                    var svs = dbService.GetAllSensorValues();
                    foreach (var sensor in sensors)
                        sensor.Values = new ObservableCollection<SensorValue>(svs.Where(sv => sv.NodeID == sensor.NodeID && sv.ID == sensor.ID));
                }
                
                if (Log != null)
                    Log(this, null, isDBServiceStarted ? "Success." : "Failed.");
            }

            // start web-server service:
            if (!isWebServerStarted)
            {
                if (Log != null)
                    Log(this, "Starting web server... ", null);

                try
                {
                    var moduleManager = new ModuleManager();

                    // Let's serve our downloaded files (Windows 7 users)
                    //var fileService = new DiskFileService("/", string.Format(@"C:\Users\{0}\Downloads", Environment.UserName));
                    //var fileService = new DiskFileService("/", @"D:\Work\telerik.kendoui.professional.2014.2.716.commercial\examples\");
                    var fileService = new DiskFileService("/", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\");



                    // Create the file module and allow files to be listed.
                    var module = new FileModule(fileService) { ListFiles = false };

                    // Add the module
                    moduleManager.Add(new RootModule());
                    moduleManager.Add(module);
                    //moduleManager.Add(new BodyDecodingModule(new UrlFormattedDecoder()));

                    // And start the server.
                    var server = new HttpServer(moduleManager);
                    server.Start(IPAddress.Any, 80);

                    isWebServerStarted = true;
                }
                catch (Exception) { isWebServerStarted = false; }

                if (Log != null)
                    Log(this, null, isWebServerStarted ? "Success." : "Failed.");
            }

            // start gateway connector service:
            if (!isConnectorStarted)
            {
                if (Log != null)
                    Log(this, "Connecting to gateway... ", null);

                isConnectorStarted = connector.Connect();

                if (Log != null)
                    Log(this, null, isConnectorStarted ? "Success." : "Failed.");
            }

            // start DNS name service:
            //if (!isNameServiceStarted)
            //{
            //    if (ComponentStartEvent != null)
            //        ComponentStartEvent(this, "Starting name service... ", null);
            //    isNameServiceStarted = nameService.AddName("mysensors", NameService.NameType.Unique, NameService.MsSuffix.Default); // register on the local network
            //    if (ComponentStartEvent != null)
            //        ComponentStartEvent(this, null, isNameServiceStarted ? "Success." : "Failed.");
            //}


            return IsStarted;
        }
        public void Stop()
        {
            connector.Disconnect();
            dbService.Stop();

            isDBServiceStarted = false;
            isConnectorStarted = false;
            //isNameServiceStarted = false;
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

            if (!IsStarted)
                return;

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
                    SensorValue sv = new SensorValue(message.NodeID, message.SensorID, DateTime.Now, (SensorValueType)message.SubType, float.Parse(message.Payload.Replace('.', ',')));
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
                            connector.Send(new Message(message.NodeID, message.SensorID, MessageType.Internal, false, (byte)InternalValueType.Time, GetTime().ToString()));
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
        private int GetTime() // seconds since 1970
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local); // from 1970/1/1 00:00:00 to now
            DateTime dtNow = DateTime.Now;
            TimeSpan result = dtNow.Subtract(dt);
            int seconds = Convert.ToInt32(result.TotalSeconds);
            return seconds;
        }
        #endregion
    }
}
