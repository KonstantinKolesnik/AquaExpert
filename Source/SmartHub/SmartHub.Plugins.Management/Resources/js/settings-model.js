
define(['jquery'], function ($) {
    var api = {
        addMonitor: function (name, sensorId, isVisible, onComplete) {
            $.post('/api/management/monitor/add', { name: name, sensorId: sensorId, isVisible: isVisible })
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        setMonitorName: function (id, name, onComplete) {
            $.post('/api/management/monitor/setname', { id: id, name: name })
                .done(function (data) {
                    if (onComplete)
                        onComplete(data);
                })
                .fail(function (data) {
                    onError(data);
                });
        },
        deleteMonitor: function (id, onComplete) {
            $.post('/api/management/monitor/delete', { id: id })
                .done(function (data) {
                    if (onComplete)
                        onComplete(data);
                })
                .fail(function (data) {
                    onError(data);
                });
        },

        addController: function (name, type, isVisible, onComplete) {
            $.post('/api/management/controller/add', { name: name, type: type, isVisible: isVisible })
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        setControllerName: function (id, name, onComplete) {
            $.post('/api/management/controller/setname', { id: id, name: name })
                .done(function (data) {
                    if (onComplete)
                        onComplete(data);
                })
                .fail(function (data) {
                    onError(data);
                });
        },
        deleteController: function (id, onComplete) {
            $.post('/api/management/controller/delete', { id: id })
                .done(function (data) {
                    if (onComplete)
                        onComplete(data);
                })
                .fail(function (data) {
                    onError(data);
                });
        },

        addZone: function (name, onComplete) {
            $.post('/api/management/zone/add', { name: name })
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        setZoneName: function (id, name, onComplete) {
            $.post('/api/management/zone/setname', { id: id, name: name })
                .done(function (data) {
                    if (onComplete)
                        onComplete(data);
                })
                .fail(function (data) {
                    onError(data);
                });
        },
        deleteZone: function (id, onComplete) {
            $.post('/api/management/zone/delete', { id: id })
                .done(function (data) {
                    if (onComplete)
                        onComplete(data);
                })
                .fail(function (data) {
                    onError(data);
                });
        }
    };

    var viewModel = kendo.observable({ });

    function onError(data) {
        alert(data.statusText);
    }

    return {
        ViewModel: viewModel,

        addMonitor: api.addMonitor,
        setMonitorName: api.setMonitorName,
        deleteMonitor: api.deleteMonitor,

        addController: api.addController,
        setControllerName: api.setControllerName,
        deleteController: api.deleteController,

        addZone: api.addZone,
        setZoneName: api.setZoneName,
        deleteZone: api.deleteZone,
    };
});