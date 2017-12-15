
define(
	['common', 'lib', 'text!webapp/zones/zone-editor.html'],
    function (common, lib, templates) {

        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            onShow: function () {
                createMultiSelector($("#msMonitors"), "monitors");
                createMultiSelector($("#msControllers"), "controllers");
                createMultiSelector($("#msScripts"), "scripts");
                //createMultiSelector($("#msGraphs"), "graphs");

                kendo.bind($("#content"), this.options.viewModel);

                function createMultiSelector(selector, entity) {
                    return selector.kendoMultiSelect({
                        dataSource: {
                            transport: {
                                read: {
                                    url: function () { return document.location.origin + "/api/" + entity + "/list"; }
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