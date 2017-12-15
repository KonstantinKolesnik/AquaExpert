define(
	['app', 'webapp/zones/settings-model', 'webapp/zones/settings-view'],
	function (application, models, views) {
	    var view;

	    var module = {
	        addZone: function (name, isVisible) {
	            if (!name) {
	                alert("Не указано имя зоны!");
	                return;
	            }

	            models.addZone(name, function () { view.refreshZonesGrid(); });
	        },
	        setZoneName: function (id, name) {
	            if (!name) {
	                alert("Не указано имя зоны!");
	                return;
	            }

	            models.setZoneName(id, name, function () { view.refreshZonesGrid(); });
	        },
	        editZone: function (id) {
	            application.navigate('webapp/zones/zone-editor', id);
	        },
	        deleteZone: function (id) {
	            models.deleteZone(id, function () { view.refreshZonesGrid(); });
	        },

	        reload: function () {
	            view = new views.LayoutView({ viewModel: models.ViewModel });
	            view.on('zone:add', module.addZone);
	            view.on('zone:setName', module.setZoneName);
	            view.on('zone:edit', module.editZone);
	            view.on('zone:delete', module.deleteZone);
	            application.setContentView(view);
	        }
	    };

	    return {
	        start: function () {
	            module.reload();
	        }
	    };
	});