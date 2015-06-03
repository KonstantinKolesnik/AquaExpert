
define(
	['common', 'lib', 'text!webapp/aquacontroller/editor.html'],
    function (common, lib, templates) {
        var model;

        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            //events: {
            //    'click .js-btn-save': 'btnSaveClick',
            //    'click .js-btn-cancel': 'btnCancelClick'
            //},
            //btnSaveClick: function (e) {
            //    e.preventDefault();

            //    //this.cm.save();
            //    //var data = lib.backbone.Syphon.serialize(this);
            //    //this.trigger('editor:save', data);
            //},
            //btnCancelClick: function (e) {
            //    e.preventDefault();
            //    this.trigger('editor:cancel');
            //},

            bindModel: function (viewModel) {
                var me = this;

                model = viewModel;
                lib.kendo.bind($("#content"), viewModel);

                switch (viewModel.Type) {
                    case 0: initHeaterController(); break;

                }

                function initHeaterController() {
                    createSensorSelector($("#ddlHeaterSensorTemperature"));
                    createSensorSelector($("#ddlHeaterSensorSwitch"));
                    createNumericTextBox($("#ntbHeaterTemperatureCalibration"), -10, 10, "n1", 1);
                    createNumericTextBox($("#ntbHeaterTemperatureMin"), 18, 32, "n1", 1);
                    createNumericTextBox($("#ntbHeaterTemperatureMax"), 18, 32, "n1", 1);
                    createNumericTextBox($("#ntbHeaterTemperatureAlarmMin"), 18, 32, "n1", 1);
                    createNumericTextBox($("#ntbHeaterTemperatureAlarmMax"), 18, 32, "n1", 1);
                    createTextBox($("#tbHeaterTemperatureAlarmMaxText"));
                    createTextBox($("#tbHeaterTemperatureAlarmMinText"));
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
            }
        });

        return {
            LayoutView: layoutView
        };
    });