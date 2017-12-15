
define(
	['app', 'common', 'webapp/mysensors/settings-model', 'webapp/mysensors/settings-view'],
	function (application, common, models, views) {
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
	        deleteValues: function (dateTo) {
	            if (!dateTo) {
	                alert("Не указана дата по!");
	                return;
	            }

	            if (common.utils.confirm('Удалить значения?'))
                    models.deleteValues(dateTo);
	        },

	        reload: function () {
	            if (application.SignalRReceivers.indexOf(models.ViewModel) == -1)
                    application.SignalRReceivers.push(models.ViewModel);

	            models.ViewModel.update(function () {
	                var view = new views.LayoutView({ viewModel: models.ViewModel });
	                view.on('node:setName', module.setNodeName);
	                view.on('sensor:setName', module.setSensorName);
	                view.on('node:delete', module.deleteNode);
	                view.on('sensor:delete', module.deleteSensor);
	                view.on('unitSystem:set', module.setUnitSystem);
	                view.on('values:delete', module.deleteValues);
	                view.on('node:test', function (view) {
	                    //debugger;

	                    //Внутри функции обработчика события можно обращаться к экземпляру представления при помощи переменной this. this==view.view
	                    //С ее помощью можно получить доступ к модели (при помощи поля model) и элементам интерфейса (при помощи метода $).
	                    //Также можно обратиться к корневому элементу интерфейса через поле $el.

	                    alert("Test");
	                });
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