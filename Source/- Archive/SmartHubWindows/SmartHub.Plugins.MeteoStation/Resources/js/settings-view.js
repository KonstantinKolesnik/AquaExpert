
define(
	['common', 'lib', 'text!webapp/meteostation/settings.html'],
    function (common, lib, templates) {
        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            onShow: function () {
                var me = this;

                createSensorSelector($("#ddlSensorTemperatureInner"), 6);
                createSensorSelector($("#ddlSensorHumidityInner"), 7);
                createSensorSelector($("#ddlSensorTemperatureOuter"), 6);
                createSensorSelector($("#ddlSensorHumidityOuter"), 7);
                createSensorSelector($("#ddlSensorAtmospherePressure"), 8);
                createSensorSelector($("#ddlSensorForecast"), 8);
                createNumericTextBox($("#ntbHeight"), -500, 9000, "n0", 0);

                kendo.bind($("#content"), this.options.viewModel);

                function createSensorSelector(selector, type) {
                    selector.kendoDropDownList({
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: {
                                    url: function () { return document.location.origin + "/api/mysensors/sensorsByType" },
                                    data: { type: type }
                                }
                            }
                        }),
                        dataValueField: "Id",
                        dataTextField: "Name",
                        //change: function (e) { me.trigger('Configuration:set'); }
                    });
                }
                function createNumericTextBox(selector, min, max, format, decimals) {
                    selector.kendoNumericTextBox({
                        min: min,
                        max: max,
                        step: 1,
                        format: format,
                        decimals: decimals
                        //change: function (e) { me.trigger('Configuration:set'); }
                    });
                }
            }
        });

        return {
            LayoutView: layoutView
        };
    });