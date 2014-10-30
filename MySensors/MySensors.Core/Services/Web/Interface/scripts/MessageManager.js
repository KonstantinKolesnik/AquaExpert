
function MessageManager() {
    var me = this;
    
    //----------------------------------------------------------------------------------------------------------------------
    // Events:
    this.onSend = null;
    this.onReceive = function (txt) {
        var msg = new NetworkMessage(NetworkMessageID.Undefined);
        msg.FromText(txt);

        var response = processMessage(msg);
        send(response);
    }
    //----------------------------------------------------------------------------------------------------------------------
    // Public functions (commands):

    this.HelloWorld = function () {
        var msg = new NetworkMessage(NetworkMessageID.Information);
        msg.SetParameter("Msg", "Hello world!");
        send(msg);
    }

    this.GetSettings = function () {
        send(new NetworkMessage(NetworkMessageID.Settings));
    }
    this.SetSettings = function (webTheme, unitSystem) {
        var msg = new NetworkMessage(NetworkMessageID.Settings);
        msg.SetParameter("WebTheme", webTheme);
        msg.SetParameter("UnitSystem", unitSystem);
        send(msg);
    }

    this.GetVersion = function () {
        send(new NetworkMessage(NetworkMessageID.Version));
    }

    this.GetNodes = function () {
        send(new NetworkMessage(NetworkMessageID.GetNodes));
    }
    //----------------------------------------------------------------------------------------------------------------------
    function send(msg) {
        if (me.onSend && msg)
            me.onSend(msg.ToText());
    }
    function processMessage(msg) {
        var response = null;

        switch (msg.GetID()) {
            case NetworkMessageID.OK:
                var s = "Operation completed successfully!";
                var ss = msg.GetParameter("Msg");
                if (ss)
                    s = ss;
                mainView.showDialog(s);
                break;
            case NetworkMessageID.Information:
                mainView.showDialog(msg.GetParameter("Msg"), "Information");
                break;
            case NetworkMessageID.Warning:
                mainView.showDialog(msg.GetParameter("Msg"), "Warning");
                break;
            case NetworkMessageID.Error:
                mainView.showDialog(msg.GetParameter("Msg"), "Error");
                break;
            //case NetworkMessageID.Power:
            //    viewModel.set("StationPower", msg.GetParameter("Power") == "True");
            //    viewModel.set("MainBoosterIsActive", msg.GetParameter("MainActive") == "True");
            //    viewModel.set("MainBoosterIsOverloaded", msg.GetParameter("MainOverload") == "True");
            //    viewModel.set("ProgBoosterIsActive", msg.GetParameter("ProgActive") == "True");
            //    viewModel.set("ProgBoosterIsOverloaded", msg.GetParameter("ProgOverload") == "True");
            //    break;
            case NetworkMessageID.Settings:
                viewModel.set("Settings.WebTheme", msg.GetParameter("WebTheme"));
                viewModel.set("Settings.UnitSystem", msg.GetParameter("UnitSystem"));
                break;
            case NetworkMessageID.Version:
                viewModel.set("Version", msg.GetParameter("Version"));
                break;
            case NetworkMessageID.GetNodes:
                var items = JSON.parse(msg.GetParameter("Nodes"));

                //var coll = viewModel.get("Devices");
                //for (var i = 0; i < items.length; i++) {
                //    coll.push(items[i]);
                //}

                viewModel.set("Devices", items);
                break;
            default:
                break;
        }

        return response;
    }
}