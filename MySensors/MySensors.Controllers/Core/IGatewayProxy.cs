
namespace MySensors.Controllers.Core
{
    interface IGatewayProxy
    {
        event SensorMessageEventHandler MessageReceived;

        bool IsStarted { get; }

        void Start();
        void Stop();

        void Send(SensorMessage message);
    }
}
