
define(
	['common', 'lib', 'text!webapp/speech/settings.html'],
    function (common, lib, templates) {
        var ddlNewVoiceCommandScript;
        var gridVoiceCommands;

        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            events: {
                'click .js-btn-add-monitor': 'addVoiceCommand',
            },
            addVoiceCommand: function (e) {
                e.preventDefault();
                this.trigger('voiceCommand:add', $("#tbNewVoiceCommandText").val(), ddlNewVoiceCommandScript.value());
            },
            refreshGrid: function () {
                gridVoiceCommands.dataSource.read();
            },

            onShow: function () {
                var me = this;

                createAddForm();
                createGrid();

                kendo.bind($("#content"), this.options.viewModel);

                function createAddForm() {
                    ddlNewVoiceCommandScript = $("#ddlNewVoiceCommandScript").kendoDropDownList({
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: {
                                    url: function () { return document.location.origin + "/api/scripts/list" },
                                }
                            }
                        }),
                        dataValueField: "id",
                        dataTextField: "name",
                        optionLabel: "Выберите скрипт..."
                    }).data("kendoDropDownList");
                }
                function createGrid() {
                    gridVoiceCommands = $("#gridVoiceCommands").kendoGrid({
                        height: 500,
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: {
                                    url: function () { return document.location.origin + "/api/speech/voicecommand/list" },
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
                            { field: "CommandText", title: "Текст", editor: getEditor },
                            { field: "ScriptName", title: "Скрипт", editor: getEditor },
                            {
                                title: "&nbsp;", width: 100, reorderable: false, sortable: false, editor: getEditor, attributes: { "class": "text-center" },
                                command: [
                                    {
                                        text: "Удалить",
                                        click: function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();

                                            var item = this.dataItem($(e.currentTarget).closest("tr"));
                                            if (common.utils.confirm('Удалить команду "{0}"?', item.CommandText))
                                                me.trigger('voiceCommand:delete', item.Id);
                                        }
                                    }
                                ]
                            }
                        ]
                    }).data("kendoGrid");
                }
                function getEditor(container, options) {
                    var grid = container.closest(".k-grid").data("kendoGrid");

                    if (options.field == "CommandText") {
                        var oldValue = options.model[options.field];

                        var editor = $("<input type='text' class='k-textbox' style='width:100%;'/>");
                        editor.appendTo(container)
                            .show().focus()
                            .unbind("keydown").keydown(preventEnter)
                            .val(oldValue)
                            .blur(function () {
                                var newValue = editor.val();
                                if (newValue != oldValue)
                                    me.trigger('voiceCommand:setText', options.model.Id, newValue);
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