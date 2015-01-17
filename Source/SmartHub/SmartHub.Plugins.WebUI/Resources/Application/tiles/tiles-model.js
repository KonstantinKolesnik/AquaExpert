
define(['app'], function (application) {
	application.module('WebUI.Tiles', function (module, app, backbone, marionette, $, _) {
		module.Tile = backbone.Model.extend({
			defaults: {
				id: null,
				title: null,
				wide: false,
				content: []
			}
		});
		module.TileCollection = backbone.Collection.extend({
			model: module.Tile
		});

		var api = {
			load: function () {
				var defer = $.Deferred();

				$.getJSON('/api/webui/tiles')
					.done(function (tiles) {
					    var collection = new module.TileCollection(tiles);

					    $.each(collection.models, function (idx, item) {
					        $.extend(item.attributes, {
					            SignalRReceiveHandler: function (data) {
					                var me = item;
					                if (data.MsgId == "Test") {
					                    me.set({ "content": new Date(data.Value) });
					                    //me.set({ "content": me.get("id") });
					                }
					            }
					        });
					        app.SignalRReceiveHandlers.push(item.attributes.SignalRReceiveHandler);
					    })

						defer.resolve(collection);
					})
					.fail(function () {
						defer.resolve(undefined);
					});

				return defer.promise();
			},
			action: function (id) {
				return $.post('/api/webui/tiles/action', { id: id }).promise();
			}
		};
		
		// requests
		app.reqres.setHandler('query:tiles:all', api.load);
		app.reqres.setHandler('cmd:tiles:action', api.action);
	});

	return application.WebUI.Tiles;
});