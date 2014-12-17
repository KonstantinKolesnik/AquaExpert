define(
	['app', 'application/sections/list'],
	function (application, sections) {
		application.module('WebUI.UserSections', function (module, app, backbone, marionette, $, _) {
			module.start = function () {
				sections.api.reload('query:sections:common', 'Приложения');
			};
		});

		return application.WebUI.UserSections;
	});