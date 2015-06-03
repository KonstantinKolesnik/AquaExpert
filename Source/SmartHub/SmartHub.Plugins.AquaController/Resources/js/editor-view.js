
define(
	['common', 'lib', 'text!webapp/aquacontroller/editor.html'],
    function (common, lib, templates) {
        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            onShow: function () {
                var me = this;

                var type = this.options.viewModel.Controller.Type;

                var tmpl = $("#tmpl" + type);
                if (!tmpl.length)
                    tmpl = $("#tmplError");

                var fn = null;

                switch (type) {
                    case 0: fn = initHeaterController; break;



                }

                $("#container").html(tmpl.html());
                if (fn)
                    fn();

                kendo.bind($("#content"), this.options.viewModel);

                function initHeaterController() {
                    //Switch = 3,             // Switch Actuator (on/off)
                    //Temperature = 6,        // Temperature sensor

                    createSensorSelector($("#ddlHeaterSensorTemperature"), 6);
                    createSensorSelector($("#ddlHeaterSensorSwitch"), 3);
                    createNumericTextBox($("#ntbHeaterTemperatureCalibration"), -10, 10, "n1", 1);
                    createNumericTextBox($("#ntbHeaterTemperatureMin"), 18, 32, "n1", 1);
                    createNumericTextBox($("#ntbHeaterTemperatureMax"), 18, 32, "n1", 1);
                    createNumericTextBox($("#ntbHeaterTemperatureAlarmMin"), 18, 32, "n1", 1);
                    createNumericTextBox($("#ntbHeaterTemperatureAlarmMax"), 18, 32, "n1", 1);
                    createTextBox($("#tbHeaterTemperatureAlarmMaxText"));
                    createTextBox($("#tbHeaterTemperatureAlarmMinText"));
                }

                function createSensorSelector(selector, type) {
                    selector.kendoDropDownList({
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: {
                                    url: function () { return document.location.origin + "/api/mysensors/sensorsByType" },
                                    //dataType: "jsonp"
                                    data: { type: type }
                                }
                            }
                        }),
                        dataValueField: "Id",
                        dataTextField: "Name"
                    });
                }
                function createTextBox(selector) {
                    selector.unbind("keydown").keydown(preventEnter);
                }
                function createNumericTextBox(selector, min, max, format, decimals) {
                    selector.kendoNumericTextBox({
                        min: min,
                        max: max,
                        step: 1,
                        format: format,
                        decimals: decimals
                    });
                }
                function preventEnter(e) {
                    if (e.keyCode == 13) {
                        e.preventDefault();
                        e.stopPropagation();
                        $(e.target).blur(); //run saving
                    }
                }
            }
        });

        return {
            LayoutView: layoutView
        };
    });