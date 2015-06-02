
define(['jquery'], function ($) {
    var api = {
        addMonitor: function (name, sensorId, isVisible, onComplete) {
            $.post('/api/aquacontroller/monitor/add', { name: name, sensorId: sensorId, isVisible: isVisible })
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        setMonitorName: function (id, name, onComplete) {
            $.post('/api/aquacontroller/monitor/setname', { id: id, name: name })
                .done(function (data) {
                    if (onComplete)
                        onComplete(data);
                })
                .fail(function (data) {
                    onError(data);
                });
        },
        setMonitorIsVisible: function (id, isVisible, onComplete) {
            $.post('/api/aquacontroller/monitor/setisvisible', { id: id, isVisible: isVisible })
                .done(function (data) {
                    if (onComplete)
                        onComplete(data);
                })
                .fail(function (data) {
                    onError(data);
                });
        },
        deleteMonitor: function (id, onComplete) {
            $.post('/api/aquacontroller/monitor/delete', { id: id })
                .done(function (data) {
                    if (onComplete)
                        onComplete(data);
                })
                .fail(function (data) {
                    onError(data);
                });
        },

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

        addMonitor: api.addMonitor,
        setMonitorName: api.setMonitorName,
        setMonitorIsVisible: api.setMonitorIsVisible,
        deleteMonitor: api.deleteMonitor,
        setHeaterControllerConfiguration: api.setHeaterControllerConfiguration
    };
});