﻿
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
            setZoneConfiguration: function (id, monitorsList, controllersList, scriptsList, /*graphsList,*/ onComplete) {
                $.post('/api/zones/setconfiguration', {
                    id: id,
                    monitorsList: JSON.stringify(monitorsList),
                    controllersList: JSON.stringify(controllersList),
                    scriptsList: JSON.stringify(scriptsList),
                    //graphsList: JSON.stringify(graphsList)
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
                api.setZoneConfiguration(
                    e.sender.Zone.Id,
                    e.sender.Zone.MonitorsList,
                    e.sender.Zone.ControllersList,
                    e.sender.Zone.ScriptsList
                    //e.sender.Zone.GraphsList
                    );
        });

        function onError(data) {
            alert(data.statusText);
        }

        return {
            ViewModel: viewModel
        }
    });