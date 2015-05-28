
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
    };

    var viewModel = kendo.observable({
        HeaterControllerConfiguration: null,
        HeaterSensorTemperature: null,
        HeaterSensorSwitch: null,



        SensorValues: [],

        update: function (onComplete) {
            var me = this;

            me.SensorValues = [];

            updateHeater(function () {
                if (onComplete)
                    onComplete();
            });

            function updateHeater(onComplete) {
                api.getHeaterControllerConfiguration(function (data) {
                    me.set("HeaterControllerConfiguration", data);

                    api.getSensor(me.HeaterControllerConfiguration.SensorTemperatureID, function (data) {
                        me.set("HeaterSensorTemperature", data);

                        api.getSensor(me.HeaterControllerConfiguration.SensorSwitchID, function (data) {
                            me.set("HeaterSensorSwitch", data);

                            getSensorValues(me.HeaterSensorTemperature, "TI", function () {
                                getSensorValues(me.HeaterSensorSwitch, "TI", function () {
                                    if (onComplete)
                                        onComplete();
                                });
                            });
                        });
                    });
                });
            }

            function getSensorValues(sensor, fieldName, onComplete) {
                if (sensor)
                    api.getSensorValues(sensor.NodeNo, sensor.SensorNo, 24, function (data) {
                        data = data || [];
                        $.each(data, function (idx, sv) {
                            //sv[fieldName] = sv.Value;
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

            var me = model;

            if (data.MsgId == "SensorValue") {
                data.Data.TimeStamp = new Date(data.Data.TimeStamp);

                if (!checkFromSensor(data.Data, me.HeaterSensorTemperature, ""))
                    if (!checkFromSensor(data.Data, me.HeaterSensorSwitch, ""))
                        return;
            }

            function checkFromSensor(sv, sensor, fieldName) {
                var isFromSensor = (sensor.NodeNo == sv.NodeNo && sensor.SensorNo == sv.SensorNo);

                if (isFromSensor) {
                    sensor.set("SensorValueValue", sv.Value);
                    sensor.set("SensorValueTimeStamp", sv.TimeStamp);

                    //sv[fieldName] = sv.Value;
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