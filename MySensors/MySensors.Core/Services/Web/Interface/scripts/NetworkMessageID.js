
NetworkMessageID = {
    // Sender: NONE
    // - No params
    Undefined: "",
    
    // Sender: STATION
    // - Param "Msg": message
    OK: "OK",

    // Sender: STATION
    // - Param "Msg": message
    Information: "Information",

    // Sender: STATION
    // - Param "Msg": message
    Warning: "Warning",

    // Sender: STATION
    // - Param "Msg": message
    Error: "Error",

    //-----------------------------------------------------------------------

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

    // Sender: STATION
    // - Param "Level"
    BatteryLevel: "BatteryLevel",

    // Sender: STATION
    // - Param "Value"
    SensorValue: "SensorValue"






}
