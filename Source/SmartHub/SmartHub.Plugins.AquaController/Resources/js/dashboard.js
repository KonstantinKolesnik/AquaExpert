define(
	['app', 'webapp/aquacontroller/dashboard-model', 'webapp/aquacontroller/dashboard-view'],
	function (application, models, views) {
	    var module = {
	        //setHeaterControllerConfiguration: function () {
	        //    models.setHeaterControllerConfiguration(models.ViewModel.HeaterControllerConfiguration);
	        //},

	        reload: function () {
	            if (application.SignalRReceivers.indexOf(models.ViewModel) == -1)
	                application.SignalRReceivers.push(models.ViewModel);

	            models.ViewModel.update(function () {
	                var view = new views.LayoutView({ viewModel: models.ViewModel });
	                //view.on('heaterControllerConfiguration:set', module.setHeaterControllerConfiguration);
	                //view.on('graphs: show', function () {
	                //    application.navigate('webapp/aquacontroller/module-graphs'/*, param1*/);
	                //});
	                application.setContentView(view);
	            });
	        }
	    };

	    return {
	        start: function () {
	            module.reload();
	        }
	    };
	});