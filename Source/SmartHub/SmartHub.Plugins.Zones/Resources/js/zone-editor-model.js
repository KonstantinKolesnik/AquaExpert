
define(['jquery'],
    function ($) {
        var api = {
            getZone: function (id, onComplete) {
                $.getJSON('/api/zones/get', { id: id })
                    .done(function (data) {
                        if (data) {
                            data.MonitorsIds = JSON.parse(data.MonitorsIds);
                            data.ControllersIds = JSON.parse(data.ControllersIds);
                            data.ScriptsIds = JSON.parse(data.ScriptsIds);
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
                    monitorsIds: JSON.stringify(zone.MonitorsIds),
                    controllersIds: JSON.stringify(zone.ControllersIds),
                    scriptsIds: JSON.stringify(zone.ScriptsIds),
                    //graphsIds: JSON.stringify(zone.GraphsIds)
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