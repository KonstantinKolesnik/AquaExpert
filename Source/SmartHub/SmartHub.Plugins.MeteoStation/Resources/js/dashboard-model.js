
define(['jquery'], function ($) {
    var api = {
        getMonitors: function (onComplete) {
            $.getJSON('/api/meteostation/monitor/list')
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
        Monitors: [],
        update: function (onComplete) {
            var me = this;

            me.Monitors = [];

            api.getMonitors(function (data) {
                me.set("Monitors", data);

                if (onComplete)
                    onComplete();
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