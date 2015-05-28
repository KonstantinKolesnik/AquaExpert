
define(['jquery'], function ($) {
    var api = {
        getSensor: function (id, onComplete) {
            $.getJSON('/api/mysensors/sensor', { id: id })
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        getSensorValues: function (nodeNo, sensorNo, hours, onComplete) {
            $.getJSON('/api/mysensors/sensorvalues', { nodeNo: nodeNo, sensorNo: sensorNo, hours: hours })
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },

        getConfiguration: function (onComplete) {
            $.getJSON('/api/meteostation/configuration')
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
        Configuration: null,

        SensorTemperatureInner: null,
        SensorHumidityInner: null,
        SensorTemperatureOuter: null,
        SensorHumidityOuter: null,
        SensorAtmospherePressure: null,
        SensorForecast: null,

        SensorValues: [],

        update: function (onComplete) {
            var me = this;

            me.SensorValues = [];

            api.getConfiguration(function (data) {
                me.set("Configuration", data);

                api.getSensor(me.Configuration.SensorTemperatureInnerID, function (data) {
                    me.set("SensorTemperatureInner", data);

                    api.getSensor(me.Configuration.SensorHumidityInnerID, function (data) {
                        me.set("SensorHumidityInner", data);

                        api.getSensor(me.Configuration.SensorTemperatureOuterID, function (data) {
                            me.set("SensorTemperatureOuter", data);

                            api.getSensor(me.Configuration.SensorHumidityOuterID, function (data) {
                                me.set("SensorHumidityOuter", data);

                                api.getSensor(me.Configuration.SensorForecastID, function (data) {
                                    me.set("SensorForecast", data);

                                    api.getSensor(me.Configuration.SensorAtmospherePressureID, function (data) {
                                        me.set("SensorAtmospherePressure", data);

                                        getSensorValues(me.SensorTemperatureInner, "TI", function () {
                                            getSensorValues(me.SensorHumidityInner, "HI", function () {
                                                getSensorValues(me.SensorTemperatureOuter, "TO", function () {
                                                    getSensorValues(me.SensorHumidityOuter, "HO", function () {
                                                        getSensorValues(me.SensorAtmospherePressure, "P", function () {
                                                            getSensorValues(me.SensorForecast, "FC", function () {
                                                                if (onComplete)
                                                                    onComplete();
                                                            });
                                                        });
                                                    });
                                                });
                                            });
                                        });
                                    });
                                });
                            });
                        });
                    });
                });
            });

            function getSensorValues(sensor, fieldName, onComplete) {
                if (sensor)
                    api.getSensorValues(sensor.NodeNo, sensor.SensorNo, 24, function (data) {
                        data = data || [];
                        $.each(data, function (idx, sv) {
                            sv[fieldName] = sv.Value;
                            me.SensorValues.push(sv);
                        });

                        if (onComplete)
                            onComplete();
                    });
                else
                    if (onComplete)
                        onComplete();
            }
        },
        SignalRReceiveHandler: function (model, data) {
            var me = model;

            if (data.MsgId == "SensorValue") {
                data.Data.TimeStamp = new Date(data.Data.TimeStamp);

                if (!checkFromSensor(data.Data, me.SensorTemperatureInner, "TI"))
                    if (!checkFromSensor(data.Data, me.SensorHumidityInner, "HI"))
                        if (!checkFromSensor(data.Data, me.SensorTemperatureOuter, "TO"))
                            if (!checkFromSensor(data.Data, me.SensorHumidityOuter, "HO"))
                                if (!checkFromSensor(data.Data, me.SensorAtmospherePressure, "P"))
                                    if (!checkFromSensor(data.Data, me.SensorForecast, "FC"))
                                        return;
            }

            function checkFromSensor(sv, sensor, fieldName) {
                var isFromSensor = (sensor.NodeNo == sv.NodeNo && sensor.SensorNo == sv.SensorNo);

                if (isFromSensor) {
                    sensor.set("SensorValueValue", sv.Value);
                    sensor.set("SensorValueTimeStamp", sv.TimeStamp);

                    sv[fieldName] = sv.Value;
                    me.SensorValues.push(sv);
                }

                return isFromSensor;
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