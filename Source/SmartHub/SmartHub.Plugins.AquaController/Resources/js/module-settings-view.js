
define(
	['common', 'lib', 'text!webapp/aquacontroller/module-settings.html'],
    function (common, lib, templates) {
        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            onShow: function () {
                var me = this;

                createTabStrip($("#tabstrip"));
                createSensorSelector($("#ddlSensorTemperatureWater"));
                createSensorSelector($("#ddlSensorSwitchHeater"));
                createNumericTextBox($("#ntbTemperatureWaterMin"), 20, 28, "n1", 1);
                createNumericTextBox($("#ntbTemperatureWaterMax"), 20, 28, "n1", 1);

                function createTabStrip(selector) {
                    selector.kendoTabStrip({
                        animation: {
                            open: { effects: "fadeIn" }
                        },
                        activate: function () {
                            //if (selector = $("#tabstrip"))
                            //    adjustSizes();
                        }
                    });
                }
                function createSensorSelector(selector) {
                    selector.kendoDropDownList({
                        dataValueField: "Id",
                        dataTextField: "Name",
                        change: function (e) { me.trigger('temperatureControllerConfiguration:set'); }
                    });
                }
                function createNumericTextBox(selector, min, max, format, decimals) {
                    selector.kendoNumericTextBox({
                        min: min,
                        max: max,
                        step: 1,
                        format: format,
                        decimals: decimals,
                        change: function (e) { me.trigger('temperatureControllerConfiguration:set'); }
                    });
                }
            },

            bindModel: function (viewModel) {
                lib.kendo.bind($("#content"), viewModel);
            }
        });

        return {
            LayoutView: layoutView
        };
    });