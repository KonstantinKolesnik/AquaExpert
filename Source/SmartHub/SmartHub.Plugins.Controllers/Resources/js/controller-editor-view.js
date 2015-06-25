
define(
	['common', 'lib', 'text!webapp/controllers/controller-editor.html'],
    function (common, lib, templates) {
        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            onShow: function () {
                var me = this;
                var viewModel = this.options.viewModel;

                var type = viewModel.Controller.Type;

                var tmpl = $("#tmpl" + type);
                if (!tmpl.length)
                    tmpl = $("#tmplError");
                $("#container").html(tmpl.html());

                var fn = null;
                switch (type) {
                    case 0: fn = initHeaterController; break;
                    case 1: fn = initSwitchController; break;
                    case 2: fn = initWaterLevelController; break;

                }
                if (fn)
                    fn();

                kendo.bind($("#content"), viewModel);

                function initHeaterController() {
                    //Switch = 3,             // Switch Actuator (on/off)
                    //Temperature = 6,        // Temperature sensor

                    createSensorSelector($("#ddlHeaterSensorTemperature"), 6);
                    createSensorSelector($("#ddlHeaterSensorSwitch"), 3);
                    createNumericTextBox($("#ntbHeaterTemperatureCalibration"), -10, 10, "n1", 1);
                    createNumericTextBox($("#ntbHeaterTemperatureMin"), 18, 32, "n1", 1);
                    createNumericTextBox($("#ntbHeaterTemperatureMax"), 18, 32, "n1", 1);
                    createNumericTextBox($("#ntbHeaterTemperatureAlarmMin"), 18, 32, "n1", 1);
                    createNumericTextBox($("#ntbHeaterTemperatureAlarmMax"), 18, 32, "n1", 1);
                    createTextBox($("#tbHeaterTemperatureAlarmMaxText"));
                    createTextBox($("#tbHeaterTemperatureAlarmMinText"));
                }
                function initSwitchController() {
                    var ddlNewPeriodFrom;
                    var ddlNewPeriodTo;
                    var gridPeriods;

                    //Switch = 3,             // Switch Actuator (on/off)
                    createSensorSelector($("#ddlSwitchSensorSwitch"), 3);
                    ddlNewPeriodFrom = createDatePointSelector($("#ddlNewPeriodFrom"));
                    ddlNewPeriodTo = createDatePointSelector($("#ddlNewPeriodTo"));
                    createPeriodsGrid($("#gridPeriods"));

                    ddlNewPeriodFrom.bind("change", function () {
                        //var startTime = ddlNewPeriodFrom.value();

                        //if (startTime) {
                        //    startTime = new Date(startTime);

                        //    ddlNewPeriodTo.max(startTime);

                        //    startTime.setMinutes(startTime.getMinutes() + this.options.interval);

                        //    ddlNewPeriodTo.min(startTime);
                        //    ddlNewPeriodTo.value(startTime);
                        //}
                    });
                    $("#btnAddPeriod").on("click", function () {
                        gridPeriods.dataSource.add({
                            From: ddlNewPeriodFrom.value(),//.toUTCString(),
                            To: ddlNewPeriodTo.value(),//.toLocaleString(),
                            IsEnabled: $("#chbNewPeriodIsEnabled").prop("checked")
                        });
                    });

                    function createPeriodsGrid(selector) {
                        gridPeriods = selector.kendoGrid({
                            height: 350,
                            sortable: true,
                            resizable: true,
                            editable: true,
                            pageable: {
                                pageSizes: [10, 20],
                                pageSize: 10
                            },
                            columns: [
                                { field: "From", title: "С", width: 80, editor: getEditor, attributes: { "class": "text-center" }, template: "#: kendo.toString(new Date(From), 'HH:mm') #" },
                                { field: "To", title: "По", width: 80, editor: getEditor, attributes: { "class": "text-center" }, template: "#: kendo.toString(new Date(To), 'HH:mm') #" },
                                { field: "IsEnabled", title: "Разрешен", width: 80, editor: getEditor, attributes: { "class": "text-center" }, template: kendo.template($("#tmplIsEnabled").html()) },
                                {
                                    title: "&nbsp;", width: 100, reorderable: false, sortable: false, editor: getEditor, attributes: { "class": "text-center" },
                                    command: [
                                        {
                                            text: "Удалить",
                                            click: function (e) {
                                                e.preventDefault();
                                                e.stopPropagation();

                                                var item = this.dataItem($(e.currentTarget).closest("tr"));
                                                if (common.utils.confirm('Удалить период?'))
                                                    gridPeriods.dataSource.remove(item);
                                            }
                                        }
                                    ]
                                }
                            ]
                        }).data("kendoGrid");
                    }
                    function getEditor(container, options) {
                        var grid = container.closest(".k-grid").data("kendoGrid");

                        if (options.field == "From" || options.field == "To") {
                            var oldValue = options.model[options.field];

                            var editor = $("<input style='width:100%;'/>");
                            editor.appendTo(container).show().focus();

                            var editorCtrl = createDatePointSelector(editor);
                            editorCtrl.value(new Date(oldValue));
                            editorCtrl.bind("change", function () {
                                var newValue = this.value();
                                if (newValue != oldValue)
                                    options.model.set(options.field, newValue);

                                grid.closeCell();
                            });
                        }
                        else if (options.field == "IsEnabled") {
                            var oldValue = options.model[options.field];

                            var editor = $("<input type='checkbox' style='width:100%;'/>");
                            editor.appendTo(container)
                                .show().focus()
                                .unbind("keydown").keydown(preventEnter)
                                .prop("checked", oldValue)
                                .change(function () {
                                    var newValue = editor.prop("checked");
                                    if (newValue != oldValue)
                                        options.model.set("IsEnabled", newValue);

                                    grid.closeCell();
                                });
                        }
                        else
                            grid.closeCell();
                    }
                }
                function initWaterLevelController() {
                    viewModel.Controller.Configuration.ExchangeTime = new Date(viewModel.Controller.Configuration.ExchangeTime);

                    //Switch = 3,             // Switch Actuator (on/off)
                    //Distance = 15,          // Distance sensor
                    createSensorSelector($("#ddlWaterLevelSensorDistance"), 15);
                    createSensorSelector($("#ddlWaterLevelSensorInSwitch"), 3);
                    createSensorSelector($("#ddlWaterLevelSensorOutSwitch"), 3);
                    createNumericTextBox($("#ntbWaterLevelDistanceMin"), 2, 10, "n0", 0);
                    createNumericTextBox($("#ntbWaterLevelDistanceMax"), 4, 20, "n0", 0);
                    createNumericTextBox($("#ntbWaterLevelDistanceAlarmMin"), 0, 22, "n0", 0);
                    createTextBox($("#tbWaterLevelDistanceAlarmMinText"));
                    createWeekDaySelector($("#ddlWaterLevelExchangeWeekDay"));
                    createDatePointSelector($("#ddlWaterLevelExchangeTime"));
                }

                function createSensorSelector(selector, type) {
                    selector.kendoDropDownList({
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: {
                                    url: function () { return document.location.origin + "/api/mysensors/sensorsByType" },
                                    //dataType: "jsonp"
                                    data: { type: type }
                                }
                            }
                        }),
                        dataValueField: "Id",
                        dataTextField: "Name"
                    });
                }
                function createTextBox(selector) {
                    selector.unbind("keydown").keydown(preventEnter);
                }
                function createNumericTextBox(selector, min, max, format, decimals) {
                    selector.kendoNumericTextBox({
                        min: min,
                        max: max,
                        step: 1,
                        format: format,
                        decimals: decimals
                    });
                }
                function createWeekDaySelector(selector) {
                    selector.kendoDropDownList({
                        dataSource: [
                            { value: "1", text: "Понедельник" },
                            { value: "2", text: "Вторник" },
                            { value: "3", text: "Среда" },
                            { value: "4", text: "Четверг" },
                            { value: "5", text: "Пятница" },
                            { value: "6", text: "Суббота" },
                            { value: "0", text: "Воскресенье" }
                        ],
                        dataValueField: "value",
                        dataTextField: "text",
                        valuePrimitive: true
                    });
                }
                function createDatePointSelector(selector) {
                    return selector.kendoTimePicker({
                        min: new Date(2000, 0, 0, 0, 0, 0),
                        max: new Date(2000, 0, 0, 23, 59, 0),
                        value: new Date(2000, 0, 0, 0, 0, 0),
                        format: "HH:mm",
                        culture: "ru-RU",
                        interval: 30
                    }).data("kendoTimePicker");
                }
                function preventEnter(e) {
                    if (e.keyCode == 13) {
                        e.preventDefault();
                        e.stopPropagation();
                        $(e.target).blur(); //run saving
                    }
                }
            }
        });

        return {
            LayoutView: layoutView
        };
    });