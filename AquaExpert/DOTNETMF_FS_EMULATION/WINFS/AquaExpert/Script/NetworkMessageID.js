
NetworkMessageID = {
    // Sender: NONE
    // - No params
    Undefined: "",
    
    // Sender: STATION
    // - Param "Msg": message
    OK: "OK",
    Information: "Info",
    Warning: "Warning",
    Error: "Error",

    //-----------------------------------------------------------------------

    // Sender: STATION
    // - Param "Layout": station layout xml
    // Sender: CLIENT
    // - No params: request for station layout
    // Sender: CLIENT
    // - Param "Layout": client layout xml
    Layout: "Layout",

    //-----------------------------------------------------------------------

    // Sender: STATION / CLIENT
    // - Param "MainBridgeCurrentThreshould"
    // - Param "ProgBridgeCurrentThreshould"
    // - Param "BroadcastBoostersCurrent"
    // - Param "WiFiSSID"
    // - Param "WiFiPassword"
    // Sender: CLIENT
    // - No params: request for station options
    Settings: "Settings",

    // Sender: STATION
    // - Param "Version"
    // Sender: CLIENT
    // - No params: request for settings
    Version: "Version",

    //-----------------------------------------------------------------------

    // Sender: STATION
    // - Param "Power": "True"/"False" - is station power on
    // - Param "MainActive": "True"/"False" - is main booster active
    // - Param "MainOverload": "True"/"False" - is main booster overloaded
    // - Param "ProgActive": "True"/"False" - is prog booster active
    // - Param "ProgOverload": "True"/"False" - is prog booster overloaded
    // Sender: CLIENT
    // - No params: request for station power status
    // Sender: CLIENT
    // - Param "Power": "True"/"False" - power status to set
    Power: "Power",

    // Sender: CLIENT
    // - No params: request for boosters current
    // Sender: STATION
    // - Param "Main": main booster current, mA
    // - Param "Prog": prog booster current, mA
    BoostersCurrent: "BoostersCurrent",

    //-----------------------------------------------------------------------

    // Sender: CLIENT
    // - No params
    BroadcastBrake: "BroadcastBrake",

    // Sender: CLIENT
    // - No params
    BroadcastStop: "BroadcastStop",

    // Sender: CLIENT
    // - No params
    BroadcastReset: "BroadcastReset",

    // Sender: CLIENT
    // - Param "Address": locomotive address
    // - Param "Long": "True"/"False" - is long address
    // - Param "Speed": speed
    // - Param "Forward": "True"/"False" - is forward
    // - Param "Light": "True"/"False" - is light on
    LocoSpeed14: "LocoSpeed14",

    // Sender: CLIENT
    // - Param "Address": locomotive address
    // - Param "Long": "True"/"False" - is long address
    // - Param "Speed": speed
    // - Param "Forward": "True"/"False" - is forward
    LocoSpeed28: "LocoSpeed28",

    // Sender: CLIENT
    // - Param "Address": locomotive address
    // - Param "Long": "True"/"False" - is long address
    // - Param "Speed": speed
    // - Param "Forward": "True"/"False" - is forward
    LocoSpeed128: "LocoSpeed128",

    // Sender: CLIENT
    // - Param "Address": locomotive address
    // - Param "Long": "True"/"False" - is long address
    // - Param "F0": "True"/"False" - F0
    // - Param "F1": "True"/"False" - F1
    // - Param "F2": "True"/"False" - F2
    // - Param "F3": "True"/"False" - F3
    // - Param "F4": "True"/"False" - F4
    LocoFunctionGroup1: "LocoFunctionGroup1",

    // Sender: CLIENT
    // - Param "Address": locomotive address
    // - Param "Long": "True"/"False" - is long address
    // - Param "F5": "True"/"False" - F5
    // - Param "F6": "True"/"False" - F6
    // - Param "F7": "True"/"False" - F7
    // - Param "F8": "True"/"False" - F8
    LocoFunctionGroup2: "LocoFunctionGroup2",

    // Sender: CLIENT
    // - Param "Address": locomotive address
    // - Param "Long": "True"/"False" - is long address
    // - Param "F9": "True"/"False" - F9
    // - Param "F10": "True"/"False" - F10
    // - Param "F11": "True"/"False" - F11
    // - Param "F12": "True"/"False" - F12
    LocoFunctionGroup3: "LocoFunctionGroup3",

    // Sender: CLIENT
    // - Param "Address": locomotive address
    // - Param "Long": "True"/"False" - is long address
    // - Param "F13": "True"/"False" - F13
    // - Param "F14": "True"/"False" - F14
    // - Param "F15": "True"/"False" - F15
    // - Param "F16": "True"/"False" - F16
    // - Param "F17": "True"/"False" - F17
    // - Param "F18": "True"/"False" - F18
    // - Param "F19": "True"/"False" - F19
    // - Param "F20": "True"/"False" - F20
    LocoFunctionGroup4: "LocoFunctionGroup4",

    // Sender: CLIENT
    // - Param "Address": locomotive address
    // - Param "Long": "True"/"False" - is long address
    // - Param "F21": "True"/"False" - F21
    // - Param "F22": "True"/"False" - F22
    // - Param "F23": "True"/"False" - F23
    // - Param "F24": "True"/"False" - F24
    // - Param "F25": "True"/"False" - F25
    // - Param "F26": "True"/"False" - F26
    // - Param "F27": "True"/"False" - F27
    // - Param "F28": "True"/"False" - F28
    LocoFunctionGroup5: "LocoFunctionGroup5"
}
