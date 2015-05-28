
define(['jquery'], function ($) {
    var api = {
        getSensorsByType: function (type, onComplete) {
            $.getJSON('/api/mysensors/sensorsByType', { type: type })
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },

        getHeaterControllerConfiguration: function (onComplete) {
            $.getJSON('/api/aquacontroller/heatercontroller/configuration')
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        setHeaterControllerConfiguration: function (conf, onComplete) {
            $.post('/api/aquacontroller/heatercontroller/configuration/set', { conf: JSON.stringify(conf) })
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

        HeaterControllerConfiguration: null,



        update: function (onComplete) {
            var me = this;

            //Switch = 3,             // Switch Actuator (on/off)
            //Temperature = 6,        // Temperature sensor

            api.getSensorsByType(6, function (data) {
                me.set("SensorsTemperatureDataSource", data);

                api.getSensorsByType(3, function (data) {
                    me.set("SensorsSwitchDataSource", data);

                    api.getHeaterControllerConfiguration(function (data) {
                        me.set("HeaterControllerConfiguration", data);

                        if (onComplete)
                            onComplete();
                    });

                });
            });
        }
    });

    function onError(data) {
        alert(data.statusText);
    }

    return {
        ViewModel: viewModel,
        setHeaterControllerConfiguration: api.setHeaterControllerConfiguration
    };
});