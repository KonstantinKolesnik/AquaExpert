
define(
	['common', 'lib', 'text!webapp/zones/dashboard.html'],
    function (common, lib, templates) {
        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            //regions: {
            //    filter: '#region-dashboard',
            //    list: '#region-graphs'
            //},
            //events: {
            //    'click .js-btn-add-monitor': 'addMonitor'
            //},
            //addMonitor: function (e) {
            //    e.preventDefault();
            //    this.trigger('monitor:add', $("#tbNewMonitorName").val(), ddlNewMonitorSensor.value());
            //},
            triggers: {
                'click .js-btn-graphs': 'graphs:show'
            },

            onShow: function () {
                var me = this;
                var listZones = null;
                
                createZonesList();
                //createHeaterChart($("#heaterChart"));

                kendo.bind($("#content"), this.options.viewModel);

                function createZonesList() {
                    //var dataSource = new kendo.data.DataSource({
                    //    transport: {
                    //        read: {
                    //            url: function () { return document.location.origin + "/api/aquacontroller/monitor/list" },
                    //            //dataType: "jsonp"
                    //        }
                    //    },
                    //    pageSize: 20
                    //});

                    //$("#listZonesPager").kendoPager({
                    //    dataSource: dataSource
                    //});

                    listZones = $("#listZones").kendoListView({
                        selectable: "single",
                        change: function (e) {
                            var item = listZones.dataItems()[this.select().index()];
                            me.trigger('zone:select', item);

                            //var data = dataSource.view(),
                            //    selected = $.map(this.select(), function (item) {
                            //        return data[$(item).index()].Name;
                            //    });
                        },
                        //dataBound: function () {
                        //    $.each($(".monitor-chart"), function (idx, selector) { createMonitorChart($(selector)); });
                        //}
                    }).data("kendoListView");

                    function createMonitorChart(selector) {
                        selector.kendoChart({
                            series: [
                                {
                                    type: "line",
                                    style: "smooth",
                                    field: "Value",
                                    color: "cornflowerblue",//getRandomColor(),
                                    //axis: "axisValue",
                                    tooltip: {
                                        visible: true,
                                        //template: "#= kendo.toString(value, 'n1') #&nbsp;°C"
                                        //template: "#= kendo.toString(category, 'MMM d, HH:mm:ss') + " - " + kendo.toString(value, 'n1') #"
                                        //template: "#= category + " - " + kendo.toString(value, 'n1') #"
                                    },
                                    //aggregate: "avg",
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
                            //        labels: {
                            //            //format: "{0} °C",
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
                    function getRandomColor() {
                        var letters = '0123456789ABCDEF'.split('');
                        var color = '#';
                        for (var i = 0; i < 6; i++) {
                            color += letters[Math.floor(Math.random() * 16)];
                        }
                        return color;
                    }
                }

                function createHeaterChart(selector) {
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