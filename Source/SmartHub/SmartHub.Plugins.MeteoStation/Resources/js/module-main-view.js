
define(
	['common', 'lib', 'text!webapp/meteostation/module-main.html'],
    function (common, lib, templates) {
        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            onShow: function () {
                var me = this;

                createSensorValuesChart($("#sensorTemperatureInnerChart"), [
                    { field: "NodeNo", operator: "eq", value: 3 },
                    { field: "SensorNo", operator: "eq", value: 0 }
                ]);
                createSensorValuesChart($("#sensorHumidityInnerChart"), [
                    { field: "NodeNo", operator: "eq", value: 3 },
                    { field: "SensorNo", operator: "eq", value: 1 }
                ]);
                createSensorValuesChart($("#sensorTemperatureOuterChart"), [
                    { field: "NodeNo", operator: "eq", value: 3 },
                    { field: "SensorNo", operator: "eq", value: 2 }
                ]);
                createSensorValuesChart($("#sensorHumidityOuterChart"), [
                    { field: "NodeNo", operator: "eq", value: 3 },
                    { field: "SensorNo", operator: "eq", value: 3 }
                ]);
                createSensorValuesChart($("#sensorAtmospherePressureChart"), [
                    { field: "NodeNo", operator: "eq", value: 3 },
                    { field: "SensorNo", operator: "eq", value: 4 }
                ]);

                function createSensorValuesChart(selector, filter) {
                    selector.kendoChart({
                        dataSource: { filter: filter },
                        series: [{
                            line: {
                                style: "smooth"
                            },
                            type: "area",
                            //aggregate: "avg",
                            field: "Value",
                            categoryField: "TimeStamp"
                        }],

                        tooltip: {
                            visible: true,
                            template: "#= value #"
                        },

                        categoryAxis: {
                            type: "date",

                            //baseUnit: "fit",
                            //baseUnit: "seconds",
                            //baseUnit: "minutes",
                            //baseUnit: "hours",
                            baseUnit: "days",
                            //baseUnit: "weeks",
                            //baseUnit: "months",
                            //baseUnit: "years",

                            labels: {
                                dateFormats: {
                                    hours: "MMM d HH:mm",
                                    //hours: "HH:mm",
                                    days: "MMM d",
                                    weeks: "MMM d",
                                    months: "yyyy MMM",
                                    years: "yyyy"
                                },
                                visible: true,
                                rotation: 270,
                            },
                            line: { visible: true },
                            majorGridLines: { visible: true }
                        }
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