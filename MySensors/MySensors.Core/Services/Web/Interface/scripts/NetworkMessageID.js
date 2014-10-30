
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

    // Sender: STATION / CLIENT
    // - Param "WebTheme"
    // - Param "UnitSystem", M or I
    // Sender: CLIENT
    // - No params: request for station settings
    Settings: "Settings",

    // Sender: STATION / CLIENT
    // - Param "Version"
    // Sender: CLIENT
    // - No params: request for station version
    Version: "Version",

    //-----------------------------------------------------------------------

    // Sender: CLIENT
    // - No params: request for all nodes list
    GetNodes: "GetNodes"
}
