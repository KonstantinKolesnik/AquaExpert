define(
	['app', 'webapp/informers/settings-model', 'webapp/informers/settings-view'],
	function (application, models, views) {
	    var view;

	    var module = {
	        addInformer: function (name, sensorDisplayId) {
	            if (!name) {
	                alert("Не указано имя информера!");
	                return;
	            }
	            if (!sensorDisplayId) {
	                alert("Не указан сенсор дисплея!");
	                return;
	            }

	            models.addInformer(name, sensorDisplayId, function () { view.refreshInformersGrid(); });
	        },
	        setInformerName: function (id, name) {
	            if (!name) {
	                alert("Не указано имя информера!");
	                return;
	            }

	            models.setInformerName(id, name, function () { view.refreshInformersGrid(); });
	        },
	        editInformer: function (id) {
	            application.navigate('webapp/informers/informer-editor', id);
	        },
	        deleteInformer: function (id) {
	            models.deleteInformer(id, function () { view.refreshInformersGrid(); });
	        },

	        reload: function () {
	            view = new views.LayoutView({ viewModel: models.ViewModel });
	            view.on('informer:add', module.addInformer);
	            view.on('informer:setName', module.setInformerName);
	            view.on('informer:edit', module.editInformer);
	            view.on('informer:delete', module.deleteInformer);
	            application.setContentView(view);
	        }
	    };

	    return {
	        start: function () {
	            module.reload();
	        }
	    };
	});