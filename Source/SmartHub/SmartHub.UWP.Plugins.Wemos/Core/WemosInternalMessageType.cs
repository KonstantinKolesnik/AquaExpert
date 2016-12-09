namespace SmartHub.UWP.Plugins.Wemos.Core
{
    public enum WemosInternalMessageType
    {
        BatteryLevel,           // Use this to report the battery level (in percent 0-100).
        Time,                   // Sensors can request the current time from the Controller using this message. The time will be reported as the seconds since 1970.
        Version,                // Sensors report their library version at startup using this message type.
        //InclusionMode,          // Start/stop inclusion mode of the Controller (1=start, 0=stop).
        Config,                 // Config request from node. Reply with (M)etric or (I)mperal back to sensor.
        //SketchName,            // Optional sketch name that can be used to identify sensor in the Controller GUI.
        //SketchVersion,         // Optional sketch version that can be reported to keep track of the version of sensor in the Controller GUI.
        Reboot                // Used by OTA firmware updates. Request for node to reboot.
    }
}
