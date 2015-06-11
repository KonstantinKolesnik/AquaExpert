
define(
	['common', 'lib', 'text!webapp/zones/zone-editor.html'],
    function (common, lib, templates) {

        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            onShow: function () {
                createMultiSelector($("#msMonitors"), "monitors", "Id", "Name");
                createMultiSelector($("#msControllers"), "controllers", "Id", "Name");
                createMultiSelector($("#msScripts"), "scripts", "id", "name");
                //createMultiSelector($("#msGraphs"), "graphs", "Id", "Name");

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