define(['lib', 'app'], function (lib, app) {
    var tileModel = lib.backbone.Model.extend({
        defaults: {
            id: null,
            title: null,
            wide: false,
            content: []
        }
    });

    var tileCollection = lib.backbone.Collection.extend({
        model: tileModel
    });

    var api = {
        load: function () {
            var defer = lib.$.Deferred();

            lib.$.getJSON('/api/webui/tiles')
				.done(function (tiles) {
				    var collection = new tileCollection(tiles);

				    $.each(collection.models, function (idx, tile) { // tile is TileWebModel
				        if (tile.attributes.SignalRReceiveHandler) {
				            tile.attributes.SignalRReceiveHandler = new Function("model, data", tile.attributes.SignalRReceiveHandler);
				            if (tile.attributes.SignalRReceiveHandler) {
				                tile.attributes.tileModel = tile;
				                //app.SignalRReceivers.push(tile.attributes);
				            }
				        }
				    })

				    defer.resolve(collection);
				})
				.fail(function () {
				    defer.resolve(undefined);
				});

            return defer.promise();
        },
        action: function (id) {
            return lib.$.post('/api/webui/tiles/action', { id: id }).promise();
        }
    };

    return {
        // entities
        Tile: tileModel,
        TileCollection: tileCollection,

        // requests
        load: api.load,
        action: api.action
    };
});