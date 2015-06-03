
define(
	['common', 'lib', 'text!webapp/aquacontroller/module-settings.html'],
    function (common, lib, templates) {
        var ddlNewMonitorSensor;
        var gridMonitors;
        var ddlNewControllerType;
        var gridControllers;

        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            events: {
                'click .js-btn-add-monitor': 'addMonitor',
                'click .js-btn-add-controller': 'addController'
            },
            addMonitor: function (e) {
                e.preventDefault();
                this.trigger('monitor:add', $("#tbNewMonitorName").val(), ddlNewMonitorSensor.value(), $("#chbNewMonitorIsVisible").prop("checked"));
            },
            addController: function (e) {
                e.preventDefault();
                this.trigger('controller:add', $("#tbNewControllerName").val(), ddlNewControllerType.value(), $("#chbNewControllerIsVisible").prop("checked"));
            },
            //triggers: {
            //    'click .js-add-monitor': 'monitor:add'
            //},

            refreshMonitorsGrid: function () {
                gridMonitors.dataSource.read();
            },
            refreshControllersGrid: function () {
                gridControllers.dataSource.read();
            },

            onShow: function () {
                var me = this;

                createTabStrip($("#tabstrip"));
                createMonitorsTab();
                createControllersTab();



                //createSensorSelector($("#ddlHeaterSensorTemperature"));
                //createSensorSelector($("#ddlHeaterSensorSwitch"));
                //createNumericTextBox($("#ntbHeaterTemperatureCalibration"), -10, 10, "n1", 1);
                //createNumericTextBox($("#ntbHeaterTemperatureMin"), 18, 32, "n1", 1);
                //createNumericTextBox($("#ntbHeaterTemperatureMax"), 18, 32, "n1", 1);
                //createNumericTextBox($("#ntbHeaterTemperatureAlarmMin"), 18, 32, "n1", 1);
                //createNumericTextBox($("#ntbHeaterTemperatureAlarmMax"), 18, 32, "n1", 1);
                //createTextBox($("#tbHeaterTemperatureAlarmMaxText"));
                //createTextBox($("#tbHeaterTemperatureAlarmMinText"));

                function createTabStrip(selector) {
                    selector.kendoTabStrip({
                        animation: {
                            open: { effects: "fadeIn" }
                        },
                        activate: function () {
                            //if (selector = $("#tabstrip"))
                            //    adjustSizes();
                        }
                    });
                }
                function createMonitorsTab() {
                    createMonitorAddForm();
                    createMonitorsGrid();

                    function createMonitorAddForm() {
                        ddlNewMonitorSensor = $("#ddlNewMonitorSensor").kendoDropDownList({
                            dataSource: new kendo.data.DataSource({
                                transport: {
                                    read: {
                                        url: function () { return document.location.origin + "/api/mysensors/sensors" },
                                        //dataType: "jsonp"
                                    }
                                }
                            }),
                            dataValueField: "Id",
                            dataTextField: "Name",
                            optionLabel: "Выберите сенсор..."
                        }).data("kendoDropDownList");
                    }
                    function createMonitorsGrid() {
                        gridMonitors = $("#gridMonitors").kendoGrid({
                            height: 350,
                            dataSource: new kendo.data.DataSource({
                                transport: {
                                    read: {
                                        url: function () { return document.location.origin + "/api/aquacontroller/monitor/listall" },
                                    }
                                },
                                pageSize: 20
                            }),
                            sortable: true,
                            resizable: true,
                            editable: true,
                            pageable: {
                                pageSizes: [10, 20, 50, 100],
                                pageSize: 20
                            },
                            columns: [
                                { field: "Name", title: "Имя", editor: getEditor },
                                { field: "Sensor.Name", title: "Сенсор", editor: getEditor },
                                { field: "IsVisible", title: "Видимый", width: 80, editor: getEditor/*, template: kendo.template($("#tmplIsVisible").html())*/ },
                                {
                                    title: "&nbsp;", width: 80, reorderable: false, sortable: false, editor: getEditor, attributes: { "class": "text-center" },
                                    command: [
                                        {
                                            text: "Удалить",
                                            click: function (e) {
                                                e.preventDefault();
                                                e.stopPropagation();

                                                var item = this.dataItem($(e.currentTarget).closest("tr"));
                                                if (common.utils.confirm('Удалить монитор "{0}"?', item.Name))
                                                    me.trigger('monitor:delete', item.Id);
                                            }
                                        }
                                    ]
                                }
                            ]
                        }).data("kendoGrid");
                    }
                    function getEditor(container, options) {
                        var grid = container.closest(".k-grid").data("kendoGrid");

                        if (options.field == "Name") {
                            var oldValue = options.model[options.field];

                            var editor = $("<input type='text' class='k-textbox' style='width:100%;'/>");
                            editor.appendTo(container)
                                .show().focus()
                                .unbind("keydown").keydown(preventEnter)
                                .val(oldValue)
                                .blur(saveName);
                        }
                        else if (options.field == "IsVisible") {
                            var oldValue = options.model[options.field];

                            var editor = $("<input type='checkbox' style='width:100%;'/>");
                            editor.appendTo(container)
                                .show().focus()
                                .unbind("keydown").keydown(preventEnter)
                                .prop("checked", oldValue)
                                .change(saveIsVisible);
                        }
                        else
                            grid.closeCell();

                        function saveName(e) {
                            var newValue = editor.val();
                            if (newValue != oldValue)
                                me.trigger('monitor:setName', options.model.Id, newValue);
                        }
                        function saveIsVisible(e) {
                            var newValue = editor.prop("checked");
                            if (newValue != oldValue)
                                me.trigger('monitor:setIsVisible', options.model.Id, newValue);
                        }
                        function preventEnter(e) {
                            if (e.keyCode == 13) {
                                e.preventDefault();
                                e.stopPropagation();
                                $(e.target).blur(); //run saving
                            }
                        }
                    }
                }
                function createControllersTab() {
                    createControllerAddForm();
                    createControllersGrid();

                    function createControllerAddForm() {
                        ddlNewControllerType = $("#ddlNewControllerType").kendoDropDownList({
                            dataSource: new kendo.data.DataSource({
                                transport: {
                                    read: {
                                        url: function () { return document.location.origin + "/api/aquacontroller/controller/type/list" },
                                    }
                                }
                            }),
                            dataValueField: "Id",
                            dataTextField: "Name",
                            optionLabel: "Выберите тип..."
                        }).data("kendoDropDownList");
                    }
                    function createControllersGrid() {
                        gridControllers = $("#gridControllers").kendoGrid({
                            height: 350,
                            dataSource: new kendo.data.DataSource({
                                transport: {
                                    read: {
                                        url: function () { return document.location.origin + "/api/aquacontroller/controller/listall" },
                                    }
                                },
                                pageSize: 20
                            }),
                            sortable: true,
                            resizable: true,
                            editable: true,
                            pageable: {
                                pageSizes: [10, 20, 50, 100],
                                pageSize: 20
                            },
                            columns: [
                                { field: "Name", title: "Имя", editor: getEditor },
                                { field: "Type", title: "Тип", editor: getEditor },
                                { field: "IsVisible", title: "Видимый", width: 80, editor: getEditor/*, template: kendo.template($("#tmplIsVisible").html())*/ },
                                {
                                    title: "&nbsp;", width: 80, reorderable: false, sortable: false, editor: getEditor, attributes: { "class": "text-center" },
                                    command: [
                                        {
                                            text: "Удалить",
                                            click: function (e) {
                                                e.preventDefault();
                                                e.stopPropagation();

                                                var item = this.dataItem($(e.currentTarget).closest("tr"));
                                                if (common.utils.confirm('Удалить контроллер "{0}"?', item.Name))
                                                    me.trigger('controller:delete', item.Id);
                                            }
                                        }
                                    ]
                                }
                            ]
                        }).data("kendoGrid");
                    }
                    function getEditor(container, options) {
                        var grid = container.closest(".k-grid").data("kendoGrid");

                        if (options.field == "Name") {
                            var oldValue = options.model[options.field];

                            var editor = $("<input type='text' class='k-textbox' style='width:100%;'/>");
                            editor.appendTo(container)
                                .show().focus()
                                .unbind("keydown").keydown(preventEnter)
                                .val(oldValue)
                                .blur(saveName);
                        }
                        else if (options.field == "IsVisible") {
                            var oldValue = options.model[options.field];

                            var editor = $("<input type='checkbox' style='width:100%;'/>");
                            editor.appendTo(container)
                                .show().focus()
                                .unbind("keydown").keydown(preventEnter)
                                .prop("checked", oldValue)
                                .change(saveIsVisible);
                        }
                        else
                            grid.closeCell();

                        function saveName(e) {
                            var newValue = editor.val();
                            if (newValue != oldValue)
                                me.trigger('controller:setName', options.model.Id, newValue);
                        }
                        function saveIsVisible(e) {
                            var newValue = editor.prop("checked");
                            if (newValue != oldValue)
                                me.trigger('controller:setIsVisible', options.model.Id, newValue);
                        }
                        function preventEnter(e) {
                            if (e.keyCode == 13) {
                                e.preventDefault();
                                e.stopPropagation();
                                $(e.target).blur(); //run saving
                            }
                        }
                    }
                }





                //function createSensorSelector(selector) {
                //    selector.kendoDropDownList({
                //        dataValueField: "Id",
                //        dataTextField: "Name",
                //        change: function (e) { me.trigger("heaterControllerConfiguration:set"); }
                //    });
                //}
                //function createTextBox(selector) {
                //    selector.unbind("keydown").keydown(preventEnter).blur(function () { me.trigger("heaterControllerConfiguration:set"); });
                //}
                //function createNumericTextBox(selector, min, max, format, decimals) {
                //    selector.kendoNumericTextBox({
                //        min: min,
                //        max: max,
                //        step: 1,
                //        format: format,
                //        decimals: decimals,
                //        change: function (e) { me.trigger("heaterControllerConfiguration:set"); }
                //    });
                //}
                function preventEnter(e) {
                    if (e.keyCode == 13) {
                        e.preventDefault();
                        e.stopPropagation();
                        $(e.target).blur(); //run saving
                    }
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