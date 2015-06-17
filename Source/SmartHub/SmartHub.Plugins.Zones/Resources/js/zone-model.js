
define(['jquery'],
    function ($) {
        var api = {
            getZone: function (id, onComplete) {
                $.getJSON('/api/zones/get/dashboard', { id: id })
                    .done(function (data) {
                        if (onComplete)
                            onComplete(data);
                    })
                    .fail(function (data) {
                        onError(data);
                    });
            },
            runScript: function (scriptId, onComplete) {
                $.post('/api/scripts/run', { scriptId: scriptId })
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
            Zone: null,
            update: function (id, onComplete) {
                var me = this;

                api.getZone(id, function (data) {
                    me.set("Zone", data);

                    if (onComplete)
                        onComplete();
                });
            },
            SignalRReceiveHandler: function (model, data) {
                if (data.MsgId == "SensorValue") {
                    data.Data.TimeStamp = new Date(data.Data.TimeStamp);
                    var sv = data.Data;

                    if (model.Zone) {
                        $.each(model.Zone.MonitorsList, function (idx, monitor) {
                            if (monitor.Sensor.NodeNo == sv.NodeNo && monitor.Sensor.SensorNo == sv.SensorNo) {
                                monitor.Sensor.set("SensorValueValue", sv.Value);
                                monitor.Sensor.set("SensorValueTimeStamp", sv.TimeStamp);

                                monitor.SensorValues.splice(0, 1);
                                monitor.SensorValues.push(sv);
                            }
                        });

                        $.each(model.Zone.ControllersList, function (idx, monitor) {
                            // if ismymessage
                        });
                    }
                }
            }
        });

        function onError(data) {
            alert(data.statusText);
        }

        return {
            ViewModel: viewModel,

            runScript: api.runScript
        }
    });