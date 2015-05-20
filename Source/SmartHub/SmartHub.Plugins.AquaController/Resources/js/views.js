
define(
	['app', 'common', 'marionette', 'backbone', 'underscore', 'kendo', 'text!webapp/aquacontroller/templates.html'],
    function (application, common, marionette, backbone, _, kendo, templates) {
        var layoutView = marionette.LayoutView.extend({
            template: _.template(templates),
            bind: function (viewModel) {
                kendo.bind($("#content"), viewModel);
            },


        });

        return {
            layoutView: layoutView
        };
    });