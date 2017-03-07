namespace SmartHub.UWP.Plugins.Wemos.Core.Messages
{
    public enum WemosMessageType
    {
        Presentation,       // Sent by nodes when they present attached sensors. This is usually done in setup() at startup.
        Report,             // This message is sent from sensor when a sensor value has changed.
        Set,                // This message is sent from or to a sensor when a sensor value should be updated.
        Get,                // Requests a variable value (usually from an actuator destined for controller).
        Internal,           // This is a special internal message.
        Stream              // Used for OTA firmware updates.
    }
}
