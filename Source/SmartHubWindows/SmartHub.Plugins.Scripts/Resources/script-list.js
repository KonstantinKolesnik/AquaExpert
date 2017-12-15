define(
	['app',	'common', 'webapp/scripts/script-list-model', 'webapp/scripts/script-list-view'],
	function (application, common, models, views) {
		var api = {
			runScript: function (view) {
				var scriptId = view.model.get('Id');

				models.runScript(scriptId).done(function () {
					var name = view.model.get('Name');
					common.utils.alert('Скрипт "{0}" выполнен.', name);
				});
			},
			deleteScript: function (view) {
				var scriptName = view.model.get('Name');

				if (common.utils.confirm('Удалить скрипт "{0}"?', scriptName)) {
					var scriptId = view.model.get('Id');
					models.deleteScript(scriptId).done(api.reload);
				}
			},
			addScriptTile: function (view) {
				var scriptId = view.model.get('Id');
				application.addTile('SmartHub.Plugins.Scripts.ScriptsTile', { id: scriptId });
			},
			addScript: function () {
				application.navigate('webapp/scripts/script-editor');
			},
			editScript: function (childView) {
				var scriptId = childView.model.get('Id');
				application.navigate('webapp/scripts/script-editor', scriptId);
			},
			reload: function () {
				models.loadScriptList()
					.done(function (items) {

						var view = new views.ScriptListView({ collection: items });

						view.on('scripts:add', api.addScript);
						view.on('childview:scripts:edit', api.editScript);
						view.on('childview:scripts:run', api.runScript);
						view.on('childview:scripts:delete', api.deleteScript);
						view.on('childview:scripts:add-tile', api.addScriptTile);

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