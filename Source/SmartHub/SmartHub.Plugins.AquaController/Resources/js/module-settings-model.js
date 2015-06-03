
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

        addController: function (name, type, isVisible, onComplete) {
            $.post('/api/aquacontroller/controller/add', { name: name, type: type, isVisible: isVisible })
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        setControllerName: function (id, name, onComplete) {
            $.post('/api/aquacontroller/controller/setname', { id: id, name: name })
                .done(function (data) {
                    if (onComplete)
                        onComplete(data);
                })
                .fail(function (data) {
                    onError(data);
                });
        },
        setControllerIsVisible: function (id, isVisible, onComplete) {
            $.post('/api/aquacontroller/controller/setisvisible', { id: id, isVisible: isVisible })
                .done(function (data) {
                    if (onComplete)
                        onComplete(data);
                })
                .fail(function (data) {
                    onError(data);
                });
        },
        deleteController: function (id, onComplete) {
            $.post('/api/aquacontroller/controller/delete', { id: id })
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
        update: function (onComplete) {
            var me = this;

            //Switch = 3,             // Switch Actuator (on/off)
            //Temperature = 6,        // Temperature sensor

            //api.getSensorsByType(6, function (data) {
            //    me.set("SensorsTemperatureDataSource", data);

            //    api.getSensorsByType(3, function (data) {
            //        me.set("SensorsSwitchDataSource", data);

            //        api.getHeaterControllerConfiguration(function (data) {
            //            me.set("HeaterControllerConfiguration", data);

                        if (onComplete)
                            onComplete();
            //        });

            //    });
            //});
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

        addController: api.addController,
        setControllerName: api.setControllerName,
        setControllerIsVisible: api.setControllerIsVisible,
        deleteController: api.deleteController,
    };
});