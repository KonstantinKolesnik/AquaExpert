using MySensors.Controller.Connectors;
using MySensors.Core.Messaging;
using System;

namespace MySensors.Controller
{
    public class Controller
    {
        private IGatewayConnector connector;



        public IGatewayConnector GatewayConnector
        {
            get { return connector; }
            set
            {
                if (connector != value)
                {
                    if (connector != null)
                        connector.MessageReceived -= connector_MessageReceived;

                    connector = value;
                    if (connector != null)
                        connector.MessageReceived += connector_MessageReceived;
                }
            }
        }





        private void connector_MessageReceived(IGatewayConnector sender, Message message)
        {
            Console.WriteLine(message.ToString());
            Console.WriteLine();
        }
    }
}
