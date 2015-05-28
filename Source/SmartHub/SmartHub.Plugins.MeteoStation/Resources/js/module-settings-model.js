
define(['jquery'], function ($) {
    var api = {
        getSensorsByType: function (type, onComplete) {
            $.getJSON('/api/meteostation/sensorsByType', { type: type })
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        getSensorsConfiguration: function (onComplete) {
            $.getJSON('/api/meteostation/configuration')
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        setSensorsConfiguration: function (sc, onComplete) {
            $.post('/api/meteostation/configuration/set', { sc: JSON.stringify(sc) })
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
        SensorsHumidityDataSource: [],
        SensorsBarometerDataSource: [],
        SensorsForecastDataSource: [],

        SensorsConfiguration: null,

        update: function (onComplete) {
            var me = this;

            //Temperature = 6,        // Temperature sensor
            //Humidity = 7,           // Humidity sensor
            //Barometer = 8,          // Barometer sensor (Pressure)

            api.getSensorsByType(6, function (data) {
                me.set("SensorsTemperatureDataSource", data);

                api.getSensorsByType(7, function (data) {
                    me.set("SensorsHumidityDataSource", data);

                    api.getSensorsByType(8, function (data) {
                        me.set("SensorsBarometerDataSource", data);

                        api.getSensorsByType(8, function (data) {
                            me.set("SensorsForecastDataSource", data);

                            api.getSensorsConfiguration(function (sc) {
                                me.set("SensorsConfiguration", sc);

                                if (onComplete)
                                    onComplete();
                            });
                        });
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
        setSensorsConfiguration: api.setSensorsConfiguration
    };
});