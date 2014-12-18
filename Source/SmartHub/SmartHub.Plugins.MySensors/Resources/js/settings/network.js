define(
	['app', 'marionette', 'backbone', 'underscore'],
	function (application, marionette, backbone, _) {
	    var module = {
	        start: function () {
	            alert("module started!");
	        }
	    };

	    return module;
	});