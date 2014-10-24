using MySensors.Core.Connectors;
using MySensors.Core.Messaging;
using MySensors.Core.Services;
using System;

namespace MySensors.Core
{
    public delegate void ControllerComponentStartEventHandler(Controller sender, ControllerComponent component, bool? result);



    public class Controller
    {
        #region Fields
        private NameService nameService;
        private IGatewayConnector connector;

        private bool isConnectorStarted = false;
        private bool isNameServiceStarted = false;
        private bool isWebServerStarted = false;
        #endregion

        #region Properties
        public IGatewayConnector GatewayConnector
        {
            get { return connector; }
            //set
            //{
            //    if (connector != value)
            //    {
            //        if (connector != null)
            //            connector.MessageReceived -= connector_MessageReceived;

            //        connector = value;
            //        if (connector != null)
            //            connector.MessageReceived += connector_MessageReceived;
            //    }
            //}
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
        #endregion

        #region Event handlers
        private void connector_MessageReceived(IGatewayConnector sender, Message message)
        {
            Console.WriteLine(message.ToString());
            Console.WriteLine();
        }
        #endregion
    }
}
