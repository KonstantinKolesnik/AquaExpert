
define(['jquery'], function ($) {
    var api = {
        getZones: function (onComplete) {
            $.getJSON('/api/zones/list')
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
        Zones: [],
        Zone: null,
        update: function (onComplete) {
            var me = this;

            me.Zones = [];

            api.getZones(function (data) {
                me.set("Zones", data);

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



                }
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