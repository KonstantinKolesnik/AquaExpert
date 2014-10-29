var model;

//var model = kendo.observable({

//});

function Model() {
    // Properties:
    this.Connected = false;
    this.Disconnected = function () { return !this.get("Connected"); }
    this.StationPower = false;
    this.MainBoosterIsActive = false;
    this.MainBoosterIsOverloaded = false;
    this.MainBoosterCurrent = 0;
    this.ProgBoosterIsActive = false;
    this.ProgBoosterIsOverloaded = false;
    this.ProgBoosterCurrent = 0;
    this.Options = {
        MainBridgeCurrentThreshould: 3500,
        ProgBridgeCurrentThreshould: 500,
        BroadcastBoostersCurrent: false,
        UseWiFi: true,
        WiFiSSID: "",
        WiFiPassword: ""
    };
    this.Version = "";
    this.OperationList = new kendo.data.ObservableArray([]);

    this.UIState = UIStateType.Main;

    this.LEDConnectionImage = function () {
        return "Resources/Led" + (this.get("Connected") ? "Green" : "Grey") + "_16.ico";
    }
    this.LEDMainBoosterImage = function () {
        if (this.get("MainBoosterIsOverloaded"))
            return "Resources/LedRed_16.ico";
        else if (this.get("MainBoosterIsActive"))
            return "Resources/LedGreen_16.ico";
        else
            return "Resources/LedGrey_16.ico";
    }
    this.LEDProgBoosterImage = function () {
        if (this.get("ProgBoosterIsOverloaded"))
            return "Resources/LedRed_16.ico";
        else if (this.get("ProgBoosterIsActive"))
            return "Resources/LedGreen_16.ico";
        else
            return "Resources/LedGrey_16.ico";
    }
    this.BtnPowerImage = function () {
        return "Resources/Power" + (this.get("StationPower") ? "On" : "Off") + ".png";
    }

    this.IsMainVisible = function () { return this.get("UIState") == UIStateType.Main; }
    this.IsLayoutVisible = function () { return this.get("UIState") == UIStateType.Layout; }
    this.IsOperationVisible = function () { return this.get("UIState") == UIStateType.Operation; }
    this.IsDecodersVisible = function () { return this.get("UIState") == UIStateType.Decoders; }
    this.IsSettingsVisible = function () { return this.get("UIState") == UIStateType.Settings; }
    this.IsInformationVisible = function () { return this.get("UIState") == UIStateType.Information; }
    this.IsFirmwareVisible = function () { return this.get("UIState") == UIStateType.Firmware; }

    // Public functions:
    this.SetPower = function () {
        this.MessageManager.SetPower(!this.get("StationPower"));
    }
    this.SetOptions = function () {
        this.MessageManager.SetOptions(
            this.get("Options.MainBridgeCurrentThreshould"),
            this.get("Options.ProgBridgeCurrentThreshould"),
            this.get("Options.BroadcastBoostersCurrent"),
            this.get("Options.UseWiFi"),
            this.get("Options.WiFiSSID"),
            this.get("Options.WiFiPassword")
            );
    }
}
