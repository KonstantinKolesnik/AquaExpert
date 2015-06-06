
define(
	['common', 'lib', 'text!webapp/management/zone-editor.html'],
    function (common, lib, templates) {

        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            onShow: function () {
                createMultiSelector($("#msMonitors"), "monitor");
                createMultiSelector($("#msControllers"), "controller");
                //createMultiSelector($("#msGraphs"), "graph");

                //$("#sparkline").kendoSparkline([1, 2, 3, 2, 1]);
                //$("#sparkline").kendoSparkline([200, 450, 300, 125]);
                $("#sparkline").kendoSparkline({
                    series: [
                        {
                            name: "World",
                            data: [15.7, 16.7, 20, 23.5, 26.6]
                        }
                    ],
                    categoryAxis: {
                        categories: [2005, 2006, 2007, 2008, 2009]
                    }
                });

                kendo.bind($("#content"), this.options.viewModel);

                function createMultiSelector(selector, entity) {
                    return selector.kendoMultiSelect({
                        dataSource: {
                            transport: {
                                read: {
                                    url: function () { return document.location.origin + "/api/management/" + entity + "/list"; }
                                }
                            },
                        },
                        dataValueField: "Id",
                        dataTextField: "Name",
                        valuePrimitive: true,
                        autoClose: false,
                        filter: "contains",
                        placeholder: "Выберите элементы...",
                    }).data("kendoMultiSelect");
                }
            }
        });

        return {
            LayoutView: layoutView
        };
    });