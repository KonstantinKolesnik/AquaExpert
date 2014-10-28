
function LocomotiveAddress(address, long) {
    this.Address = address;
    this.Long = long;
}
//----------------------------------------------------------------------------------------------------------------------
function MessageManager(socket) {
    var me = this;
    var socket = socket;
    
    if (socket)
        socket.onmessage = function (e) {
            var msg = new NetworkMessage(NetworkMessageID.Undefined);
            msg.FromText(e.data);
            me.MessageReceived(msg);
        };
    //----------------------------------------------------------------------------------------------------------------------
    // Events:
    this.SentAllBrake = function () { }
    this.SentAllStop = function () { }
    this.SentAllReset = function () { }
    this.MessageReceived = function (networkMessage) { }
    //----------------------------------------------------------------------------------------------------------------------
    // Public functions (commands):
    this.Send = function (networkMessage) {
        //setTimeout(sss(), 500);
        sss();

        function sss() {
            if (socket && networkMessage)
                socket.send(networkMessage.ToText());
        }
    }

    this.GetPower = function () {
        this.Send(new NetworkMessage(NetworkMessageID.Power));
    }
    this.SetPower = function (on) {
        var msg = new NetworkMessage(NetworkMessageID.Power);
        msg.SetParameter("Power", on ? "True" : "False");
        this.Send(msg);
    }

    this.GetOptions = function () {
        this.Send(new NetworkMessage(NetworkMessageID.Options));
    }
    this.SetOptions = function (mainImax, progImax, broadcastI, useWifi, wifiSSID, wifiPassword) {
        var msg = new NetworkMessage(NetworkMessageID.Options);
        msg.SetParameter("MainBridgeCurrentThreshould", mainImax);
        msg.SetParameter("ProgBridgeCurrentThreshould", progImax);
        msg.SetParameter("BroadcastBoostersCurrent", broadcastI ? "True" : "False");
        msg.SetParameter("UseWiFi", useWifi ? "True" : "False");
        msg.SetParameter("WiFiSSID", wifiSSID);
        msg.SetParameter("WiFiPassword", wifiPassword);
        this.Send(msg);
    }

    this.GetVersion = function () {
        this.Send(new NetworkMessage(NetworkMessageID.Version));
    }

    this.BroadcastBrake = function () {
        this.Send(new NetworkMessage(NetworkMessageID.BroadcastBrake));
        this.SentAllBrake();
    }
    this.BroadcastStop = function () {
        this.Send(new NetworkMessage(NetworkMessageID.BroadcastStop));
        this.SentAllStop();
    }
    this.BroadcastReset = function () {
        this.Send(new NetworkMessage(NetworkMessageID.BroadcastReset));
        this.SentAllReset();
    }

    this.SetLocoSpeed14 = function(address, speed, forward, light)
    {
        var msg = new NetworkMessage(NetworkMessageID.LocoSpeed14);
        msg.SetParameter("Address", address.Address);
        msg.SetParameter("Long", address.Long ? "True" : "False");
        msg.SetParameter("Speed", speed);
        msg.SetParameter("Forward", forward ? "True" : "False");
        msg.SetParameter("Light", light ? "True" : "False");
        this.Send(msg);
    }
    this.SetLocoSpeed28 = function (address, speed, forward) {
        var msg = new NetworkMessage(NetworkMessageID.LocoSpeed28);
        msg.SetParameter("Address", address.Address);
        msg.SetParameter("Long", address.Long ? "True" : "False");
        msg.SetParameter("Speed", speed);
        msg.SetParameter("Forward", forward ? "True" : "False");
        this.Send(msg);
    }
    this.SetLocoSpeed128 = function (address, speed, forward) {
        var msg = new NetworkMessage(NetworkMessageID.LocoSpeed128);
        msg.SetParameter("Address", address.Address);
        msg.SetParameter("Long", address.Long ? "True" : "False");
        msg.SetParameter("Speed", speed);
        msg.SetParameter("Forward", forward ? "True" : "False");
        this.Send(msg);
    }

    this.SetLocoFunctionGroup1 = function (address, F0, F1, F2, F3, F4) {
        var msg = new NetworkMessage(NetworkMessageID.LocoFunctionGroup1);
        msg.SetParameter("Address", address.Address);
        msg.SetParameter("Long", address.Long ? "True" : "False");
        msg.SetParameter("F0", F0 ? "True" : "False");
        msg.SetParameter("F1", F1 ? "True" : "False");
        msg.SetParameter("F2", F2 ? "True" : "False");
        msg.SetParameter("F3", F3 ? "True" : "False");
        msg.SetParameter("F4", F4 ? "True" : "False");
        this.Send(msg);
    }
    this.SetLocoFunctionGroup2 = function (address, F5, F6, F7, F8) {
        var msg = new NetworkMessage(NetworkMessageID.LocoFunctionGroup2);
        msg.SetParameter("Address", address.Address);
        msg.SetParameter("Long", address.Long ? "True" : "False");
        msg.SetParameter("F5", F5 ? "True" : "False");
        msg.SetParameter("F6", F6 ? "True" : "False");
        msg.SetParameter("F7", F7 ? "True" : "False");
        msg.SetParameter("F8", F8 ? "True" : "False");
        this.Send(msg);
    }
    this.SetLocoFunctionGroup3 = function (address, F9, F10, F11, F12) {
        var msg = new NetworkMessage(NetworkMessageID.LocoFunctionGroup3);
        msg.SetParameter("Address", address.Address);
        msg.SetParameter("Long", address.Long ? "True" : "False");
        msg.SetParameter("F9", F9 ? "True" : "False");
        msg.SetParameter("F10", F10 ? "True" : "False");
        msg.SetParameter("F11", F11 ? "True" : "False");
        msg.SetParameter("F12", F12 ? "True" : "False");
        this.Send(msg);
    }
    this.SetLocoFunctionGroup4 = function (address, F13, F14, F15, F16, F17, F18, F19, F20) {
        var msg = new NetworkMessage(NetworkMessageID.LocoFunctionGroup4);
        msg.SetParameter("Address", address.Address);
        msg.SetParameter("Long", address.Long ? "True" : "False");
        msg.SetParameter("F13", F13 ? "True" : "False");
        msg.SetParameter("F14", F14 ? "True" : "False");
        msg.SetParameter("F15", F15 ? "True" : "False");
        msg.SetParameter("F16", F16 ? "True" : "False");
        msg.SetParameter("F17", F17 ? "True" : "False");
        msg.SetParameter("F18", F18 ? "True" : "False");
        msg.SetParameter("F19", F19 ? "True" : "False");
        msg.SetParameter("F20", F20 ? "True" : "False");
        this.Send(msg);
    }
    this.SetLocoFunctionGroup5 = function (address, F21, F22, F23, F24, F25, F26, F27, F28) {
        var msg = new NetworkMessage(NetworkMessageID.LocoFunctionGroup5);
        msg.SetParameter("Address", address.Address);
        msg.SetParameter("Long", address.Long ? "True" : "False");
        msg.SetParameter("F21", F21 ? "True" : "False");
        msg.SetParameter("F22", F22 ? "True" : "False");
        msg.SetParameter("F23", F23 ? "True" : "False");
        msg.SetParameter("F24", F24 ? "True" : "False");
        msg.SetParameter("F25", F25 ? "True" : "False");
        msg.SetParameter("F26", F26 ? "True" : "False");
        msg.SetParameter("F27", F27 ? "True" : "False");
        msg.SetParameter("F28", F28 ? "True" : "False");
        this.Send(msg);
    }
}