
define(
	['common', 'lib', 'webapp/monitors/utils', 'webapp/controllers/utils', 'text!webapp/zones/zone.html'],
    function (common, lib, monitorUtils, controllerUtils, templates) {
        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            events: {
                'click .script-button': 'scriptClick'
            },
            scriptClick: function (e) {
                e.preventDefault();
                var id = $(e.currentTarget).attr("scriptId");
                this.trigger('script:run', id);
            },

            onShow: function () {
                var me = this;
                var viewModel = me.options.viewModel;

                $("#monitorsHolder").toggle(viewModel.Zone.MonitorsList.length > 0);
                $("#controllersHolder").toggle(viewModel.Zone.ControllersList.length > 0);
                $("#scriptsHolder").toggle(viewModel.Zone.ScriptsList.length > 0);

                if (viewModel.Zone.MonitorsList.length > 0)
                    createListView($("#lvMonitors"), "monitors");
                if (viewModel.Zone.ControllersList.length > 0)
                    createListView($("#lvControllers"), "controllers");
                if (viewModel.Zone.ScriptsList.length > 0)
                    createListView($("#lvScripts"), "scripts");

                kendo.bind($("#content"), viewModel);

                function createListView(selector, type) {
                    selector.kendoListView({
                        dataBound: function (e) {
                            if (this.dataSource.total() == 0)
                                return;

                            if (type == "scripts")
                                return;

                            var entities = getEntitiesList(type);

                            $.each($(".entity-view-holder"), function (idx, selectorEntity) {
                                var entity = null;

                                $.each(entities, function (idx, item) {
                                    if (item.Id == $(selectorEntity).attr("entityId")) {
                                        entity = item;
                                        return false;
                                    }
                                });

                                setTimeout(function () { processEntity(entity, type, selectorEntity); }, 0);
                            });

                            function getEntitiesList(type) {
                                switch (type) {
                                    case "monitors": return viewModel.Zone.MonitorsList;
                                    case "controllers": return viewModel.Zone.ControllersList;
                                    case "scripts": return viewModel.Zone.ScriptsList;
                                    default: return [];
                                }
                            }
                            function processEntity(entity, type, selectorEntity) {
                                if (entity) {
                                    switch (type) {
                                        case "monitors": monitorUtils.createMonitorChart($(selectorEntity), entity); break;
                                        case "controllers": controllerUtils.createControllerWidget($(selectorEntity), entity); break;
                                        default: break;
                                    }
                                }
                            }
                        }
                    });
                }
                function createGraph_ForExample(selector) {
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
