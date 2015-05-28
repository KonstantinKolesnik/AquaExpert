
define(['jquery'], function ($) {
    var api = {
        getSensorsByType: function (type, onComplete) {
            $.getJSON('/api/aquacontroller/sensorsByType', { type: type })
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },

        getTemperatureControllerConfiguration: function (onComplete) {
            $.getJSON('/api/aquacontroller/configuration/temperaturecontroller')
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        setTemperatureControllerConfiguration: function (conf, onComplete) {
            $.post('/api/aquacontroller/configuration/temperaturecontroller/set', { conf: JSON.stringify(conf) })
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
        SensorsTemperatureDataSource: [],
        SensorsSwitchDataSource: [],

        TemperatureControllerConfiguration: null,

        update: function (onComplete) {
            var me = this;

            //Switch = 3,             // Switch Actuator (on/off)
            //Temperature = 6,        // Temperature sensor

            api.getSensorsByType(6, function (data) {
                me.set("SensorsTemperatureDataSource", data);

                api.getSensorsByType(3, function (data) {
                    me.set("SensorsSwitchDataSource", data);

                    api.getTemperatureControllerConfiguration(function (data) {
                        me.set("TemperatureControllerConfiguration", data);

                        if (onComplete)
                            onComplete();
                    });
                });
            });
        }
    });

    function onError(data) {
        //alert(data.responseJSON.ExceptionMessage);
        //alert(data.statusText);
        alert(data.responseText);
    }

    return {
        ViewModel: viewModel,
        setTemperatureControllerConfiguration: api.setTemperatureControllerConfiguration
    };
});