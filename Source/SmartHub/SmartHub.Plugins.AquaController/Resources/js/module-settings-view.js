
define(
	['common', 'lib', 'text!webapp/aquacontroller/module-settings.html'],
    function (common, lib, templates) {
        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            onShow: function () {
                var me = this;

                createTabStrip($("#tabstrip"));
                createSensorSelector($("#ddlHeaterSensorTemperature"));
                createSensorSelector($("#ddlHeaterSensorSwitch"));
                createNumericTextBox($("#ntbHeaterTemperatureCalibration"), -10, 10, "n1", 1);
                createNumericTextBox($("#ntbTemperatureMin"), 10, 32, "n1", 1);
                createNumericTextBox($("#ntbTemperatureMax"), 10, 32, "n1", 1);
                createNumericTextBox($("#ntbTemperatureAlarmMin"), 10, 32, "n1", 1);
                createNumericTextBox($("#ntbTemperatureAlarmMax"), 10, 32, "n1", 1);
                createTextBox($("#tbHeaterTemperatureAlarmMaxText"));
                createTextBox($("#tbHeaterTemperatureAlarmMinText"));

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
                        change: function (e) { me.trigger("heaterControllerConfiguration:set"); }
                    });
                }
                function createTextBox(selector) {
                    selector.unbind("keydown").keydown(preventEnter).blur(function () { me.trigger("heaterControllerConfiguration:set"); });
                }
                function createNumericTextBox(selector, min, max, format, decimals) {
                    selector.kendoNumericTextBox({
                        min: min,
                        max: max,
                        step: 1,
                        format: format,
                        decimals: decimals,
                        change: function (e) { me.trigger("heaterControllerConfiguration:set"); }
                    });
                }
                function preventEnter(e) {
                    if (e.keyCode == 13) {
                        e.preventDefault();
                        e.stopPropagation();
                        $(e.target).blur(); //run saving
                    }
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