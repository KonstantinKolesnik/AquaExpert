define(
	[
		'marionette',
		'backbone',
		'underscore',
		'jquery',
		'json2',
		'chart',
		'chart.scatter',
        'kendo',
        'signalR'
	],
	function (
        marionette,
        backbone,
        underscore,
        jquery,
        json2,
        chartjs,
        chartscatter,
        kendo,
        signalR) {

	    return {
			marionette: marionette,
			backbone: backbone,
			_: underscore,
			$: jquery,
			json2: json2,
			Chart: chartjs,
			kendo: kendo,
			signalR: signalR
		};
	});