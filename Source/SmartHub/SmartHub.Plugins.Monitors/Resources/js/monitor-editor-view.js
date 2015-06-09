
define(
	['common', 'lib', 'webapp/monitors/utils', 'text!webapp/monitors/monitor-editor.html'],
    function (common, lib, utils, templates) {
        var viewModel = null;

        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            createChart: function () {
                utils.createMonitorChart($("#monitor"), viewModel.Monitor.Configuration);
            },

            onShow: function () {
                var me = this;
                
                viewModel = me.options.viewModel;
                me.createChart();

                kendo.bind($("#content"), viewModel.Monitor);
            }
        });

        return {
            LayoutView: layoutView
        };
    });