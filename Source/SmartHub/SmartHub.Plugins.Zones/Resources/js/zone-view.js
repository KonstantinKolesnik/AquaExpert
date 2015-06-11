﻿
define(
	['common', 'lib', 'webapp/monitors/utils', 'text!webapp/zones/zone.html'],
    function (common, lib, monitorUtils, templates) {

        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            events: {
                'click .script-view': 'scriptClick'
            },
            scriptClick: function (e) {
                e.preventDefault();
                var id = $(e.currentTarget).attr("scriptId");
                this.trigger('script:run', id);
            },

            onShow: function () {
                var me = this;

                createListView($("#lvMonitors"), "monitors");
                createListView($("#lvControllers"), "controllers");
                createListView($("#lvScripts"), "scripts");

                kendo.bind($("#content"), me.options.viewModel);

                function createListView(selector, type) {
                    selector.kendoListView({
                        dataBound: function (e) {
                            if (type == "scripts")
                                return;

                            $.each($(".entity-view-holder"), function (idx, selectorEntity) {
                                var entity = null;

                                $.each(getEntitiesList(type), function (idx, item) {
                                    if (item.Id == $(selectorEntity).attr("entityId")) {
                                        entity = item;
                                        return false;
                                    }
                                });

                                processEntity(entity, type, selectorEntity);
                            });
                        }
                    });

                    function getEntitiesList(type) {
                        switch (type) {
                            case "monitors": return me.options.viewModel.Zone.MonitorsList;
                            case "controllers": return me.options.viewModel.Zone.ControllersList;
                            case "scripts": return me.options.viewModel.Zone.ScriptsList;
                            default: return [];
                        }
                    }
                    function processEntity(entity, type, selectorEntity) {
                        if (entity) {
                            switch (type) {
                                case "monitors": monitorUtils.createMonitorChart($(selectorEntity), entity); break;
                                case "controllers": $(selectorEntity).html("controller"); break;
                                default: break;
                            }
                        }
                    }
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