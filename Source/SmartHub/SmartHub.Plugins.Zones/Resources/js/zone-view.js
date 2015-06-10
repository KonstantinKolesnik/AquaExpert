
define(
	['common', 'lib', 'webapp/monitors/utils', 'text!webapp/zones/zone.html'],
    function (common, lib, monitorUtils, templates) {

        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            onShow: function () {
                var me = this;

                createListView($("#lvMonitors"), "monitors");
                createListView($("#lvControllers"));
                createListView($("#lvScripts"));

                //createMultiSelector($("#msGraphs"), "graphs", "Id", "Name");

                //$("#sparkline").kendoSparkline([1, 2, 3, 2, 1]);
                //$("#sparkline").kendoSparkline([200, 450, 300, 125]);
                //$("#sparkline").kendoSparkline({
                //    series: [
                //        {
                //            name: "World",
                //            data: [15.7, 16.7, 20, 23.5, 26.6]
                //        }
                //    ],
                //    categoryAxis: {
                //        categories: [2005, 2006, 2007, 2008, 2009]
                //    }
                //});

                kendo.bind($("#content"), me.options.viewModel);

                function createListView(selector, type) {
                    var res = selector.kendoListView({
                        dataBound: function (e) {
                            $.each($(".monitor-view-holder"), function (idx, selector) {
                                var monitor = null;
                                $.each(me.options.viewModel.Zone.MonitorsList, function (idx, item) {
                                    if (item.Id == $(selector).attr("monitorId")) {
                                        monitor = item;
                                        return false;
                                    }
                                });

                                if (monitor)
                                    monitorUtils.createMonitorChart($(selector), monitor);
                            });
                        }
                    }).data("kendoListView");
                }

                function createMultiSelector(selector, entity, valueFieldName, textFieldName) {
                    return selector.kendoMultiSelect({
                        dataSource: {
                            transport: {
                                read: {
                                    url: function () { return document.location.origin + "/api/" + entity + "/list"; }
                                }
                            },
                        },
                        dataValueField: valueFieldName,
                        dataTextField: textFieldName,
                        valuePrimitive: true,
                        autoClose: false,
                        filter: "contains",
                        placeholder: "Выберите элементы...",
                    }).data("kendoMultiSelect");
                }
                function createHeaterChart_ForExample(selector) {
                    selector.kendoChart({
                        theme: "MaterialBlack",
                        title: {
                            text: "Статистика"
                        },
                        legend: {
                            position: "right"
                        },
                        //seriesDefault: {
                        //    //aggregate: "avg",
                        //},
                        series: [
                            {
                                type: "line",
                                style: "smooth",
                                name: "Температура воды",
                                field: "T",
                                color: "red",
                                axis: "temp",
                                tooltip: {
                                    visible: true,
                                    template: "#= kendo.toString(value, 'n1') #&nbsp;°C"
                                }
                            },
                            {
                                //type: "bar",
                                name: "Обогреватель",
                                field: "S",
                                color: "lightgreen",
                                axis: "switch",
                                tooltip: {
                                    visible: true,
                                    template: "#= value ? 'Вкл.' : 'Выкл.' #"
                                }
                            },
                        ],
                        valueAxes: [
                            {
                                name: "temp",
                                color: "red",
                                title: {
                                    text: "Температура воды",
                                    //background: "green",
                                    //border: {
                                    //    width: 5,
                                    //}
                                },
                                min: 18,
                                max: 32,
                                majorUnit: 1,
                                majorTicks: {
                                    step: 1
                                },
                                //minorTicks: {
                                //    size: 2,
                                //    color: "red",
                                //    width: 5,
                                //    visible: true
                                //},
                                labels: {
                                    format: "{0} °C",
                                }
                            },
                            {
                                name: "switch",
                                color: "lightgreen",
                                title: {
                                    text: "Обогреватель"
                                },
                                majorUnit: 1,
                                min: 0,
                                max: 1,
                                labels: {
                                    template: "#= value ? 'Вкл.' : 'Выкл.' #"
                                }
                            },
                        ],
                        categoryAxis: {
                            field: "TimeStamp",
                            type: "date",

                            //baseUnit: "fit",
                            //baseUnit: "seconds",
                            baseUnit: "minutes",
                            //baseUnit: "hours",
                            //baseUnit: "days",
                            //baseUnit: "weeks",
                            //baseUnit: "months",
                            //baseUnit: "years",

                            axisCrossingValues: [0, 0],
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
            }
        });

        return {
            LayoutView: layoutView
        };
    });