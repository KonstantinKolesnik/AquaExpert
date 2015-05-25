
define(['jquery'], function ($) {
    var api = {
        getSensor: function (id, onComplete) {
            $.getJSON('/api/meteostation/sensor', { id: id })
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
    };

    var viewModel = kendo.observable({
        SensorsConfiguration: null,

        SensorTemperatureInner: null,
        SensorHumidityInner: null,
        SensorTemperatureOuter: null,
        SensorHumidityOuter: null,
        SensorAtmospherePressure: null,

        SensorValues: [],

        update: function (onComplete) {
            var me = this;

            api.getSensorsConfiguration(function (data) {
                me.set("SensorsConfiguration", data);

                api.getSensor(me.SensorsConfiguration.SensorTemperatureInnerID, function (data) {
                    me.set("SensorTemperatureInner", data);

                    api.getSensor(me.SensorsConfiguration.SensorHumidityInnerID, function (data) {
                        me.set("SensorHumidityInner", data);

                        api.getSensor(me.SensorsConfiguration.SensorTemperatureOuterID, function (data) {
                            me.set("SensorTemperatureOuter", data);

                            api.getSensor(me.SensorsConfiguration.SensorHumidityOuterID, function (data) {
                                me.set("SensorHumidityOuter", data);

                                api.getSensor(me.SensorsConfiguration.SensorAtmospherePressureID, function (data) {
                                    me.set("SensorAtmospherePressure", data);

                                    if (onComplete)
                                        onComplete();
                                });
                            });
                        });
                    });
                });
            });
        },
        SignalRReceiveHandler: function (model, data) {
            var me = model;

            if (data.MsgId == "SensorValue") {
                data.Data.TimeStamp = new Date(data.Data.TimeStamp);

                if (!checkFromSensor(data.Data), SensorTemperatureInner)
                    if (!checkFromSensor(data.Data), SensorHumidityInner)
                        if (!checkFromSensor(data.Data), SensorTemperatureOuter)
                            if (!checkFromSensor(data.Data), SensorHumidityOuter)
                                if (!checkFromSensor(data.Data), SensorAtmospherePressure)
                                    return;
            }

            function checkFromSensor(sv, sensor) {
                var isFromSensor = sensor.NodeNo == sv.NodeNo && sensor.SensorNo == sv.SensorNo;

                if (isFromSensor) {
                    sensor.set("SensorValueValue", sv.Value);
                    sensor.set("SensorValueTimeStamp", sv.TimeStamp);

                    me.SensorValues.push(sv);
                }

                return isFromSensor;
            }
        }
    });

    function onError(data) {
        //alert(data.responseJSON.ExceptionMessage);
        //alert(data.statusText);
        alert(data.responseText);
    }

    return {
        ViewModel: viewModel
    };
});