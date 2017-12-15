define(
	['app', 'webapp/meteostation/settings-model', 'webapp/meteostation/settings-view'],
	function (application, models, views) {
	    var module = {
	        reload: function () {
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