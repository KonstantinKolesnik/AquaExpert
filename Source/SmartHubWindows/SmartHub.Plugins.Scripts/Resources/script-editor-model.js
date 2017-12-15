define(['lib'], function (lib) {

	// entities
	var scriptData = lib.backbone.Model.extend({
		defaults: {
			Id: null,
			Body: null
		}
	});

	// api
	var api = {
		loadScript: function (scriptId) {
			var defer = lib.$.Deferred();

			lib.$.getJSON('/api/scripts/get', { id: scriptId })
				.done(function (script) {
					var model = new scriptData(script);
					defer.resolve(model);
				})
				.fail(function () {
					defer.resolve(undefined);
				});

			return defer.promise();
		},
		saveScript: function (model) {
			var scriptId = model.get('Id');
			var scriptName = model.get('Name');
			var scriptBody = model.get('Body');

			var rq = lib.$.post('/api/scripts/save', {
				id: scriptId,
				name: scriptName,
				body: scriptBody
			});

			return rq.promise();
		}
	};

	return {
		// entities
		ScriptData: scriptData,

		// requests
		loadScript: api.loadScript,
		saveScript: api.saveScript
	}
});