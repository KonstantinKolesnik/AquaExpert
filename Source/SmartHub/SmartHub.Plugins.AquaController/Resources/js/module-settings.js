define(
	['app', 'webapp/aquacontroller/module-settings-model', 'webapp/aquacontroller/module-settings-view'],
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

	        setHeaterControllerConfiguration: function () {
	            models.setHeaterControllerConfiguration(models.ViewModel.HeaterControllerConfiguration);
	        },



	        reload: function () {
	            models.ViewModel.update(function () {
	                view = new views.LayoutView();
	                view.on('monitor:add', module.addMonitor);
	                view.on('monitor:setName', module.setMonitorName);
	                view.on('monitor:setIsVisible', module.setMonitorIsVisible);
	                view.on('monitor:delete', module.deleteMonitor);
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