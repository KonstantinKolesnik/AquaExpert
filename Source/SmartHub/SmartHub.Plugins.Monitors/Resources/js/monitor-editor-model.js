
define(['jquery'],
    function ($) {
        var api = {
            getMonitor: function (id, onComplete) {
                $.getJSON('/api/monitors/get/dashboard', { id: id })
                    .done(function (data) {
                        if (data)
                            data.Configuration = JSON.parse(data.Configuration);

                        if (onComplete)
                            onComplete(data);
                    })
                    .fail(function (data) {
                        onError(data);
                    });
            },
            setMonitorConfiguration: function (id, config, onComplete) {
                $.post('/api/monitors/setconfiguration', { id: id, config: JSON.stringify(config) })
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
            Monitor: null,
            update: function (id, onComplete) {
                var me = this;

                api.getMonitor(id, function (data) {
                    me.set("Monitor", data);

                    if (onComplete)
                        onComplete();
                });
            }
        });
        viewModel.bind("change", function (e) {
            if (e.field.indexOf("Monitor.Configuration") > -1)
                api.setMonitorConfiguration(e.sender.Monitor.Id, e.sender.Monitor.Configuration);
        });

        function onError(data) {
            alert(data.statusText);
        }

        return {
            ViewModel: viewModel
        }
    });