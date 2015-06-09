
define(
	['common', 'lib', 'webapp/monitors/utils', 'text!webapp/monitors/monitor-editor.html', '/webapp/monitors/jquery.jsoneditor.min.js'],
    function (common, lib, utils, templates, editor) {
        var viewModel = null;

        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            events: {
                'click .js-btn-default': 'btnDefaultClick',
                'change .js-editor': 'editorChange'
            },
            btnDefaultClick: function () {
                viewModel.Monitor.set("Configuration", JSON.stringify(utils.getDefaultConfiguration()));
                this.refreshChart();
                this.refreshJsonEditor();
            },
            editorChange: function () {
                this.refreshChart();
                this.refreshJsonEditor();
            },

            refreshChart: function () {
                utils.createMonitorChart($("#monitor"), viewModel.Monitor.Configuration);
                kendo.bind($("#content"), viewModel.Monitor);
            },
            refreshJsonEditor: function () {
                var me = this;

                $('#editor').jsonEditor(JSON.parse(viewModel.Monitor.Configuration), {
                    change: function (data) {
                        viewModel.Monitor.set("Configuration", JSON.stringify(data));
                        me.refreshChart();
                    },
                    propertyclick: function (path) { /* called when a property is clicked with the JS path to that property */ }
                    /*propertyElement: '<textarea>', */ // element of the property field, <input> is default
                    /*valueElement: '<textarea>' */  // element of the value field, <input> is default
                });
            },

            onShow: function () {
                viewModel = this.options.viewModel;

                this.refreshJsonEditor();
                this.refreshChart();
            }
        });

        return {
            LayoutView: layoutView
        };
    });