define(
	['app', 'webapp/controllers/settings-model', 'webapp/controllers/settings-view'],
	function (application, models, views) {
	    var view;

	    var module = {
	        addController: function (name, type) {
	            if (!name) {
	                alert("Не указано имя контроллера!");
	                return;
	            }
	            if (!type) {
	                alert("Не указан тип контроллера!");
	                return;
	            }

	            models.addController(name, type, function () { view.refreshControllersGrid(); });
	        },
	        setControllerName: function (id, name) {
	            if (!name) {
	                alert("Не указано имя контроллера!");
	                return;
	            }

	            models.setControllerName(id, name, function () { view.refreshControllersGrid(); });
	        },
	        editController: function (id) {
	            application.navigate('webapp/controllers/controller-editor', id);
	        },
	        deleteController: function (id) {
	            models.deleteController(id, function () { view.refreshControllersGrid(); });
	        },

	        reload: function () {
	            view = new views.LayoutView({ viewModel: models.ViewModel });
	            view.on('controller:add', module.addController);
	            view.on('controller:setName', module.setControllerName);
	            view.on('controller:edit', module.editController);
	            view.on('controller:delete', module.deleteController);
	            application.setContentView(view);
	        }
	    };

	    return {
	        start: function () {
	            module.reload();
	        }
	    };
	});