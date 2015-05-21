
define(
	['app', 'webapp/mysensors/module-model', 'webapp/mysensors/module-view'],
	function (application, models, views) {
	    var module = {
	        setNodeName: function (id, name) {
	            models.setNodeName(id, name);
	        },
	        setSensorName: function (id, name) {
	            models.setSensorName(id, name);
	        },
	        deleteNode: function (id) {
	            models.deleteNode(id);
	        },
	        deleteSensor: function (id) {
	            models.deleteSensor(id);
	        },
	        setUnitSystem: function (us) {
	            models.setUnitSystem(us);
	        },

	        reload: function () {
	            application.SignalRReceivers.push(models.ViewModel);

	            models.ViewModel.update(function (items) {
					//var view = new views.ScriptListView({ collection: items });
	                var view = new views.LayoutView();
	                view.on('node:setName', module.setNodeName);
	                view.on('sensor:setName', module.setSensorName);
	                view.on('node:delete', module.deleteNode);
	                view.on('sensor:delete', module.deleteSensor);
	                view.on('unitSystem:set', module.setUnitSystem);
	                view.on('node:test', function (view) {
	                    //debugger;

	                    //Внутри функции обработчика события можно обращаться к экземпляру представления при помощи переменной this. this==view.view
	                    //С ее помощью можно получить доступ к модели (при помощи поля model) и элементам интерфейса (при помощи метода $).
	                    //Также можно обратиться к корневому элементу интерфейса через поле $el.

	                    alert("Test");
	                });

	                application.setContentView(view);

	                view.bind(models.ViewModel);
	            });
	        }
	    };

	    return {
	        start: function () {
	            module.reload();
	        }
	    };
	});