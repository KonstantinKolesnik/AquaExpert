define(
	['app', 'marionette', 'backbone', 'underscore'/*, 'my-plugin/views', 'text!myplugin/mytemplate.tpl'*/],
	function (application, marionette, backbone, _/*, views, tmpl) */) {
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

	        //$.post('/api/scripts/run', {
	        //    scriptId: "8819B702-55BB-44CD-85C6-629D949ACAF6"
	        //});
	        $.getJSON('/api/mysensors/nodes')
                .done(function (data) {
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
	                //Id: "31249bdb-1649-45b3-ac9c-89db4b88ef63"
	                //Name: null
	                //NodeNo: 1
	                //ProtocolVersion: "1.4.1"
	                //SketchName: "Power switch 8"
	                //SketchVersion: "1.0"
	                //Type: 17

                    var nodes = new backbone.Collection(data);
                    nodes.comparator = 'Name';
                    nodes.sort();
	                //nodes.each(function (obj) { console.log(obj.get('Name')); });

                    var view = new nodesView({ collection: nodes });
	                application.setContentView(view);
	            });
	        }
	    };

	    return module;
	});