
define(['jquery'], function ($) {
    var api = {
        addZone: function (name, onComplete) {
            $.post('/api/zones/add', { name: name })
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        setZoneName: function (id, name, onComplete) {
            $.post('/api/zones/setname', { id: id, name: name })
                .done(function (data) {
                    if (onComplete)
                        onComplete(data);
                })
                .fail(function (data) {
                    onError(data);
                });
        },
        deleteZone: function (id, onComplete) {
            $.post('/api/zones/delete', { id: id })
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

        addZone: api.addZone,
        setZoneName: api.setZoneName,
        deleteZone: api.deleteZone,
    };
});