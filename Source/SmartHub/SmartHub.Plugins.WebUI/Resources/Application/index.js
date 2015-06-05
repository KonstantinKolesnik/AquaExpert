﻿requirejs.config({
	baseUrl: '/',
	paths: {
		app: 'application/app',
		common: 'application/common',
		lib: 'application/lib',

		text: 'vendor/js/require-text',
		json: 'vendor/js/require-json',

		json2: 'vendor/js/json2.min',
		jquery: 'vendor/js/jquery.min',
		underscore: 'vendor/js/underscore.min',
		backbone: 'vendor/js/backbone.min',
		marionette: 'vendor/js/backbone.marionette.min',
		syphon: 'vendor/js/backbone.syphon',
		bootstrap: 'vendor/js/bootstrap.min',
		moment: 'vendor/js/moment.min',
		codemirror: 'vendor/js/codemirror-all',
		chart: 'vendor/js/chart.min',
		'chart.scatter': 'vendor/js/chart.scatter.min',

		tiles:				'application/tiles/tiles',
		'tiles-edit-mode':	'application/tiles/tiles-edit-mode',
		apps:				'application/sections/user',
		settings:			'application/sections/system',

		kendo: 'vendor/js/kendo.all.min',
		cultureRU: 'vendor/js/cultures/kendo.culture.ru-RU.min',
		messagesRU: 'vendor/js/messages/kendo.messages.ru-RU.min',

		signalR: 'vendor/js/jquery.signalR-2.1.2.min'
	},
	shim: {
	    bootstrap: ['jquery'],
	    backbone: {
	        deps: ['json2', 'jquery', 'underscore'],
	        exports: 'Backbone'
	    },
	    syphon: {
	        deps: ['backbone'],
	        exports: 'Backbone.Syphon'
	    },
	    marionette: {
	        deps: ['backbone', 'syphon'],
	        exports: 'Marionette'
	    },
		kendo: {
		    deps: ['jquery'],
		    exports: 'Kendo'
		},
		signalR: {
		    deps: ['jquery'],
		    exports: 'signalR'
		}
	},
	map: {
	    'chart.scatter': {
	        'Chart': 'chart'
	    }
	}
});

require(['app', 'common', 'bootstrap'], function (app) {
    app.start();
});