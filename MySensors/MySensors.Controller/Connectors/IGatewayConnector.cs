using MySensors.Core.Messaging;

namespace MySensors.Controller.Connectors
{
    public delegate void MessageEventHandler(IGatewayConnector sender, Message message);

    public interface IGatewayConnector
    {
        event MessageEventHandler MessageReceived;

        bool Connect();
        void Disconnect();

    }
}
