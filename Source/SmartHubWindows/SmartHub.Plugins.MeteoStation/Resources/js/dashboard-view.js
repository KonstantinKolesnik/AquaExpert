
define(
	['common', 'lib', 'text!webapp/meteostation/dashboard.html'],
    function (common, lib, templates) {
        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            onShow: function () {
                var me = this;
                var viewModel = me.options.viewModel;

                createMonitorsList();
                kendo.bind($("#content"), viewModel);

                function createMonitorsList() {
                    $("#listMonitors").kendoListView({
                        dataBound: function (e) {
                            if (this.dataSource.total() == 0)
                                return;

                            $.each($(".monitor-chart"), function (idx, selector) {
                                var type = viewModel.Monitors[idx].Type;
                                createMonitorWidget($(selector), type);
                            });
                        }
                    });

                    function createMonitorWidget(selector, type) {
                        selector.kendoChart({
                            series: [
                                {
                                    type: "line",
                                    style: "smooth",
                                    field: "Value",
                                    color: "cornflowerblue",
                                    axis: "axisValue",
                                    tooltip: {
                                        visible: true,
                                        template: function (data) {
                                            switch (type) {
                                                case "T": return kendo.toString(data.value, 'n1') + "&nbsp;°C";
                                                case "H": return kendo.toString(data.value, 'n1') + "&nbsp;%";
                                                case "P": return kendo.toString(data.value / 133.3, 'n2') + "&nbsp;mmHg";
                                                case "F":
                                                    var weather = [ "Ясно", "Солнечно", "Облачно", "К дождю", "Дождь", "-" ];
                                                    return weather[data.value];
                                                default: return data.value;
                                            }
                                        }
                                    }
                                },
                            ],
                            valueAxes: [
                                {
                                    name: "axisValue",
                                    majorUnit: type == "P" ? 133 : 1,
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
                                        template: function (data) {
                                            switch (type) {
                                                case "T": return kendo.toString(data.value, 'n1') + "&nbsp;°C";
                                                case "H": return kendo.toString(data.value, 'n1') + "&nbsp;%";
                                                case "P": return kendo.toString(data.value / 133.3, 'n2') + "&nbsp;mmHg";
                                                case "F":
                                                    var weather = ["Ясно", "Солнечно", "Облачно", "К дождю", "Дождь", "-"];
                                                    return weather[data.value];

                                                    //var icon = "wi-cloud-refresh";
                                                    //switch (data.value) {
                                                    //    case 0: icon = "wi-day-cloudy"; break;
                                                    //    case 1: icon = "wi-day-sunny"; break;
                                                    //    case 2: icon = "wi-cloudy"; break;
                                                    //    case 3: icon = "wi-day-rain-mix"; break;
                                                    //    case 4: icon = "wi-storm-showers"; break;
                                                    //    case 5: icon = ""; break;
                                                    //}
                                                    //return "<span class='" + icon + "' style='font-size:22px;'></span>";

                                                default: return data.value;
                                            }
                                        }
                                    }
                                },
                            ],
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

                                //axisCrossingValues: [0],
                                labels: {
                                    //dateFormats: {
                                    //    //minutes: "mm:ss",
                                    //    hours: "d.MM HH:mm",
                                    //    days: "MMM d",
                                    //    weeks: "MMM d",
                                    //    months: "yyyy MMM",
                                    //    years: "yyyy"
                                    //},
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