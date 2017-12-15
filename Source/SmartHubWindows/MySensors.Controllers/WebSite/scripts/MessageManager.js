
function MessageManager() {
    var me = this;
    
    this.IsFromServer = false;
    //----------------------------------------------------------------------------------------------------------------------
    // Events:
    this.onSend = null;
    this.ProcessMessage = function (txt) {
        //console.log(txt);
        var msg = new NetworkMessage(NetworkMessageID.Undefined);
        msg.FromText(txt);

        var response = processMessage(msg);
        send(response);
    }
    //----------------------------------------------------------------------------------------------------------------------
    // Public functions (commands):
    this.GetSettings = function () {
        send(new NetworkMessage(NetworkMessageID.Settings));
    }
    this.SetSettings = function (webTheme, unitSystem) {
        send(new NetworkMessage(NetworkMessageID.Settings, [webTheme, unitSystem]));
    }
    this.GetVersion = function () {
        send(new NetworkMessage(NetworkMessageID.Version));
    }

    this.GetNodes = function () {
        send(new NetworkMessage(NetworkMessageID.GetNodes));
    }
    this.DeleteNode = function (id) {
        send(new NetworkMessage(NetworkMessageID.DeleteNode, [id]));
    }

    this.GetModules = function () {
        send(new NetworkMessage(NetworkMessageID.GetModules));
    }
    this.AddModule = function () {
        send(new NetworkMessage(NetworkMessageID.AddModule));
    }
    this.SetModule = function (module) {
        var obj = $.extend({}, module);
        obj.Script = btoa(obj.Script);
        obj.View = btoa(obj.View);

        send(new NetworkMessage(NetworkMessageID.SetModule, [JSON.stringify(obj)]));
    }
    this.DeleteModule = function (id) {
        send(new NetworkMessage(NetworkMessageID.DeleteModule, [id]));
    }

    this.SendNodeMessage = function (nodeID, messageType, valueType, value) {
        me.SendSensorMessage(nodeID, 255, messageType, valueType, value);
    }
    this.SendSensorMessage = function (nodeID, sensorID, messageType, valueType, value) {
        send(new NetworkMessage(NetworkMessageID.SensorMessage, [nodeID + "*" + sensorID + "*" + messageType + "*" + "0" + "*" + valueType + "*" + value]));
    }
    //----------------------------------------------------------------------------------------------------------------------
    function send(msg) {
        if (me.onSend && msg)
            me.onSend(msg.ToText());
    }
    function processMessage(msg) {
        me.IsFromServer = true;

        var response = null;

        switch (msg.GetID()) {
            case NetworkMessageID.Message: mainView.showDialog(msg.GetParameter(0), msg.GetParameter(1)); break;
            case NetworkMessageID.Version: viewModel.set("Version", msg.GetParameter(0)); break;
            case NetworkMessageID.Settings:
                viewModel.set("Settings.WebTheme", msg.GetParameter(0));
                viewModel.set("Settings.UnitSystem", msg.GetParameter(1));
                break;
            case NetworkMessageID.GetNodes:
                var newItems = JSON.parse(msg.GetParameter(0));

                // approach #1:
                for (var i = 0; i < newItems.length; i++)
                    enrichItem(newItems[i]);
                viewModel.set("Nodes", newItems);
                //viewModel.PopulateSensors();

                // approach #2:
                //var oldItems = viewModel.get("Nodes");
                //oldItems.length = 0; // remove all old nodes
                //for (var i = 0; i < newItems.length; i++) {
                //    var item = newItems[i];
                //    enrichItem(item);
                //    oldItems.push(item);
                //}
                //viewModel.PopulateSensors();

                break;
            case NetworkMessageID.DeleteNode:
                var id = msg.GetParameter(0);

                for (var i = 0; i < viewModel.Sensors.length; i++)
                    if (viewModel.Sensors[i].NodeID == id)
                        viewModel.Sensors.splice(i, 1);

                for (var i = 0; i < viewModel.Nodes.length; i++)
                    if (viewModel.Nodes[i].ID == id)
                        viewModel.Nodes.splice(i, 1);

                viewModel.PopulateSensors();

                break;
            case NetworkMessageID.GetModules:
                var items = JSON.parse(msg.GetParameter(0));

                for (var i = 0; i < items.length; i++) {
                    items[i].Script = atob(items[i].Script);
                    items[i].View = atob(items[i].View);
                }

                viewModel.set("Modules", items);
                break;
            case NetworkMessageID.AddModule:
                var items = JSON.parse(msg.GetParameter(0));

                for (var i = 0; i < items.length; i++) {
                    items[i].Script = atob(items[i].Script);
                    items[i].View = atob(items[i].View);

                    viewModel.Modules.push(items[i]);
                }
                break;
            case NetworkMessageID.SetModule:
                var items = JSON.parse(msg.GetParameter(0));

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];
                    item.Script = atob(item.Script);
                    item.View = atob(item.View);

                    var exists = false;
                    for (var j = 0; j < viewModel.Modules.length; j++)
                        var module = viewModel.Modules[j];
                        if (module.ID == item.ID) {
                            exists = true;

                            module.set("Name", item.Name);
                            module.set("Description", item.Description);
                            module.set("Script", item.Script);
                            module.set("View", item.View);
                            break;
                        }

                    if (!exists)
                        viewModel.Modules.push(item);
                }
                break;
            case NetworkMessageID.DeleteModule:
                var id = msg.GetParameter(0);

                for (var i = 0; i < viewModel.Modules.length; i++)
                    if (viewModel.Modules[i].ID == id)
                        viewModel.Modules.splice(i, 1);
                break;
            case NetworkMessageID.NodePresentation: presentNode(JSON.parse(msg.GetParameter(0))); break;
            case NetworkMessageID.SensorPresentation: presentSensor(JSON.parse(msg.GetParameter(0))); break;
            case NetworkMessageID.BatteryLevel: addBatteryLevel(JSON.parse(msg.GetParameter(0))); break;
            case NetworkMessageID.SensorValue: addSensorValue(JSON.parse(msg.GetParameter(0))); break;
            default: break;
        }

        me.IsFromServer = false;

        return response;

        function enrichItem(item) {
            enrichNode(item);
            for (var i = 0; i < item.Sensors.length; i++)
                enrichSensor(item.Sensors[i]);
        }
        function enrichNode(node) {
            //if (node.isEnriched)
            //    return;

            node.TypeName = function () {
                return getSensorTypeName(this.get("Type"));
            }

            node.BatteryLevels = node.BatteryLevels || [];
            for (var i = 0; i < node.BatteryLevels.length; i++)
                node.BatteryLevels[i].Time = new Date(node.BatteryLevels[i].Time); // convert battery level's date/time

            node.Sensors = node.Sensors || [];

            //node.isEnriched = true;
        }
        function enrichSensor(sensor) {
            //if (sensor.isEnriched)
            //    return;

            sensor.TypeName = function () {
                return getSensorTypeName(this.get("Type"));
            }

            sensor.Values = sensor.Values || [];
            for (var j = 0; j < sensor.Values.length; j++)
                sensor.Values[j].Time = new Date(sensor.Values[j].Time); // convert sensor value's date/time

            //sensor.isEnriched = true;
        }

        function presentNode(newNode) {
            //if (!viewModel.Nodes)
            //    viewModel.Nodes = new kendo.observableArray();

            for (var i = 0; i < viewModel.Nodes.length; i++) {
                var nd = viewModel.Nodes[i];
                if (nd.ID == newNode.ID) { // update node
                    nd.set("Type", newNode.Type);
                    nd.set("ProtocolVersion", newNode.ProtocolVersion);
                    nd.set("SketchName", newNode.SketchName);
                    nd.set("SketchVersion", newNode.SketchVersion);
                    return;
                }
            }

            // node isn't in list; add it:
            //enrichItem(newNode); // with sensors!!! not enrichNode (it doesn't enriches sensors)!!!
            enrichNode(newNode);
            viewModel.Nodes.push(newNode);
        }
        function presentSensor(newSensor) {
            //if (!viewModel.Nodes)
            //    viewModel.Nodes = new kendo.observableArray();

            for (var i = 0; i < viewModel.Nodes.length; i++) {
                var nd = viewModel.Nodes[i];
                if (nd.ID == newSensor.NodeID) {
                    //if (!nd.Sensors)
                    //    nd.Sensors = new kendo.observableArray();

                    for (var j = 0; j < nd.Sensors.length; j++) {
                        var snsr = nd.Sensors[j];
                        if (snsr.NodeID == newSensor.NodeID && snsr.ID == newSensor.ID) { // update sensor
                            console.log("presentSensor: exists");
                            snsr.set("Type", newSensor.Type);
                            snsr.set("ProtocolVersion", newSensor.ProtocolVersion);
                            //viewModel.PresentSensor(snsr);
                            return;
                        }
                    }

                    // sensor isn't in list; add it:
                    enrichSensor(newSensor);
                    nd.Sensors.push(newSensor);
                    //viewModel.PresentSensor(newSensor);
                }
            }
        }
        function addBatteryLevel(bl) {
            bl.Time = new Date(bl.Time);

            var nodes = viewModel.Nodes || [];
            for (var i = 0; i < nodes.length; i++)
                if (nodes[i].ID == bl.NodeID) {
                    nodes[i].BatteryLevels.push(bl);
                    nodes[i].set("LastBatteryLevel", bl);
                }
        }
        function addSensorValue(sv) {
            sv.Time = new Date(sv.Time);

            var nodes = viewModel.Nodes || [];
            for (var i = 0; i < nodes.length; i++)
                if (nodes[i].ID == sv.NodeID) {
                    var sensors = nodes[i].Sensors || [];
                    for (var j = 0; j < sensors.length; j++)
                        if (sensors[j].NodeID == sv.NodeID && sensors[j].ID == sv.ID) {
                            //if (!sensors[j].Values)
                            //    sensors[j].Values = new kendo.observableArray();

                            sensors[j].Values.push(sv);
                            sensors[j].set("LastValue", sv);
                        }
                }
        }
        function getSensorTypeName(value) {
            for (var name in SensorType)
                if (SensorType[name] == value)
                    return name;

            return "Unknown";
        }
    }
}