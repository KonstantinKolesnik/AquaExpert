﻿define(
	['app', 'application/tiles/tiles-model', 'application/tiles/tiles-view'],
	function (application, models, views) {
	    var api = {
	        open: function (childView) { // childView is TileWebModel
	            var id = childView.model.get('id');
	            var url = childView.model.get('url');

	            if (url) {
	                var parameters = childView.model.get('parameters');
	                application.loadPath(url, parameters);
	            } else
	                models.action(id).done(function (response) {
	                    if (response)
	                        alert(response);
	                });
	        },
	        reload: function () {
	            models.load().done(function (collection) {
	                var view = new views.TileCollectionView({ collection: collection });
	                view.on('childview:webui:tile:click', api.open);

	                application.setContentView(view);
	            });
	        }
	    };

	    return {
	        start: function () {
	            api.reload();
	        }
	    };
	});