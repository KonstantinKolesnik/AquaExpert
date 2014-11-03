using MySensors.Core;

namespace MySensors.Core
{
    public interface IGatewayProxy
    {
        event SensorMessageEventHandler MessageReceived;

        bool IsStarted { get; }

        void Connect();
        void Disconnect();

        void Send(SensorMessage message);
    }
}
