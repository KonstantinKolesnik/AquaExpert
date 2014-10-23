
namespace MySensors.Controller.Connectors
{
    public delegate void MessageEventHandler(IGatewayConnector sender, string message);

    public interface IGatewayConnector
    {
        event MessageEventHandler MessageReceived;

        bool Connect();
        void Disconnect();

    }
}
