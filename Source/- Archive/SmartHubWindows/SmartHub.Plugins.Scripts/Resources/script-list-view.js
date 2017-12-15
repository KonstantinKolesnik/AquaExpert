﻿define(
	['lib', 'text!webapp/scripts/script-list.tpl', 'text!webapp/scripts/script-list-item.tpl'],
	function (lib, listTemplate, itemTemplate) {
		var scriptView = lib.marionette.ItemView.extend({
			template: lib._.template(itemTemplate),
			triggers: {
				'click .js-btn-run': 'scripts:run',
				'click .js-btn-edit': 'scripts:edit',
				'click .js-btn-delete': 'scripts:delete',
				'click .js-btn-add-tile': 'scripts:add-tile'
			}
		});

		var scriptListView = lib.marionette.CompositeView.extend({
			template: lib._.template(listTemplate),
			childView: scriptView,
			childViewContainer: '.js-list',
			triggers: {
				'click .js-add-script': 'scripts:add'
			}
		});

		return {
			ScriptView: scriptView,
			ScriptListView: scriptListView
		};
	});