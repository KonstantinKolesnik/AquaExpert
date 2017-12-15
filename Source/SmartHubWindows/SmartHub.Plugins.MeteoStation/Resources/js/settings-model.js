
define(['jquery'], function ($) {
    var api = {
        getConfiguration: function (onComplete) {
            $.getJSON('/api/meteostation/configuration')
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        setConfiguration: function (conf, onComplete) {
            $.post('/api/meteostation/configuration/set', { conf: JSON.stringify(conf) })
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
        Configuration: null,
        update: function (onComplete) {
            var me = this;

            api.getConfiguration(function (data) {
                me.set("Configuration", data);

                if (onComplete)
                    onComplete();
            });
        }
    });
    viewModel.bind("change", function (e) {
        if (e.field.indexOf("Configuration.") > -1)
            api.setConfiguration(e.sender.Configuration);
    });

    function onError(data) {
        alert(data.statusText);
    }

    return {
        ViewModel: viewModel,
        setConfiguration: api.setConfiguration
    };
});