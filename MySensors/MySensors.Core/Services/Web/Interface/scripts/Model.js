
SensorType = {
    Door:                  0,      // Door and window sensors
    Motion:                1,      // Motion sensors
    Smoke:                 2,      // Smoke sensor
    Light:                 3,      // Light Actuator (on/off)
    Dimmer:                4,      // Dimmable device of some kind
    Cover:                 5,      // Window covers or shades
    Temperature:           6,      // Temperature sensor
    Humidity:              7,      // Humidity sensor
    Barometer:             8,      // Barometer sensor (Pressure)
    Wind:                  9,      // Wind sensor
    Rain:                  10,     // Rain sensor
    UV:                    11,     // UV sensor
    Weight:                12,     // Weight sensor for scales etc.
    Power:                 13,     // Power measuring device, like power meters
    Heater:                14,     // Heater device
    Distance:              15,     // Distance sensor
    LightLevel:            16,     // Light sensor
    /*ArduinoNode*/Device:           17,     // Arduino node device
    /*ArduinoRelay*/Repeater:          18,     // Arduino repeating node device
    Lock:                  19,     // Lock device
    IR:                    20,     // IR sender/receiver device
    Water:                 21,     // Water meter
    AirQuality:            22,     // Air quality sensor e.g. MQ-2
    Custom:                23,     // Use this for custom sensors where no other fits.
    Dust:                  24,     // Dust level sensor
    SceneController:       25      // Scene controller device
};
//----------------------------------------------------------------------------------------------------------------------
function Model() {
    // Properties:
    this.IsConnected = false;
    this.Settings = {
        WebTheme: "default",
        UnitSystem: "M"
    };
    this.Version = "";

    this.Devices = [];
    this.Sensors = function () {
        var res = [];

        var nodes = this.get("Devices");
        for (var i = 0; i < nodes.length; i++)
            for (var j = 0; j < nodes[i].Sensors.length; j++)
                res.push(nodes[i].Sensors[j]);

        return res;
    };

    //this.LEDConnectionImage = function () {
    //    return "Resources/Led" + (this.get("Connected") ? "Green" : "Grey") + "_16.ico";
    //}
    //this.LEDMainBoosterImage = function () {
    //    if (this.get("MainBoosterIsOverloaded"))
    //        return "Resources/LedRed_16.ico";
    //    else if (this.get("MainBoosterIsActive"))
    //        return "Resources/LedGreen_16.ico";
    //    else
    //        return "Resources/LedGrey_16.ico";
    //}
    //this.LEDProgBoosterImage = function () {
    //    if (this.get("ProgBoosterIsOverloaded"))
    //        return "Resources/LedRed_16.ico";
    //    else if (this.get("ProgBoosterIsActive"))
    //        return "Resources/LedGreen_16.ico";
    //    else
    //        return "Resources/LedGrey_16.ico";
    //}
    //this.BtnPowerImage = function () {
    //    return "Resources/Power" + (this.get("StationPower") ? "On" : "Off") + ".png";
    //}

    //this.IsMainVisible = function () { return this.get("UIState") == UIStateType.Main; }
    //this.IsLayoutVisible = function () { return this.get("UIState") == UIStateType.Layout; }
    //this.IsOperationVisible = function () { return this.get("UIState") == UIStateType.Operation; }
    //this.IsDecodersVisible = function () { return this.get("UIState") == UIStateType.Decoders; }
    //this.IsSettingsVisible = function () { return this.get("UIState") == UIStateType.Settings; }
    //this.IsInformationVisible = function () { return this.get("UIState") == UIStateType.Information; }
    //this.IsFirmwareVisible = function () { return this.get("UIState") == UIStateType.Firmware; }

    // Public functions:
}
