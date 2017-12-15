define(
	['app', 'webapp/meteostation/dashboard-model', 'webapp/meteostation/dashboard-view'],
	function (application, models, views) {
	    var module = {
	        reload: function () {
	            if (application.SignalRReceivers.indexOf(models.ViewModel) == -1)
	                application.SignalRReceivers.push(models.ViewModel);

	            models.ViewModel.update(function () {
	                var view = new views.LayoutView({ viewModel: models.ViewModel });
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