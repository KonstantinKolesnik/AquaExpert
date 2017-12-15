
define(['jquery'], function ($) {
    var api = {
        addInformer: function (name, sensorDisplayId, onComplete) {
            $.post('/api/informers/add', { name: name, sensorDisplayId: sensorDisplayId })
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        setInformerName: function (id, name, onComplete) {
            $.post('/api/informers/setname', { id: id, name: name })
                .done(function (data) {
                    if (onComplete)
                        onComplete(data);
                })
                .fail(function (data) {
                    onError(data);
                });
        },
        deleteInformer: function (id, onComplete) {
            $.post('/api/informers/delete', { id: id })
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

        addInformer: api.addInformer,
        setInformerName: api.setInformerName,
        deleteInformer: api.deleteInformer,
    };
});