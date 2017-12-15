
define(
	['common', 'lib', 'text!webapp/zones/settings.html'],
    function (common, lib, templates) {
        var gridZones;

        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            events: {
                'click .js-btn-add-zone': 'addZone'
            },
            addZone: function (e) {
                e.preventDefault();
                this.trigger('zone:add', $("#tbNewZoneName").val());
            },
            refreshZonesGrid: function () {
                gridZones.dataSource.read();
            },

            onShow: function () {
                var me = this;

                createZonesGrid();

                kendo.bind($("#content"), this.options.viewModel);

                function createZonesGrid() {
                    gridZones = $("#gridZones").kendoGrid({
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: {
                                    url: function () { return document.location.origin + "/api/zones/list" },
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
                            {
                                title: "&nbsp;", width: 220, reorderable: false, sortable: false, editor: getEditor, attributes: { "class": "text-center" },
                                command: [
                                    {
                                        text: "Редактировать",
                                        click: function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();

                                            var item = this.dataItem($(e.currentTarget).closest("tr"));
                                            me.trigger('zone:edit', item.Id);
                                        }
                                    },
                                    {
                                        text: "Удалить",
                                        click: function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();

                                            var item = this.dataItem($(e.currentTarget).closest("tr"));
                                            if (common.utils.confirm('Удалить зону "{0}"?', item.Name))
                                                me.trigger('zone:delete', item.Id);
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
                                    me.trigger('zone:setName', options.model.Id, newValue);
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