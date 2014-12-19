define(
	['app', 'marionette', 'backbone', 'underscore'],
	function (application, marionette, backbone, _) {
	    //// описываем параметры представлений для коллекции и для ее отдельного элемента
	    //var personView = marionette.ItemView.extend({
	    //    template: _.template('<%= name %> (<%= id %>)')			// шаблон отображения
	    //});

	    //var peoplesView = marionette.CompositeView.extend({
	    //    template: _.template('<h1>Узлы</h1><div id="lstNodes"></div>'),	// шаблон отображения
	    //    childView: personView,			    // представление для элемента коллекции
	    //    childViewContainer: '#lstNodes'		// контейнер, куда будут добавляться элементы
	    //});



	    var nodeView = marionette.ItemView.extend({
	        template: _.template('[# <%= NodeNo %>] [Type: <%= Type %>] [Sketch name: <%= SketchName %>] [Sketch version: <%= SketchVersion %>] [Protocol version: <%= ProtocolVersion %>]')
	    });

	    var nodesView = marionette.CompositeView.extend({
	        template: _.template('<h1>Узлы</h1><div id="lstNodes"></div>'),
	        childView: nodeView,
	        childViewContainer: '#lstNodes'
	    });




	    function loadData() {
	        var obj = $.Deferred();

	        //$.post('/api/scripts/run', { scriptId: "8819B702-55BB-44CD-85C6-629D949ACAF6" })
	        $.getJSON('/api/mysensors/nodes')
                .done(function (data) {
                    //Id: "31249bdb-1649-45b3-ac9c-89db4b88ef63"
                    //Name: null
                    //NodeNo: 1
                    //ProtocolVersion: "1.4.1"
                    //SketchName: "Power switch 8"
                    //SketchVersion: "1.0"
                    //Type: 17

                    // после получения данных с сервера создаем для них модель 
                    // меняем состояние операции на "завершена успешно"
                    //var model = new backbone.Model(data);
                    var model = new backbone.Collection(data);
                    obj.resolve(model);
                });
	        return obj;
	    }


	    var module = {
	        start: function () {
                // 1)
	            //var items = [
		        //    { id: 1, name: 'Lev Tolstoy' },
		        //    { id: 2, name: 'Ivan Turgenev' },
		        //    { id: 3, name: 'Nikolay Gogol' },
		        //    { id: 4, name: 'Alexander Pushkin' },
                //    { id: 5, name: 'Константин Колесник' }
	            //];

	            //var model = new backbone.Collection(items);
	            //model.comparator = 'name';
	            //model.sort();
	            ////model.each(function (obj) { console.log(obj.get('name')); });

	            //// создаем экземпляр представления и передаем туда данные (модель Backbone.Collection)
	            //var view = new peoplesView({ collection: model });

	            //// отображаем представление на странице
	            //application.setContentView(view);


                // 2)
	            //var query = loadData();
	            //query.done(function (data) {
	            //    // при успешном завершении ajax запроса отображаем полученные данные на странице 
	            //    //var view = new myView({ model: data });
	            //    var view = new nodesView({ collection: data });
	            //    application.setContentView(view);
	            //});
	            //query.fail(function () {
	            //    // при ошибке выводим сообщение "error"
	            //    alert("error");
	            //});

	            // 3)
	            $.getJSON('/api/mysensors/nodes').done(function (data) {
                    var res = new backbone.Collection(data);
	                var view = new nodesView({ collection: res });
	                application.setContentView(view);
	            });


	        }
	    };

	    return module;
	});