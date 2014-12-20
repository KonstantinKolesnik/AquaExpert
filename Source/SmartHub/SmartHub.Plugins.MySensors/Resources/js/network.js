
define(
	['app', 'marionette', 'backbone', 'underscore', 'webapp/mysensors/views'],
	function (application, marionette, backbone, _, views) {
	    var api = {
	        getNodes: function (onComplete, onError) {
	            $.getJSON('/api/mysensors/nodes')
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	            .fail(function () {
	                if (onError)
	                    onError("Error");
	            });
	        },
	        getSensors: function (onComplete, onError) {
	            $.getJSON('/api/mysensors/sensors')
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	            .fail(function () {
	                if (onError)
	                    onError("Error");
	            });
	        }
	    }

	    var module = {
	        start: function () {
	            api.getNodes(function (data) {
                    var nodes = new backbone.Collection(data);
                    nodes.comparator = 'Name';
                    nodes.sort();
	                //nodes.each(function (obj) { console.log(obj.get('Name')); });


	                //var view = new views.nodesView({ collection: nodes });
	                //application.setContentView(view);



	                //// создаем экземпляр layout view и добавляем его на страницу
	                var layoutView = new views.layoutView();
	                application.setContentView(layoutView);

	                //// создаем экземпляры дочерних представлений
                    //var filterView = new myFilterView( ... );
	                var listView = new views.nodesView({ collection: nodes });

	                //// отображаем дочерние представления на странице
                    //layoutView.filter.show(filterView);
                    layoutView.list.show(listView);
	            }, null);
	        }
	    };

	    return module;
	});