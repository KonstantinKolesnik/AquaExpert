define(
	['app', 'webapp/aquacontroller/module-settings-model', 'webapp/aquacontroller/module-settings-view'],
	function (application, models, views) {
	    var module = {
	        setTemperatureControllerConfiguration: function () {
	            models.setTemperatureControllerConfiguration(models.ViewModel.TemperatureControllerConfiguration);
	        },



	        reload: function () {
	            models.ViewModel.update(function () {
	                var view = new views.LayoutView();
	                view.on('temperatureControllerConfiguration:set', module.setTemperatureControllerConfiguration);




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