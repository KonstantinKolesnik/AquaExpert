define(
	['app', 'webapp/management/settings-model', 'webapp/management/settings-view'],
	function (application, models, views) {
	    var view;

	    var module = {
	        addMonitor: function (name, sensorId, isVisible) {
	            if (!name) {
	                alert("Не указано имя монитора!");
	                return;
	            }
	            if (!sensorId) {
	                alert("Не указан сенсор монитора!");
	                return;
	            }

	            models.addMonitor(name, sensorId, isVisible, function () { view.refreshMonitorsGrid(); });
	        },
	        setMonitorName: function (id, name) {
	            if (!name) {
	                alert("Не указано имя монитора!");
	                return;
	            }

	            models.setMonitorName(id, name, function () { view.refreshMonitorsGrid(); });
	        },
	        deleteMonitor: function (id) {
	            models.deleteMonitor(id, function () { view.refreshMonitorsGrid(); });
	        },

	        addController: function (name, type, isVisible) {
	            if (!name) {
	                alert("Не указано имя контроллера!");
	                return;
	            }
	            if (!type) {
	                alert("Не указан тип контроллера!");
	                return;
	            }

	            models.addController(name, type, isVisible, function () { view.refreshControllersGrid(); });
	        },
	        setControllerName: function (id, name) {
	            if (!name) {
	                alert("Не указано имя контроллера!");
	                return;
	            }

	            models.setControllerName(id, name, function () { view.refreshControllersGrid(); });
	        },
	        editController: function (id) {
	            application.navigate('webapp/management/controller-editor', id);
	        },
	        deleteController: function (id) {
	            models.deleteController(id, function () { view.refreshControllersGrid(); });
	        },

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
	            application.navigate('webapp/management/zone-editor', id);
	        },
	        deleteZone: function (id) {
	            models.deleteZone(id, function () { view.refreshZonesGrid(); });
	        },

	        reload: function () {
	            view = new views.LayoutView({ viewModel: models.ViewModel });
	            view.on('monitor:add', module.addMonitor);
	            view.on('monitor:setName', module.setMonitorName);
	            view.on('monitor:delete', module.deleteMonitor);
	            view.on('controller:add', module.addController);
	            view.on('controller:setName', module.setControllerName);
	            view.on('controller:edit', module.editController);
	            view.on('controller:delete', module.deleteController);
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