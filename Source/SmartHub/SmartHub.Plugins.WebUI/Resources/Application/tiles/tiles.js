
define(
	['app', 'common', 'application/tiles/tiles-model', 'application/tiles/tiles-view'],
	function (application, commonModule) {
		application.module('WebUI.Tiles', function (module, app, backbone, marionette, $, _) {
			var api = {
				getTiles: function () {
				    app.request('query:tiles:all')
                        .done(function (collection) {
                            var view = new module.TileCollectionView({ collection: collection });
						    view.on('childview:webui:tile:click', api.onTileClick);
						    app.setContentView(view);
					    });
				},
				onTileClick: function (childView) {
					var url = childView.model.get('url');

					if (url) {
						var args = childView.model.get('parameters');
						app.loadPath(url, args);
					}
					else {
					    var id = childView.model.get('id');
					    app.request('cmd:tiles:action', id)
                            .done(function (result) {
                                if (result)
                                    alert(result);
                            });
					}
				}
			};

			module.start = function () {
			    api.getTiles();
			};
		});

		return application.WebUI.Tiles;
	});