
define(
	['common', 'lib', 'text!webapp/meteostation/module-main.html'],
    function (common, lib, templates) {
        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            onShow: function () {
                var me = this;

                createSensorValuesChart($("#sensorsChart"));

                function createSensorValuesChart(selector) {
                    selector.kendoChart({
                        //theme: "MetroBlack",
                        title: {
                            text: "Статистика"
                        },
                        legend: {
                            position: "right"
                        },
                        seriesDefault: {
                            //line: {
                            //    style: "smooth"
                            //},
                            style: "smooth"
                            //aggregate: "avg",
                        },
                        series: [
                            {
                                type: "line",
                                name: "T in",
                                field: "TI",
                                color: "red",
                                axis: "temp",
                                tooltip: {
                                    visible: true,
                                    template: "#= kendo.toString(value, 'n1') #&nbsp;°C"
                                }
                            },
                            {
                                type: "line",
                                name: "Hum in",
                                field: "HI",
                                color: "blue",
                                axis: "hum",
                                tooltip: {
                                    visible: true,
                                    template: "#= kendo.toString(value, 'n1') #&nbsp;%"
                                }
                            },
                            {
                                type: "line",
                                name: "T out",
                                field: "TO",
                                color: "pink",
                                axis: "temp",
                                tooltip: {
                                    visible: true,
                                    template: "#= kendo.toString(value, 'n1') #&nbsp;°C"
                                }
                            },
                            {
                                type: "line",
                                name: "Hum out",
                                field: "HO",
                                color: "cornflowerblue",
                                axis: "hum",
                                tooltip: {
                                    visible: true,
                                    template: "#= kendo.toString(value, 'n1') #&nbsp;%"
                                }
                            },
                            {
                                type: "area",
                                line: {
                                    style: "smooth"
                                },
                                name: "P",
                                field: "P",
                                color: "lightgreen",
                                axis: "press",
                                tooltip: {
                                    visible: true,
                                    template: "#= kendo.toString(value / 133.3, 'n2') #&nbsp;mmHg"
                                }
                            },
                        ],
                        //tooltip: {
                        //    visible: true,
                        //    //format: "Value: {0:N0}",
                        //    //template: "${category} - ${value}"
                        //    template: "#= kendo.toString(value, 'n1') #"
                        //},
                        valueAxes: [
                            {
                                name: "temp",
                                color: "red",
                                //min: -50,
                                //max: 50,
                                labels: {
                                    format: "{0} °C",
                                }
                            },
                            {
                                name: "hum",
                                color: "cornflowerblue",
                                //min: 0,
                                //max: 100,
                                labels: {
                                    format: "{0} %",
                                }
                            },
                            {
                                name: "press",
                                color: "lightgreen",
                                //min: 30000,
                                //max: 110000,
                                min: 93300,
                                max: 104000,
                                labels: {
                                    //format: "{0} Pa",
                                    template: "#= kendo.toString(value / 133.3, 'n2') #&nbsp;mmHg"
                                }
                            },
                        ],
                        categoryAxis: {
                            field: "TimeStamp",
                            type: "date",

                            //baseUnit: "fit",
                            //baseUnit: "seconds",
                            //baseUnit: "minutes",
                            baseUnit: "hours",
                            //baseUnit: "days",
                            //baseUnit: "weeks",
                            //baseUnit: "months",
                            //baseUnit: "years",

                            axisCrossingValues: [0, 0, 1000000],
                            labels: {
                                dateFormats: {
                                    hours: "MMM d, HH:mm",
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

            bindModel: function (viewModel) {
                lib.kendo.bind($("#content"), viewModel);
            }
        });

        return {
            LayoutView: layoutView
        };
    });