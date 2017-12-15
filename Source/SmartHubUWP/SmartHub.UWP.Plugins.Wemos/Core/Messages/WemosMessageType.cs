namespace SmartHub.UWP.Plugins.Wemos.Core.Messages
{
    public enum WemosMessageType
    {
        Presentation,       // Sent by nodes when they present attached sensors. This is usually done in setup() at startup.
        Report,             // Sent by line when its value has changed.
        Set,                // Sent from or to a line when its value should be updated.
        Get,                // Sent to line to report its value.
        Internal,           // Special internal message.
        Stream              // Used for OTA firmware updates.
    }
}
