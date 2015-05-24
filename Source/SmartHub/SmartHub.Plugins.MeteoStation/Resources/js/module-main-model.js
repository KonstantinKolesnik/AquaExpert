
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
        }
    };




    var viewModel = kendo.observable({
        SensorsTemperature: [],
        SensorsHumidyty: [],
        SensorsBarometer: [],

        SensorTemperatureInnerID: null,

        update: function (onComplete) {
            var me = this;

            //Temperature = 6,        // Temperature sensor
            //Humidity = 7,           // Humidity sensor
            //Barometer = 8,          // Barometer sensor (Pressure)


            api.getSensors(6, function (data) {
                me.set("SensorsTemperature", data);

                api.getSensors(7, function (data) {
                    me.set("SensorsHumidyty", data);

                    api.getSensors(8, function (data) {
                        me.set("SensorsBarometer", data);

                        //debugger;
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

        //getNodes: api.getNodes,
        //setNodeName: api.setNodeName,
        //deleteNode: api.deleteNode,
        //getSensors: api.getSensors,
        //setSensorName: api.setSensorName,
        //deleteSensor: api.deleteSensor,
        //getUnitSystem: api.getUnitSystem,
        //setUnitSystem: api.setUnitSystem,
        //getBatteryLevels: api.getBatteryLevels,
        //getSensorValues: api.getSensorValues
    };
});