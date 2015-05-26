define(
	['app', 'webapp/meteostation/module-main-model', 'webapp/meteostation/module-main-view'],
	function (application, models, views) {
	    var module = {
	        reload: function () {
	            if (application.SignalRReceivers.indexOf(models.ViewModel) == -1)
	                application.SignalRReceivers.push(models.ViewModel);

	            models.ViewModel.update(function () {
	                var view = new views.LayoutView();
	                view.viewModel = models.ViewModel;
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