using SmartHub.Plugins.MySensors.Core;

namespace SmartHub.Plugins.MySensors
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
