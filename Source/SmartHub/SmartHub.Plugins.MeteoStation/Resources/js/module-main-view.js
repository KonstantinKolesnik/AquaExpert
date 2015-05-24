
define(
	['common', 'lib', 'text!webapp/meteostation/module-main.html'],
    function (common, lib, templates) {
        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            //triggers: {
            //    'click .js-test': 'node:test'
            //},
            onShow: function () {
                var me = this;



            },
            //onDestroy: function () {

            //},

            bindModel: function (viewModel) {
                lib.kendo.bind($("#content"), viewModel);
            }
        });

        return {
            LayoutView: layoutView
        };
    });