﻿define(
	['app', 'application/sections/list-model', 'application/sections/list-view'],
	function (application) {
		application.module('WebUI.Sections', function (module, app, backbone, marionette, $, _) {
			var api = {
				addTile: function (childView) {
				    var typeFullName = childView.model.get('tileTypeFullName');
				    var parameters = { };

					if (!typeFullName) {
					    typeFullName = 'SmartHub.Plugins.WebUI.Tiles.DefaultTile';
					    parameters.title = childView.model.get('name');
						parameters.url = childView.model.get('path');
					}

					app.addTile(typeFullName, parameters);
				},
				reload: function (requestName, pageTitle) {
					app.request(requestName).done(function (items) {
						var view = new module.SectionListView({ collection: items, title: pageTitle });
						view.on('childview:sections:add-tile', api.addTile);
						app.setContentView(view);
					});
				}
			};

			module.api = api;
		});

		return application.WebUI.Sections;
	});