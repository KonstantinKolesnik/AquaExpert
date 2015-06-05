
define(['jquery'],
    function ($) {
        var api = {
            getController: function (id, onComplete) {
                $.getJSON('/api/management/controller', { id: id })
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
            setControllerConfiguration: function (id, config, onComplete) {
                $.post('/api/management/controller/setconfiguration', { id: id, config: JSON.stringify(config) })
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
            Controller: null,
            update: function (id, onComplete) {
                var me = this;

                api.getController(id, function (data) {
                    me.set("Controller", data);

                    if (onComplete)
                        onComplete();
                });
            }
        });
        viewModel.bind("change", function (e) {
            if (e.field.indexOf("Controller.Configuration") > -1)
                api.setControllerConfiguration(e.sender.Controller.Id, e.sender.Controller.Configuration);
        });

        function onError(data) {
            alert(data.statusText);
        }

        return {
            ViewModel: viewModel
        }
    });