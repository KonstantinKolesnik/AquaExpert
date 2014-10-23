
namespace MySensors.Core.Nodes
{
    public enum InternalValueType : byte
    {
        BatteryLevel =              0,      // Use this to report the battery level (in percent 0-100).
        Time =                      1,      // Sensors can request the current time from the Controller using this message. The time will be reported as the seconds since 1970
        Version =                   2,      // Sensors report their library version at startup using this message type
        IDRequest =             	3,      // Use this to request a unique node id from the controller.
        IDResponse =                4,      // Id response back to sensor. Payload contains sensor id.
        InclusionMode =             5,      // Start/stop inclusion mode of the Controller (1=start, 0=stop).
        Config =                    6,      // Config request from node. Reply with (M)etric or (I)mperal back to sensor.
        FindParent =                7,      // When a sensor starts up, it broadcast a search request to all neighbor nodes. They reply with a FindParentResponse.
        FindParentResponse =        8,      // Reply message type to I_FIND_PARENT request.
        LogMessage =                9,      // Sent by the gateway to the Controller to trace-log a message
        Children =                  10,     // A message that can be used to transfer child sensors (from EEPROM routing table) of a repeating node.
        SketchName =                11,     // Optional sketch name that can be used to identify sensor in the Controller GUI
        SketchVersion =             12,     // Optional sketch version that can be reported to keep track of the version of sensor in the Controller GUI.
        Reboot =                    13,     // Used by OTA firmware updates. Request for node to reboot.
        GatewayReady =              14,     // Send by gateway to controller when startup is complete.
    }
}
