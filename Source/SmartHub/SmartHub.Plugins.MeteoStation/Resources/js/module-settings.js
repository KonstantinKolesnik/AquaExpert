define(
	['app', 'webapp/meteostation/module-settings-model', 'webapp/meteostation/module-settings-view'],
	function (application, models, views) {
	    var module = {
	        setConfiguration: function () {
	            models.setConfiguration(models.ViewModel.Configuration);
	        },
	        reload: function () {
	            models.ViewModel.update(function () {
	                var view = new views.LayoutView();
	                view.on('Configuration:set', module.setConfiguration);

	                application.setContentView(view);

	                view.bindModel(models.ViewModel);
	            });
	        }
	    };

	    return {
	        start: function () {
	            module.reload();
	        }
	    };
	});