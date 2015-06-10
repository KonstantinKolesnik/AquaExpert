define(
	['app', 'webapp/zones/dashboard-model', 'webapp/zones/dashboard-view'],
	function (application, models, views) {
	    var module = {
	        showZone: function (id) {
	            application.navigate('webapp/zones/zone', id);
	        },
	        reload: function () {
	            var view = new views.LayoutView({ viewModel: models.ViewModel });
	            view.on('zone:show', module.showZone);
	            application.setContentView(view);
	        }
	    };

	    return {
	        start: function () {
	            module.reload();
	        }
	    };
	});