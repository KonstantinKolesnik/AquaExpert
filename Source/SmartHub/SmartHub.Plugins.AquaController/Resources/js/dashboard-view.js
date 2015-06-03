
define(
	['common', 'lib', 'text!webapp/aquacontroller/dashboard.html'],
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
                
                createMonitorsList();
                //createCheckBox($("#chbHeaterAutoMode"));
                //createHeaterChart($("#heaterChart"));


                function createMonitorsList() {
                    //var dataSource = new kendo.data.DataSource({
                    //    transport: {
                    //        read: {
                    //            url: function () { return document.location.origin + "/api/aquacontroller/monitor/list" },
                    //            //dataType: "jsonp"
                    //        }
                    //    },
                    //    pageSize: 20
                    //});

                    //$("#listMonitorsPager").kendoPager({
                    //    dataSource: dataSource
                    //});

                    $("#listMonitors").kendoListView({
                        dataBound: function () {
                            $.each($(".monitor-chart"), function (idx, selector) { createMonitorChart($(selector)); });
                        }
                    });

                    $("#listMonitors").kendoSortable({
                        filter: ">div.monitor",
                        cursor: "move",
                        placeholder: function (element) {
                            return element.clone().css("opacity", 0.1);
                        },
                        hint: function (element) {
                            return element.clone().removeClass("k-state-selected");
                        },
                        change: function (e) {
                            //var skip = dataSource.skip(),
                            //    oldIndex = e.oldIndex + skip,
                            //    newIndex = e.newIndex + skip,
                            //    data = dataSource.data(),
                            //    dataItem = dataSource.getByUid(e.item.data("uid"));

                            //dataSource.remove(dataItem);
                            //dataSource.insert(newIndex, dataItem);
                        }
                    });

                    function createMonitorChart(selector) {
                        selector.kendoChart({
                            //theme: "MaterialBlack",
                            theme: "Black",
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
                                type: "date",

                                baseUnit: "fit",
                                //baseUnit: "seconds",
                                //baseUnit: "minutes",
                                //baseUnit: "hours",
                                //baseUnit: "days",
                                //baseUnit: "weeks",
                                //baseUnit: "months",
                                //baseUnit: "years",

                                axisCrossingValues: [0],
                                labels: {
                                    //dateFormats: {
                                    //    //minutes: "mm:ss",
                                    //    hours: "MMM d, HH:mm",
                                    //    //hours: "HH:mm",
                                    //    days: "MMM d",
                                    //    weeks: "MMM d",
                                    //    months: "yyyy MMM",
                                    //    years: "yyyy"
                                    //},
                                    visible: true,
                                    rotation: 270,
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



                //function createCheckBox(selector) {
                //    selector.change(function () {
                //        debugger;
                //        me.trigger("heaterControllerConfiguration:set");
                //    });
                //}
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

            },

            bindModel: function (viewModel) {
                lib.kendo.bind($("#content"), viewModel);
            }
        });

        return {
            LayoutView: layoutView
        };
    });