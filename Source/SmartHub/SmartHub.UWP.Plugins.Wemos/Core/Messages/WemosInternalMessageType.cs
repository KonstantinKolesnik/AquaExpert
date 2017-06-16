namespace SmartHub.UWP.Plugins.Wemos.Core.Messages
{
    public enum WemosInternalMessageType
    {
        BatteryLevel,           // Use this to report the battery level (in percent 0-100).
        Time,                   // Lines can request the current time from the Controller using this message. The time will be reported as the seconds since 1970.
        Version,                // Lines report their library version at startup using this message type.
        Config,                 // Configuration request from node. Reply with (M)etric or (I)mperal back to node.
        FirmwareName,           // Optional Firmware name that can be used to identify sensor in the Controller GUI.
        FirmwareVersion,        // Optional Firmware version that can be reported to keep track of the version of sensor in the Controller GUI.
        Reboot                  // Used by OTA firmware updates. Request for node to reboot.
    }
}
