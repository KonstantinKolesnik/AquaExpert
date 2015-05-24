
define(['jquery'], function ($) {
    var api = {
        getSensors: function (type, onComplete) {
            $.getJSON('/api/meteostation/sensors', { type: type })
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        getSensorsConfiguration: function (onComplete) {
            $.getJSON('/api/meteostation/sensorsCofiguration')
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        setSensorsConfiguration: function (sc, onComplete) {
            $.post('/api/meteostation/setSensorsCofiguration', { Value: JSON.stringify(sc) })
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
        SensorsTemperature: [],
        SensorsHumidity: [],
        SensorsBarometer: [],
        SensorsConfiguration: null,

        update: function (onComplete) {
            var me = this;

            //Temperature = 6,        // Temperature sensor
            //Humidity = 7,           // Humidity sensor
            //Barometer = 8,          // Barometer sensor (Pressure)

            api.getSensors(6, function (data) {
                me.set("SensorsTemperature", data);

                api.getSensors(7, function (data) {
                    me.set("SensorsHumidity", data);

                    api.getSensors(8, function (data) {
                        me.set("SensorsBarometer", data);

                        api.getSensorsConfiguration(function (sc) {
                            me.set("SensorsConfiguration", sc);

                            if (onComplete)
                                onComplete();
                        });
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
        setSensorsConfiguration: api.setSensorsConfiguration
    };
});