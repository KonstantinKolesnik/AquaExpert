﻿
define(['jquery', 'text!webapp/monitors/utils.html'],
    function ($, templates) {
        var api = {
            createMonitorChart: function (selector, monitor) {
                selector.empty();
                selector.html($(templates));

                var config = this.getDefaultConfiguration();
                if (monitor && monitor.Configuration)
                    //try {
                    config = JSON.parse(monitor.Configuration);
                    //}
                    //catch(e) {
                    //    config = this.getDefaultConfiguration();
                    //}

                kendo.bind(selector.find(".monitor-view"), monitor);

                return selector.find(".monitor-chart").kendoChart(config).data("kendoChart");
            },
            getDefaultConfiguration: function () {
                return {
                    series: [
                        {
                            field: "Value",
                            axis: "axisValue",

                            type: "line",
                            /*
                            area
                            bar
                            bubble
                            bullet
                            candlestick
                            column
                            donut
                            funnel
                            horizontalWaterfall
                            line
                            ohlc
                            pie
                            polarArea
                            polarLine
                            polarScatter
                            radarArea
                            radarColumn
                            radarLine
                            rangeBar
                            rangeColumn
                            scatter
                            scatterLine
                            verticalArea
                            verticalBullet
                            verticalLine
                            waterfall
                            */

                            style: "smooth",
                                //"normal" - The values will be connected with straight line.
                                //"step" - The values will be connected with a line with right angle.
                                //"smooth" - The values will be connected with a smooth line.

                            color: "cornflowerblue",
                            tooltip: {
                                visible: true,
                                template: "#= kendo.toString(data.value, 'n1') #"
                            }
                        },
                    ],
                    valueAxes: [
                        {
                            name: "axisValue",
                            //majorUnit: type == "P" ? 133 : 1,
                            //majorTicks: {
                            //    step: 1
                            //},
                            //minorTicks: {
                            //    size: 3,
                            //    //color: "red",
                            //    width: 2,
                            //    visible: true
                            //},
                            //min: 93300,
                            //max: 104000,
                            labels: {
                                //format: "{0} °C",
                                font: "10px Segoe UI",
                                template: "#= kendo.toString(data.value, 'n1') #",
                                //template: function (data) {
                                //    switch (type) {
                                //        case "T": return kendo.toString(data.value, 'n1') + "&nbsp;°C";
                                //        case "H": return kendo.toString(data.value, 'n1') + "&nbsp;%";
                                //        case "P": return kendo.toString(data.value / 133.3, 'n2') + "&nbsp;mmHg";
                                //        case "F":
                                //            var weather = ["Ясно", "Солнечно", "Облачно", "К дождю", "Дождь", "-"];
                                //            return weather[data.value];

                                //            //var icon = "wi-cloud-refresh";
                                //            //switch (data.value) {
                                //            //    case 0: icon = "wi-day-cloudy"; break;
                                //            //    case 1: icon = "wi-day-sunny"; break;
                                //            //    case 2: icon = "wi-cloudy"; break;
                                //            //    case 3: icon = "wi-day-rain-mix"; break;
                                //            //    case 4: icon = "wi-storm-showers"; break;
                                //            //    case 5: icon = ""; break;
                                //            //}
                                //            //return "<span class='" + icon + "' style='font-size:22px;'></span>";

                                //        default: return data.value;
                                //    }
                                //}
                            }
                        },
                    ],
                    categoryAxis: {
                        field: "TimeStamp",

                        //type: "date",
                        type: "category",

                        baseUnit: "fit",
                        //baseUnit: "seconds",
                        //baseUnit: "minutes",
                        //baseUnit: "hours",
                        //baseUnit: "days",
                        //baseUnit: "weeks",
                        //baseUnit: "months",
                        //baseUnit: "years",

                        //axisCrossingValues: [0],
                        labels: {
                            rotation: 315,
                            font: "9px Segoe UI",
                            dateFormats: {
                                minutes: "mm:ss",
                                hours: "d.MM HH:mm",
                                days: "MMM d",
                                weeks: "MMM d",
                                months: "yyyy MMM",
                                years: "yyyy"
                            },
                            template: "#: kendo.toString(new Date(value), 'd.MM - HH:mm') #"
                        },
                        line: { visible: true },
                        majorGridLines: { visible: true }
                    }
                };
            }
        };

        return {
            createMonitorChart: api.createMonitorChart,
            getDefaultConfiguration: api.getDefaultConfiguration
        };
});