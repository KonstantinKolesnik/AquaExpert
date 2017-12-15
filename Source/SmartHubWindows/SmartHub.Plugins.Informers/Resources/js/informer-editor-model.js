
define(['jquery'],
    function ($) {
        var api = {
            getInformer: function (id, onComplete) {
                $.getJSON('/api/informers/get', { id: id })
                    .done(function (data) {
                        if (data)
                            data.MonitorsIds = JSON.parse(data.MonitorsIds);

                        if (onComplete)
                            onComplete(data);
                    })
                    .fail(function (data) {
                        onError(data);
                    });
            },
            setInformerConfiguration: function (informer, onComplete) {
                $.post('/api/informers/setconfiguration', {
                    id: informer.Id,
                    monitorsIds: JSON.stringify(informer.MonitorsIds),
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
            Informer: null,
            update: function (id, onComplete) {
                var me = this;

                api.getInformer(id, function (data) {
                    me.set("Informer", data);

                    if (onComplete)
                        onComplete();
                });
            }
        });
        viewModel.bind("change", function (e) {
            if (e.field.indexOf("Informer.") > -1)
                api.setInformerConfiguration(e.sender.Informer);
        });

        function onError(data) {
            alert(data.statusText);
        }

        return {
            ViewModel: viewModel
        }
    });