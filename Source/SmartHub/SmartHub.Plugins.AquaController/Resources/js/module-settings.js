define(
	['app', 'webapp/aquacontroller/module-settings-model', 'webapp/aquacontroller/module-settings-view'],
	function (application, models, views) {
	    var module = {
	        setConfiguration: function () {
	            models.setConfiguration(models.ViewModel.Configuration);
	        },
	        reload: function () {
	            models.ViewModel.update(function () {
	                var view = new views.LayoutView();
	                view.on('configuration:set', module.setConfiguration);

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