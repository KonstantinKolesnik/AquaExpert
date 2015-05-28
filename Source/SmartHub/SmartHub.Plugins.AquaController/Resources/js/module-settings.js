define(
	['app', 'webapp/aquacontroller/module-settings-model', 'webapp/aquacontroller/module-settings-view'],
	function (application, models, views) {
	    var module = {
	        setHeaterControllerConfiguration: function () {
	            models.setHeaterControllerConfiguration(models.ViewModel.HeaterControllerConfiguration);
	        },



	        reload: function () {
	            models.ViewModel.update(function () {
	                var view = new views.LayoutView();
	                view.on('heaterControllerConfiguration:set', module.setHeaterControllerConfiguration);




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