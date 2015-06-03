define(
	['app', 'webapp/aquacontroller/settings-model', 'webapp/aquacontroller/settings-view'],
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
	        setMonitorIsVisible: function (id, isVisible) {
	            models.setMonitorIsVisible(id, isVisible, function () { view.refreshMonitorsGrid(); });
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
	        setControllerIsVisible: function (id, isVisible) {
	            models.setControllerIsVisible(id, isVisible, function () { view.refreshControllersGrid(); });
	        },
	        editController: function (id) {
	            application.navigate('webapp/aquacontroller/editor', id);
	        },
	        deleteController: function (id) {
	            models.deleteController(id, function () { view.refreshControllersGrid(); });
	        },

	        reload: function () {
	            view = new views.LayoutView();
	            view.on('monitor:add', module.addMonitor);
	            view.on('monitor:setName', module.setMonitorName);
	            view.on('monitor:setIsVisible', module.setMonitorIsVisible);
	            view.on('monitor:delete', module.deleteMonitor);
	            view.on('controller:add', module.addController);
	            view.on('controller:setName', module.setControllerName);
	            view.on('controller:setIsVisible', module.setControllerIsVisible);
	            view.on('controller:edit', module.editController);
	            view.on('controller:delete', module.deleteController);
	            application.setContentView(view);

	            view.bindModel(models.ViewModel);
	        }
	    };

	    return {
	        start: function () {
	            module.reload();
	        }
	    };
	});