
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
