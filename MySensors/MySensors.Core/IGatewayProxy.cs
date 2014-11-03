using MySensors.Core;

namespace MySensors.Core
{
    public interface IGatewayProxy
    {
        event SensorMessageEventHandler MessageReceived;

        bool IsStarted { get; }

        void Start();
        void Stop();

        void Send(SensorMessage message);
    }
}
