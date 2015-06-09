define(
	['app', 'webapp/monitors/settings-model', 'webapp/monitors/settings-view', 'webapp/monitors/utils'],
	function (application, models, views, utils) {
	    var view;

	    var module = {
	        addMonitor: function (name, sensorId) {
	            if (!name) {
	                alert("Не указано имя монитора!");
	                return;
	            }
	            if (!sensorId) {
	                alert("Не указан сенсор монитора!");
	                return;
	            }

	            models.addMonitor(name, sensorId, utils.getDefaultConfiguration(), function () { view.refreshMonitorsGrid(); });
	        },
	        setMonitorName: function (id, name) {
	            if (!name) {
	                alert("Не указано имя монитора!");
	                return;
	            }

	            models.setMonitorName(id, name, function () { view.refreshMonitorsGrid(); });
	        },
	        editMonitor: function (id) {
	            application.navigate('webapp/monitors/monitor-editor', id);
	        },
	        deleteMonitor: function (id) {
	            models.deleteMonitor(id, function () { view.refreshMonitorsGrid(); });
	        },

	        reload: function () {
	            view = new views.LayoutView({ viewModel: models.ViewModel });
	            view.on('monitor:add', module.addMonitor);
	            view.on('monitor:setName', module.setMonitorName);
	            view.on('monitor:edit', module.editMonitor);
	            view.on('monitor:delete', module.deleteMonitor);
	            application.setContentView(view);
	        }
	    };

	    return {
	        start: function () {
	            module.reload();
	        }
	    };
	});