
define(['jquery'], function ($) {
    var api = {
        getSensorsDataSource: function (type, onComplete) {
            $.getJSON('/api/aquacontroller/sensorsDataSource', { type: type })
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        getConfiguration: function (onComplete) {
            $.getJSON('/api/aquacontroller/configuration')
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        setConfiguration: function (sc, onComplete) {
            $.post('/api/aquacontroller/configuration/set', { sc: JSON.stringify(sc) })
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

        Configuration: null,

        update: function (onComplete) {
            var me = this;

            //Temperature = 6,        // Temperature sensor
            //Humidity = 7,           // Humidity sensor
            //Barometer = 8,          // Barometer sensor (Pressure)

            api.getSensorsDataSource(6, function (data) {
                me.set("SensorsTemperatureDataSource", data);

                api.getSensorsDataSource(7, function (data) {
                    me.set("SensorsHumidityDataSource", data);

                    api.getSensorsDataSource(8, function (data) {
                        me.set("SensorsBarometerDataSource", data);

                        api.getSensorsDataSource(8, function (data) {
                            me.set("SensorsForecastDataSource", data);

                            api.getConfiguration(function (sc) {
                                me.set("Configuration", sc);

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
        //alert(data.responseJSON.ExceptionMessage);
        //alert(data.statusText);
        alert(data.responseText);
    }

    return {
        ViewModel: viewModel,
        setConfiguration: api.setConfiguration
    };
});