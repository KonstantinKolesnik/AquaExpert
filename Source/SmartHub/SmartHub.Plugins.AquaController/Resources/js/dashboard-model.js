
define(['jquery'], function ($) {
    var api = {
        getMonitors: function (onComplete) {
            $.getJSON('/api/aquacontroller/monitor/listvisible')
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        getControllers: function (onComplete) {
            $.getJSON('/api/aquacontroller/controller/listvisible')
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
    };

    var viewModel = kendo.observable({
        Monitors: [],
        Controllers: [],
        update: function (onComplete) {
            var me = this;

            me.Monitors = [];
            me.Controllers = [];

            api.getMonitors(function (data) {
                me.set("Monitors", data);

                api.getControllers(function (data) {
                    me.set("Controllers", data);

                    if (onComplete)
                        onComplete();
                });
            });
        },
        SignalRReceiveHandler: function (model, data) {
            if (data.MsgId == "SensorValue") {
                data.Data.TimeStamp = new Date(data.Data.TimeStamp);

                $.each(model.Monitors, function (idx, monitor) {
                    var sv = data.Data;

                    if (monitor.Sensor.NodeNo == sv.NodeNo && monitor.Sensor.SensorNo == sv.SensorNo) {
                        monitor.Sensor.set("SensorValueValue", sv.Value);
                        monitor.Sensor.set("SensorValueTimeStamp", sv.TimeStamp);

                        monitor.SensorValues.splice(0, 1);
                        monitor.SensorValues.push(sv);
                    }
                });
            }
        }
    });

    function onError(data) {
        alert(data.statusText);
    }

    return {
        ViewModel: viewModel
    };
});