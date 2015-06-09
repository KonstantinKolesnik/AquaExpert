
define(
	['common', 'lib', 'text!webapp/zones/zone-editor.html'],
    function (common, lib, templates) {

        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            onShow: function () {
                createMultiSelector($("#msMonitors"), "monitors", "Id", "Name");
                createMultiSelector($("#msControllers"), "controllers", "Id", "Name");
                createMultiSelector($("#msScripts"), "scripts", "id", "name");
                createMultiSelector($("#msGraphs"), "graphs", "Id", "Name");

                //$("#sparkline").kendoSparkline([1, 2, 3, 2, 1]);
                //$("#sparkline").kendoSparkline([200, 450, 300, 125]);
                //$("#sparkline").kendoSparkline({
                //    series: [
                //        {
                //            name: "World",
                //            data: [15.7, 16.7, 20, 23.5, 26.6]
                //        }
                //    ],
                //    categoryAxis: {
                //        categories: [2005, 2006, 2007, 2008, 2009]
                //    }
                //});

                kendo.bind($("#content"), this.options.viewModel);

                function createMultiSelector(selector, entity, valueFieldName, textFieldName) {
                    return selector.kendoMultiSelect({
                        dataSource: {
                            transport: {
                                read: {
                                    url: function () { return document.location.origin + "/api/" + entity + "/list"; }
                                }
                            },
                        },
                        dataValueField: valueFieldName,
                        dataTextField: textFieldName,
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