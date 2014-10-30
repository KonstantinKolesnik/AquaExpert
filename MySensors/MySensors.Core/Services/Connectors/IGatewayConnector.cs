using MySensors.Core.Sensors;

namespace MySensors.Core.Services.Connectors
{
    public delegate void MessageEventHandler(IGatewayConnector sender, Message message);

    public interface IGatewayConnector
    {
        event MessageEventHandler MessageReceived;

        bool IsStarted { get; }

        void Connect();
        void Disconnect();

        void Send(Message message);
    }
}
