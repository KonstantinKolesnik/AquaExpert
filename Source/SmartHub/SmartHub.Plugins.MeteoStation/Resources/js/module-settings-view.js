﻿
define(
	['common', 'lib', 'text!webapp/meteostation/module-settings.html'],
    function (common, lib, templates) {
        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            onShow: function () {
                var me = this;

                createSensorSelector($("#ddlSensorTemperatureInner"));
                createSensorSelector($("#ddlSensorHumidityInner"));
                createSensorSelector($("#ddlSensorTemperatureOuter"));
                createSensorSelector($("#ddlSensorHumidityOuter"));
                createSensorSelector($("#ddlSensorAtmospherePressure"));

                function createSensorSelector(selector) {
                    selector.kendoDropDownList({
                        dataValueField: "Id",
                        dataTextField: "Name",
                        change: function (e) { me.trigger('SensorsConfiguration:set'); }
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