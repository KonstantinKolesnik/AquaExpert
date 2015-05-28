define(
	['app', 'webapp/aquacontroller/module-main-model', 'webapp/aquacontroller/module-main-view'],
	function (application, models, views) {
	    var module = {
	        setHeaterControllerConfiguration: function () {
	            models.setHeaterControllerConfiguration(models.ViewModel.HeaterControllerConfiguration);
	        },

	        reload: function () {
	            if (application.SignalRReceivers.indexOf(models.ViewModel) == -1)
	                application.SignalRReceivers.push(models.ViewModel);

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