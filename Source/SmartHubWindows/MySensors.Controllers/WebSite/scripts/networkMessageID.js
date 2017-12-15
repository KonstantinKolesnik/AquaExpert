
NetworkMessageID = {
    // Sender: NONE
    // - No params
    Undefined: "",
    
    // Sender: STATION
    // - Param "Msg": message
    // - Param "Type": type - "", "Warning", "Error"
    Message: "Message",

    // Sender: STATION
    // - Param "WebTheme"
    // - Param "UnitSystem", M or I
    // Sender: CLIENT
    // - No params: request for station settings
    // - Param "WebTheme"
    // - Param "UnitSystem", M or I
    Settings: "Settings",

    // Sender: STATION
    // - Param "Version"
    // Sender: CLIENT
    // - No params: request for station version
    Version: "Version",

    // Sender: STATION
    // - Param "Ms"
    // Sender: CLIENT
    // - No params: request for station time
    // - Param "Ms"
    Time: "Time",

    // Sender: STATION
    // - Param "Zone"
    // Sender: CLIENT
    // - No params: request for station timezone
    // - Param "Zone"
    TimeZone: "TimeZone",

    //-----------------------------------------------------------------------

    // Sender: STATION
    // - Param "Nodes"
    // Sender: CLIENT
    // - No params: request for all nodes list
    GetNodes: "GetNodes",

    // Sender: CLIENT
    // - Param "NodeID"
    DeleteNode: "DeleteNode",

    // Sender: STATION
    // - Param "Node"
    NodePresentation: "NodePresentation",

    // Sender: STATION
    // - Param "Sensor"
    SensorPresentation: "SensorPresentation",

    // Sender: STATION
    // - Param "Level"
    BatteryLevel: "BatteryLevel",

    // Sender: STATION
    // - Param "Value"
    SensorValue: "SensorValue",

    // Sender: CLIENT
    // - Param "Msg"
    SensorMessage: "SensorMsg",

    // Sender: STATION
    // - Param "Modules"
    // Sender: CLIENT
    // - No params: request for all modules list
    GetModules: "GetModules",

    // Sender: STATION
    // - Param "Modules"
    // Sender: CLIENT
    // - No params: request for new module
    AddModule: "AddModule",

    // Sender: CLIENT
    // - Param "Module"
    SetModule: "SetModule",

    // Sender: CLIENT
    // - Param "ModuleID"
    DeleteModule: "DeleteModule"

}
