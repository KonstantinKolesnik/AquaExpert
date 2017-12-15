
define(
	['common', 'lib', 'text!webapp/informers/settings.html'],
    function (common, lib, templates) {
        var ddlNewInformerSensor;
        var gridInformers;

        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            events: {
                'click .js-btn-add-informer': 'addInformer'
            },
            addInformer: function (e) {
                e.preventDefault();
                this.trigger('informer:add', $("#tbNewInformerName").val(), ddlNewInformerSensor.value());
            },
            refreshInformersGrid: function () {
                gridInformers.dataSource.read();
            },

            onShow: function () {
                var me = this;

                createInformerAddForm();
                createInformersGrid();

                kendo.bind($("#content"), this.options.viewModel);

                function createInformerAddForm() {
                    ddlNewInformerSensor = $("#ddlNewInformerSensor").kendoDropDownList({
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: {
                                    url: function () { return document.location.origin + "/api/mysensors/sensorsByType" },
                                    data: { type: 28 } // informer
                                }
                            }
                        }),
                        dataValueField: "Id",
                        dataTextField: "Name",
                        optionLabel: "Выберите сенсор..."
                    }).data("kendoDropDownList");
                }
                function createInformersGrid() {
                    gridInformers = $("#gridInformers").kendoGrid({
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: {
                                    url: function () { return document.location.origin + "/api/informers/list" },
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
                            { field: "SensorDisplayName", title: "Сенсор дисплея", editor: getEditor },
                            {
                                title: "&nbsp;", width: 220, reorderable: false, sortable: false, editor: getEditor, attributes: { "class": "text-center" },
                                command: [
                                    {
                                        text: "Редактировать",
                                        click: function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();

                                            var item = this.dataItem($(e.currentTarget).closest("tr"));
                                            me.trigger('informer:edit', item.Id);
                                        }
                                    },
                                    {
                                        text: "Удалить",
                                        click: function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();

                                            var item = this.dataItem($(e.currentTarget).closest("tr"));
                                            if (common.utils.confirm('Удалить информер "{0}"?', item.Name))
                                                me.trigger('informer:delete', item.Id);
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
                                    me.trigger('informer:setName', options.model.Id, newValue);
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