
define(['jquery'],
    function ($) {
        var api = {
            getMonitor: function (id, onComplete) {
                $.getJSON('/api/monitors/get/rich', { id: id })
                    .done(function (data) {
                        if (onComplete)
                            onComplete(data);
                    })
                    .fail(function (data) {
                        onError(data);
                    });
            },
            setMonitorConfiguration: function (monitor, onComplete) {
                $.post('/api/monitors/setconfiguration', { id: monitor.Id, config: monitor.Configuration })
            		.done(function (data) {
            		    if (onComplete)
            		        onComplete(data);
            		})
                    .fail(function (data) {
                        onError(data);
                    });
            }
        };

        var viewModel = kendo.observable({
            Monitor: null,
            update: function (id, onComplete) {
                var me = this;

                api.getMonitor(id, function (data) {
                    me.set("Monitor", data);

                    if (onComplete)
                        onComplete();
                });
            },
            SignalRReceiveHandler: function (model, data) {
                if (data.MsgId == "SensorValue") {
                    data.Data.TimeStamp = new Date(data.Data.TimeStamp);
                    var sv = data.Data;
                    var monitor = model.Monitor;

                    if (monitor && monitor.SensorNodeNo == sv.NodeNo && monitor.SensorSensorNo == sv.SensorNo) {
                        monitor.SensorValues.splice(0, 1);
                        monitor.SensorValues.push(sv);
                    }
                }
            }
        });
        viewModel.bind("change", function (e) {
            if (e.field.indexOf("Monitor.Configuration") > -1)
                api.setMonitorConfiguration(e.sender.Monitor);
        });

        function onError(data) {
            alert(data.statusText);
        }

        return {
            ViewModel: viewModel
        }
    });