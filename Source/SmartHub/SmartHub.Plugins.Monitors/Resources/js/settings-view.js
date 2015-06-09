
define(
	['common', 'lib', 'text!webapp/monitors/settings.html'],
    function (common, lib, templates) {
        var ddlNewMonitorSensor;
        var ddlNewMonitorType;
        var gridMonitors;

        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            events: {
                'click .js-btn-add-monitor': 'addMonitor'
            },
            addMonitor: function (e) {
                e.preventDefault();
                this.trigger('monitor:add', $("#tbNewMonitorName").val(), ddlNewMonitorSensor.value(), ddlNewMonitorType.value());
            },
            refreshMonitorsGrid: function () {
                gridMonitors.dataSource.read();
            },

            onShow: function () {
                var me = this;

                createMonitorAddForm();
                createMonitorsGrid();

                kendo.bind($("#content"), this.options.viewModel);

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

                    ddlNewMonitorType = $("#ddlNewMonitorType").kendoDropDownList({
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: {
                                    url: function () { return document.location.origin + "/api/monitors/monitortype/list" },
                                }
                            }
                        }),
                        dataValueField: "Id",
                        dataTextField: "Name",
                        optionLabel: "Выберите тип графика..."
                    }).data("kendoDropDownList");
                }
                function createMonitorsGrid() {
                    gridMonitors = $("#gridMonitors").kendoGrid({
                        height: 500,
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: {
                                    url: function () { return document.location.origin + "/api/monitors/list" },
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
                            { field: "TypeName", title: "Тип", editor: getEditor },
                            {
                                title: "&nbsp;", width: 220, reorderable: false, sortable: false, editor: getEditor, attributes: { "class": "text-center" },
                                command: [
                                    {
                                        text: "Редактировать",
                                        click: function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();

                                            var item = this.dataItem($(e.currentTarget).closest("tr"));
                                            me.trigger('monitor:edit', item.Id);
                                        }
                                    },
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
                            .blur(function () {
                                var newValue = editor.val();
                                if (newValue != oldValue)
                                    me.trigger('monitor:setName', options.model.Id, newValue);
                            });
                    }
                    else
                        grid.closeCell();
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