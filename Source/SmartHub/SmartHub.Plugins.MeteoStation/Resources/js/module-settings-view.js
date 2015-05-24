
define(
	['common', 'lib', 'text!webapp/meteostation/module-settings.html'],
    function (common, lib, templates) {
        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            //triggers: {
            //    'click .js-test': 'node:test'
            //},
            //initialize: function () {
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
            //onDestroy: function () {

            //},

            bindModel: function (viewModel) {
                lib.kendo.bind($("#content"), viewModel);
            }
        });

        return {
            LayoutView: layoutView
        };
    });