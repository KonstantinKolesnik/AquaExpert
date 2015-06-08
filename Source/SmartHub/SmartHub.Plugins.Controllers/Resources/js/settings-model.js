
define(['jquery'], function ($) {
    var api = {
        addController: function (name, type, onComplete) {
            $.post('/api/controllers/add', { name: name, type: type })
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        setControllerName: function (id, name, onComplete) {
            $.post('/api/controllers/setname', { id: id, name: name })
                .done(function (data) {
                    if (onComplete)
                        onComplete(data);
                })
                .fail(function (data) {
                    onError(data);
                });
        },
        deleteController: function (id, onComplete) {
            $.post('/api/controllers/delete', { id: id })
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

        addController: api.addController,
        setControllerName: api.setControllerName,
        deleteController: api.deleteController
    };
});