
define(['jquery'], function ($) {
    var api = {
        getMonitors: function ( onComplete) {
            $.getJSON('/api/aquacontroller/monitor/listvisible')
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        getControllers: function (onComplete) {
            $.getJSON('/api/aquacontroller/controller/listvisible')
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },


        //getSensor: function (id, onComplete) {
        //    $.getJSON('/api/mysensors/sensor', { id: id })
		//		.done(function (data) {
		//		    if (onComplete)
		//		        onComplete(data);
		//		})
	    //        .fail(function (data) {
	    //            onError(data);
	    //        });
        //},
        //getSensorValues: function (nodeNo, sensorNo, hours, onComplete) {
        //    $.getJSON('/api/mysensors/sensorvalues', { nodeNo: nodeNo, sensorNo: sensorNo, hours: hours })
		//		.done(function (data) {
		//		    if (onComplete)
		//		        onComplete(data);
		//		})
	    //        .fail(function (data) {
	    //            onError(data);
	    //        });
        //},

        //getHeaterControllerConfiguration: function (onComplete) {
        //    $.getJSON('/api/aquacontroller/heatercontroller/configuration')
		//		.done(function (data) {
		//		    if (onComplete)
		//		        onComplete(data);
		//		})
	    //        .fail(function (data) {
	    //            onError(data);
	    //        });
        //},
        //setHeaterControllerConfiguration: function (conf, onComplete) {
        //    $.post('/api/aquacontroller/heatercontroller/configuration/set', { conf: JSON.stringify(conf) })
		//		.done(function (data) {
		//		    if (onComplete)
		//		        onComplete(data);
		//		})
	    //        .fail(function (data) {
	    //            onError(data);
	    //        });
        //},

    };

    var viewModel = kendo.observable({
        //HeaterControllerConfiguration: null,
        //HeaterSensorTemperature: null,
        //HeaterSensorSwitch: null,


        Monitors: [],
        Controllers: [],

        //SensorValues: [],

        update: function (onComplete) {
            var me = this;

            me.Monitors = [];
            me.Controllers = [];

            updateMonitors(function () {
                //updateControllers(function () {
                    if (onComplete)
                        onComplete();
                //});
            });

            function updateMonitors(onComplete) {
                api.getMonitors(function (data) {
                    me.set("Monitors", data);

                    if (onComplete)
                        onComplete();
                });
            }
            function updateControllers(onComplete) {
                api.getControllers(function (data) {
                    me.set("Controllers", data);

                    if (onComplete)
                        onComplete();
                });
            }

            //function updateHeater(onComplete) {
            //    api.getHeaterControllerConfiguration(function (data) {
            //        me.set("HeaterControllerConfiguration", data);

            //        api.getSensor(me.HeaterControllerConfiguration.SensorTemperatureID, function (data) {
            //            me.set("HeaterSensorTemperature", data);

            //            api.getSensor(me.HeaterControllerConfiguration.SensorSwitchID, function (data) {
            //                me.set("HeaterSensorSwitch", data);

            //                getSensorValues(me.HeaterSensorTemperature, "T", function () {
            //                    getSensorValues(me.HeaterSensorSwitch, "S", function () {
            //                        if (onComplete)
            //                            onComplete();
            //                    });
            //                });
            //            });
            //        });
            //    });
            //}

            //function getSensorValues(sensor, fieldName, onComplete) {
            //    if (sensor)
            //        api.getSensorValues(sensor.NodeNo, sensor.SensorNo, 24, function (data) {
            //            data = data || [];
            //            $.each(data, function (idx, sv) {
            //                sv[fieldName] = sv.Value;
            //                me.SensorValues.push(sv);
            //            });

            //            if (onComplete)
            //                onComplete();
            //        });
            //    else
            //        if (onComplete)
            //            onComplete();
            //}
        },
        SignalRReceiveHandler: function (model, data) {
            var me = model;

            if (data.MsgId == "SensorValue") {
                data.Data.TimeStamp = new Date(data.Data.TimeStamp);

                $.each(me.Monitors, function (idx, monitor) {
                    var sv = data.Data;
                    var sensor = monitor.Sensor;

                    var isFromSensor = (sensor.NodeNo == sv.NodeNo && sensor.SensorNo == sv.SensorNo);

                    if (isFromSensor) {
                        sensor.set("SensorValueValue", sv.Value);
                        sensor.set("SensorValueTimeStamp", sv.TimeStamp);

                        monitor.SensorValues.push(sv);
                    }
                });

                //if (!checkFromSensor(data.Data, me.HeaterSensorTemperature, "T"))
                //    if (!checkFromSensor(data.Data, me.HeaterSensorSwitch, "S"))
                //        return;
            }

            //function checkFromSensor(sv, sensor, fieldName) {
            //    var isFromSensor = (sensor.NodeNo == sv.NodeNo && sensor.SensorNo == sv.SensorNo);

            //    if (isFromSensor) {
            //        sensor.set("SensorValueValue", sv.Value);
            //        sensor.set("SensorValueTimeStamp", sv.TimeStamp);

            //        sv[fieldName] = sv.Value;
            //        me.SensorValues.push(sv);
            //    }

            //    return isFromSensor;
            //}
        }
    });

    //viewModel.bind("change", function (e) {
    //    if (e.field == "HeaterControllerConfiguration.IsAutoMode")
    //        api.setHeaterControllerConfiguration(e.sender.HeaterControllerConfiguration);
    //});

    function onError(data) {
        alert(data.statusText);
    }

    return {
        ViewModel: viewModel,
        //setHeaterControllerConfiguration: api.setHeaterControllerConfiguration
    };
});