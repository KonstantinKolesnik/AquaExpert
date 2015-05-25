
define(
	['common', 'lib', 'text!webapp/meteostation/module-main.html'],
    function (common, lib, templates) {
        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            //triggers: {
            //    'click .js-test': 'node:test'
            //},
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
                    //e.detailRow.find(".sensorDetailsOptions").bind("change", function () {
                    //    var chart = e.detailRow.find(".sensorDetailsValues").data("kendoChart"),
                    //        series = chart.options.series,
                    //        categoryAxis = chart.options.categoryAxis,
                    //        baseUnitInputs = e.detailRow.find(".sensorDetailsOptions input:radio[name=baseUnit]"),
                    //        aggregateInputs = e.detailRow.find(".sensorDetailsOptions input:radio[name=aggregate]");

                    //    for (var i = 0, length = series.length; i < length; i++)
                    //        series[i].aggregate = aggregateInputs.filter(":checked").val();

                    //    categoryAxis.baseUnit = baseUnitInputs.filter(":checked").val();

                    //    chart.refresh();
                    //});

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
                            //format: "{0}qqqqq",
                            //template: "#= lib.kendo.toString(category, 'dd.MM.yyyy') #: #= value #"
                            template: "#= value #"
                        },


                        ////theme: "blueOpal",
                        //transitions: true,
                        ////style: "step",//"smooth",
                        //title: { text: e.data.TypeName + " статистика" },
                        //legend: { visible: true, position: "bottom" },
                        //series: [
                        //    {
                        //        categoryField: "TimeStamp",
                        //        field: "Value",
                        //        type: "area",
                        //        labels: {
                        //            //format: "{0}" + me.getSensorValueUnit(e.data),
                        //            visible: true
                        //            //background: "transparent"
                        //        },
                        //        line: {
                        //            color: "cornflowerblue",
                        //            //opacity: 0.5,
                        //            width: 0.5,
                        //            style: "smooth" // "step", ""
                        //        }
                        //    }
                        //],
                        //valueAxis: {
                        //    labels: {
                        //        //format: "{0}" + me.getSensorValueUnit(e.data),
                        //        visible: true
                        //    },
                        //    line: { visible: true },
                        //    majorGridLines: { visible: true },
                        //    //title: {
                        //    //    text: "wwwww",
                        //    //    background: "green",
                        //    //    border: {
                        //    //        width: 1,
                        //    //    }
                        //    //}
                        //    //min: 0,
                        //    //max: 120
                        //},
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