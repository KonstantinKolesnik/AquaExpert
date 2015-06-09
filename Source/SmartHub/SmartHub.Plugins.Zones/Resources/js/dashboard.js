define(
	['app', 'webapp/zones/dashboard-model', 'webapp/zones/dashboard-view'],
	function (application, models, views) {
	    var module = {
	        reload: function () {
	            if (application.SignalRReceivers.indexOf(models.ViewModel) == -1)
	                application.SignalRReceivers.push(models.ViewModel);

	            models.ViewModel.update(function () {
	                var view = new views.LayoutView({ viewModel: models.ViewModel });
	                view.on('zone:select', function (item) {
	                    models.ViewModel.set("Zone", item);
	                });
	                //view.on('graphs:show', function () {
	                //    application.navigate('webapp/management/graphs'/*, param1*/);
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