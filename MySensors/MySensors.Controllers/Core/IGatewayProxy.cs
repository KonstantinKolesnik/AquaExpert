
namespace MySensors.Controllers.Core
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
