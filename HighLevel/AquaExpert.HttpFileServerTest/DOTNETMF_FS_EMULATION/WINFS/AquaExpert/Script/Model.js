var model;

function Model() {
    // Fields:
    var me = this;
    var socket = null;
    var port = 12000;
    var resourcePath = "Typhoon";

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

    this.MessageManager;

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
    this.Init = function () {
        createWebSocket();

        this.MessageManager = new MessageManager(socket);
        this.MessageManager.MessageReceived = onServerMessage;
    }
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

    // Private functions:
    function createWebSocket() {
        var support = "MozWebSocket" in window ? 'MozWebSocket' : ("WebSocket" in window ? 'WebSocket' : null);
        if (support) {
            //socket = new WebSocket("ws://" + document.location.hostname + ":" + port + '/' + resourcePath);
            socket = new window[support]("ws://" + document.location.hostname + ":" + port + '/' + resourcePath);
            socket.onopen = onSocketOpen;
            socket.onclose = onSocketClose;
            socket.onerror = onSocketError;
        }
        else
            showDialog("No WebSocket support! Please try another browser.", "Warning");
        //alert("No WebSocket support! Please try another browser.");
        function onSocketOpen() {
            model.set("Connected", true);
            model.MessageManager.GetPower();
        }
        function onSocketClose() {
            model.set("Connected", false);
            closeWebSocket();
            createWebSocket();
        }
        function onSocketError(e) {
            //showDialog("WebSocket error: " + e);
        }
    }
    function closeWebSocket() {
        if (socket)
            socket.close();
        socket = null;
    }

    function showDialog(txt, title) {
        var win = $("#dlg").kendoWindow({
            actions: ["Close"],
            width: "300px",
            height: "100px",
            title: "Typhoon",
            visible: false,
            draggable: true,
            resizable: false,
            modal: true
        }).data("kendoWindow");

        if (txt)
            win.content(txt);
        if (title)
            win.title(title);
        win.center().open();
    }

    // Event handlers:
    function onServerMessage(networkMessage) {
        var response = processServerMessage(networkMessage);
        model.MessageManager.Send(response);
    }
    function processServerMessage(msg) {
        var response = null;

        switch (msg.GetID()) {
            case NetworkMessageID.OK:
                var s = "Operation completed successfully!";
                var ss = msg.GetParameter("Msg");
                if (ss)
                    s = ss;
                showDialog(s);
                break;
            case NetworkMessageID.Information:
                showDialog(msg.GetParameter("Msg"), "Information");
                break;
            case NetworkMessageID.Warning:
                showDialog(msg.GetParameter("Msg"), "Warning");
                break;
            case NetworkMessageID.Error:
                showDialog(msg.GetParameter("Msg"), "Error");
                break;
            case NetworkMessageID.Power:
                model.set("StationPower", msg.GetParameter("Power") == "True");
                model.set("MainBoosterIsActive", msg.GetParameter("MainActive") == "True");
                model.set("MainBoosterIsOverloaded", msg.GetParameter("MainOverload") == "True");
                model.set("ProgBoosterIsActive", msg.GetParameter("ProgActive") == "True");
                model.set("ProgBoosterIsOverloaded", msg.GetParameter("ProgOverload") == "True");
                break;
            case NetworkMessageID.Options:
                model.set("Options.MainBridgeCurrentThreshould", msg.GetParameter("MainBridgeCurrentThreshould"));
                model.set("Options.ProgBridgeCurrentThreshould", msg.GetParameter("ProgBridgeCurrentThreshould"));
                model.set("Options.BroadcastBoostersCurrent", msg.GetParameter("BroadcastBoostersCurrent") == "True");
                model.set("Options.UseWiFi", msg.GetParameter("UseWiFi") == "True");
                model.set("Options.WiFiSSID", msg.GetParameter("WiFiSSID"));
                model.set("Options.WiFiPassword", msg.GetParameter("WiFiPassword"));
                break;
            case NetworkMessageID.Version:
                model.set("Version", msg.GetParameter("Version"));
                break;
            case NetworkMessageID.BoostersCurrent:
                model.set("MainBoosterCurrent", msg.GetParameter("Main"));
                model.set("ProgBoosterCurrent", msg.GetParameter("Prog"));
                break;
            default:
                break;
        }

        return response;
    }
}
