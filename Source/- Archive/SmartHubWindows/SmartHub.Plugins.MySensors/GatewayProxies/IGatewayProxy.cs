using SmartHub.Plugins.MySensors.Core;
using System;

namespace SmartHub.Plugins.MySensors.GatewayProxies
{
    interface IGatewayProxy
    {
        event EventHandler Connected;
        event SensorMessageEventHandler MessageReceived;
        event EventHandler Disconnected;

        bool IsStarted { get; }

        void Start();
        void Stop();

        void Send(SensorMessage message);
    }
}
