
define(['jquery'], function ($) {
    var api = {
        addMonitor: function (name, sensorId, type, onComplete) {
            $.post('/api/monitors/add', { name: name, sensorId: sensorId, type: type })
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        setMonitorName: function (id, name, onComplete) {
            $.post('/api/monitors/setname', { id: id, name: name })
                .done(function (data) {
                    if (onComplete)
                        onComplete(data);
                })
                .fail(function (data) {
                    onError(data);
                });
        },
        deleteMonitor: function (id, onComplete) {
            $.post('/api/monitors/delete', { id: id })
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
        deleteMonitor: api.deleteMonitor
    };
});