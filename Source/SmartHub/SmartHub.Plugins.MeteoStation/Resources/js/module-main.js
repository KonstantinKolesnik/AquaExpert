define(
	['app', 'webapp/meteostation/module-main-model', 'webapp/meteostation/module-main-view'],
	function (application, models, views) {
	    var module = {





	        reload: function () {
	            //if (application.SignalRReceivers.indexOf(models.ViewModel) == -1)
	            //    application.SignalRReceivers.push(models.ViewModel);

	            models.ViewModel.update(function () {
	                var view = new views.LayoutView();
	                //view.on('node:setName', module.setNodeName);
	                //view.on('sensor:setName', module.setSensorName);
	                //view.on('node:delete', module.deleteNode);
	                //view.on('sensor:delete', module.deleteSensor);
	                //view.on('unitSystem:set', module.setUnitSystem);
	                //view.on('node:test', function (view) {
	                //    //debugger;

	                //    //Внутри функции обработчика события можно обращаться к экземпляру представления при помощи переменной this. this==view.view
	                //    //С ее помощью можно получить доступ к модели (при помощи поля model) и элементам интерфейса (при помощи метода $).
	                //    //Также можно обратиться к корневому элементу интерфейса через поле $el.

	                //    alert("Test");
	                //});

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