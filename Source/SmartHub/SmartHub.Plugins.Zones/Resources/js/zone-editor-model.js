
define(['jquery'],
    function ($) {
        var api = {
            getZone: function (id, onComplete) {
                $.getJSON('/api/zones/get', { id: id })
                    .done(function (data) {
                        if (data) {
                            data.MonitorsList = JSON.parse(data.MonitorsList);
                            data.ControllersList = JSON.parse(data.ControllersList);
                            data.ScriptsList = JSON.parse(data.ScriptsList);
                        }

                        if (onComplete)
                            onComplete(data);
                    })
                    .fail(function (data) {
                        onError(data);
                    });
            },
            setZoneConfiguration: function (zone, onComplete) {
                $.post('/api/zones/setconfiguration', {
                    id: zone.Id,
                    monitorsList: JSON.stringify(zone.MonitorsList),
                    controllersList: JSON.stringify(zone.ControllersList),
                    scriptsList: JSON.stringify(zone.ScriptsList),
                    //graphsList: JSON.stringify(zone.GraphsList)
                })
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
            Zone: null,
            update: function (id, onComplete) {
                var me = this;

                api.getZone(id, function (data) {
                    me.set("Zone", data);

                    if (onComplete)
                        onComplete();
                });
            }
        });
        viewModel.bind("change", function (e) {
            if (e.field.indexOf("Zone.") > -1)
                api.setZoneConfiguration(e.sender.Zone);
        });

        function onError(data) {
            alert(data.statusText);
        }

        return {
            ViewModel: viewModel
        }
    });