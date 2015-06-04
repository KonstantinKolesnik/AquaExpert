
define(
	['common', 'lib', 'text!webapp/meteostation/dashboard.html'],
    function (common, lib, templates) {
        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            onShow: function () {
                var me = this;

                createMonitorsList();
                kendo.bind($("#content"), this.options.viewModel);

                function createMonitorsList() {
                    $("#listMonitors").kendoListView({
                        dataBound: function () {
                            $.each($(".monitor-chart"), function (idx, selector) { createMonitorChart($(selector)); });
                        }
                    });

                    function createMonitorChart(selector) {
                        selector.kendoChart({
                            series: [
                                {
                                    type: "line",
                                    style: "smooth",
                                    field: "Value",
                                    color: "cornflowerblue",
                                    //axis: "axisValue",
                                    tooltip: {
                                        visible: true,

                                        //template: "#= kendo.toString(value, 'n1') #&nbsp;°C",
                                        //template: "#= kendo.toString(value, 'n1') #&nbsp;%",
                                        //template: "#= kendo.toString(value / 133.3, 'n2') #&nbsp;mmHg"

                                    }
                                },
                            ],
                            //valueAxes: [
                            //    {
                            //        name: "axisValue",
                            //        //majorUnit: 1,
                            //        //majorTicks: {
                            //        //    step: 1
                            //        //},
                            //        //minorTicks: {
                            //        //    size: 3,
                            //        //    //color: "red",
                            //        //    width: 2,
                            //        //    visible: true
                            //        //},
                            //          min: 93300,
                            //          max: 104000,
                            //        labels: {
                            //            //format: "{0} °C",
                            //              format: "{0} %",
                            //              template: "#= kendo.toString(value / 133.3, 'n2') #&nbsp;mmHg"
                            //        }
                            //    },
                            //],
                            categoryAxis: {
                                field: "TimeStamp",
                                //type: "date",

                                //baseUnit: "fit",
                                //baseUnit: "seconds",
                                //baseUnit: "minutes",
                                //baseUnit: "hours",
                                //baseUnit: "days",
                                //baseUnit: "weeks",
                                //baseUnit: "months",
                                //baseUnit: "years",

                                axisCrossingValues: [0],
                                labels: {
                                    dateFormats: {
                                        //minutes: "mm:ss",
                                        hours: "d.MM HH:mm",
                                        days: "MMM d",
                                        weeks: "MMM d",
                                        months: "yyyy MMM",
                                        years: "yyyy"
                                    },
                                    visible: true,
                                    rotation: 270,
                                    font: "9px sans-serif",
                                    template: "#: kendo.toString(new Date(value), 'd.MM HH:mm') #"
                                },
                                line: { visible: true },
                                majorGridLines: { visible: true }
                            }
                        });
                    }
                }
            }
        });

        return {
            LayoutView: layoutView
        };
    });