using SmartNetwork.Plugins.MySensors.Core;

namespace SmartNetwork.Plugins.MySensors
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
