﻿
define(['app'], function (application) {
	application.module('WebUI.TilesEditMode', function (module, app, backbone, marionette, $, _) {
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
						defer.resolve(collection);
					})
					.fail(function () {

						defer.resolve(undefined);
					});

				return defer.promise();
			},
			del: function (id) {
				return $.post('/api/webui/tiles/delete', { id: id }).promise();
			},
			sort: function (collection) {
				var ids = [];
				collection.each(function(el) {
					var id = el.get('id');
					ids.push(id);
				});

				var json = JSON.stringify(ids);
				return $.post('/api/webui/tiles/sort', { data: json }).promise();
			}
		};
		
		// requests
		app.reqres.setHandler('query:tiles:edit-mode-list', api.load);
		app.reqres.setHandler('cmd:tiles:edit-mode-delete', api.del);
		app.reqres.setHandler('cmd:tiles:edit-mode-sort', api.sort);
	});

	return application.WebUI.TilesEditMode;
});