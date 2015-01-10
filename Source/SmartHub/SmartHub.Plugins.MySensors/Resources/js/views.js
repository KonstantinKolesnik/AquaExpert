
define(
	['app', 'marionette', 'backbone', 'underscore', 'kendo', 'text!webapp/mysensors/templates.html'],
    function (application, marionette, backbone, _, kendo, templates) {
	    //var mLayout = marionette.LayoutView.extend({
	    //    //template: _.template(tmplLayout),
	    //    template: _.template('<h1>MySensors network</h1><div id="region-list"></div>' +
        //        '<select id="size">' + //data-role="kendodropdownlist"
        //        '<option>S - 6 3/4"</option>'+
        //        '<option>M - 7 1/4"</option>'+
        //        '<option>L - 7 1/8"</option>'+
        //        '<option>XL - 7 5/8"</option>' +
        //    '</select>'
        //        ),
	    //    regions: {
	    //        //filter: '#region-filter',
	    //        list: '#region-list'
	    //    }
	    //});


	    //var mNodeView = marionette.ItemView.extend({
	    //    //template: _.template(tmplListItem),
	    //    //template: _.template('[# <%= NodeNo %>] [Type: <%= Type %>] [Sketch name: <%= SketchName %>] [Sketch version: <%= SketchVersion %>] [Protocol version: <%= ProtocolVersion %>]')
	    //    template: _.template(templates)
	    //});
	    //var mNodesView = marionette.CompositeView.extend({
	    //    //template: _.template(tmplList),
	    //    template: _.template('<h3>Узлы</h3><div id="lstNodes"></div>'),
	    //    childView: mNodeView,
	    //    childViewContainer: '#lstNodes',
	    //    onShow: function () {
	    //    }
	    //});



        var ctrlNodesGrid;

        function initKendoCustomGrid() {
            // add "beforeEdit" event:
            kendo.ui.Grid.fn.editCell = (function (editCell) {
                return function (cell) {
                    cell = $(cell);

                    var that = this,
                        column = that.columns[that.cellIndex(cell)],
                        model = that._modelForContainer(cell),
                        event = {
                            container: cell,
                            model: model,
                            preventDefault: function () {
                                this.isDefaultPrevented = true;
                            }
                        };

                    if (model && typeof this.options.beforeEdit === "function") {
                        this.options.beforeEdit.call(this, event);

                        // don't edit if prevented in beforeEdit
                        if (event.isDefaultPrevented)
                            return;
                    }

                    editCell.call(this, cell);
                };
            })(kendo.ui.Grid.fn.editCell);
        }
        function createNodesGrid() {
            //me.gridNodesStateManager = new GridStateManager("gridNodes");

            ctrlNodesGrid = $("#gridNodes").kendoGrid({
                //height: 400,
                groupable: true,
                sortable: true,
                reorderable: true,
                resizable: true,
                scrollable: true,
                selectable: false,
                navigatable: false,
                editable: true,
                pageable: {
                    pageSizes: [10, 20, 50, 100, 300],
                    pageSize: 20
                },
                columns:
                    [
                        { field: "NodeNo", title: "ID", width: 35, groupable: false, editor: getNodeEditor, attributes: { "class": "text-right" } },
                        { field: "Name", title: "Имя", groupable: false, editor: getNodeEditor },
                        { field: "TypeName", title: "Тип", width: 95, editor: getNodeEditor },
                        {
                            title: "Прошивка",
                            columns:
                                [
                                    { field: "SketchName", title: "Имя", width: 120, editor: getNodeEditor },
                                    { field: "SketchVersion", title: "Версия", width: 60, editor: getNodeEditor, attributes: { "class": "text-center" } }
                                ]
                        },
                        { field: "BatteryLevel.Level", title: "Батарея, %", width: 80, editor: getNodeEditor, attributes: { "class": "text-center" }, template: kendo.template($("#batteryLevelCellTemplate").html()) },
                        { field: "ProtocolVersion", title: "Версия протокола", width: 120, editor: getNodeEditor, attributes: { "class": "text-center" } },
                        {
                            title: "&nbsp;", width: 80, reorderable: false, filterable: false, sortable: false, editor: getNodeEditor, attributes: { "class": "text-center" },
                            command: ["delete"]
                            //command: [
                            //    {
                            //        text: "Удалить",
                            //        click: function (e) {
                            //            e.preventDefault();
                            //            e.stopPropagation();
                            //            var item = this.dataItem($(e.currentTarget).closest("tr"));
                            //            //var item = $("#gridNodes").data("kendoGrid").dataItem($(e.target).closest("tr"));
                            //            api.deleteNode(item.Id);
                            //        }
                            //    }
                            //]
                        }
                    ],
                detailTemplate: kendo.template($("#nodeDetailsTemplate").html()),
                detailInit: function (e) {
                    createTabStrip(e.detailRow.find(".nodeDetailsTabStrip"));
                    createSensorsGrid();
                    //createBatteryLevelsChart(e.detailRow.find(".deviceDetailsBatteryLevels"));
                    kendo.bind(e.detailRow, e.data);

                    //$(document).bind("kendo:skinChange", createChart);
                    //e.detailRow.find(".nodeDetailsBatteryLevels").data("kendoChart").setOptions({
                    //    categoryAxis: {
                    //        baseUnit: "hours"
                    //        //baseUnit: "days",
                    //        //baseUnit: "months",
                    //        //baseUnit: "weeks",
                    //        //baseUnit: "years",
                    //    }
                    //});

                    function createSensorsGrid() {
                        e.detailRow.find(".nodeDetailsSensors").kendoGrid({
                            dataSource: {
                                filter: { field: "NodeNo", operator: "eq", value: e.data.NodeNo }
                            },
                            groupable: true,
                            sortable: true,
                            reorderable: true,
                            resizable: true,
                            scrollable: true,
                            selectable: false,
                            navigatable: false,
                            editable: true,
                            pageable: {
                                pageSizes: [5, 10, 20, 50, 100, 300],
                                pageSize: 20
                            },
                            columns: [
                                { field: "SensorNo", title: "ID", width: 35, groupable: false, editor: getSensorEditor, attributes: { "class": "text-right" } },
                                { field: "Name", title: "Имя", groupable: false, editor: getSensorEditor },
                                { field: "TypeName", title: "Тип", width: 95, editor: getSensorEditor },
                                { field: "SensorValue.Value", title: "Состояние", width: 80, groupable: false, editor: getSensorEditor, attributes: { "class": "text-right" }, template: kendo.template($("#sensorValueCellTemplate").html()) },
                                //{ field: "ProtocolVersion", title: "Версия протокола", width: 120, editor: getSensorEditor, attributes: { "class": "text-center" } },
                                {
                                    title: "&nbsp;", width: 80, reorderable: false, filterable: false, sortable: false, editor: getSensorEditor, attributes: { "class": "text-center" },
                                    command: ["delete"]
                                    //command: [
                                    //    {
                                    //        text: "Удалить",
                                    //        click: function (e) {
                                    //            e.preventDefault();
                                    //            e.stopPropagation();
                                    //            var item = this.dataItem($(e.currentTarget).closest("tr"));
                                    //            //var item = $("#gridSensors").data("kendoGrid").dataItem($(e.target).closest("tr"));
                                    //            api.deleteSensor(item.Id);
                                    //        }
                                    //    }
                                    //]
                                }
                            ]
                        });
                    }
                },
                detailExpand: function (e) {
                    //me.gridNodesStateManager.onDetailExpand(e);
                },
                detailCollapse: function (e) {
                    //me.gridNodesStateManager.onDetailCollapse(e);
                },
                dataBound: function (e) {
                    //me.gridNodesStateManager.onDataBound();
                    //localStorage["kendo-grid-options"] = kendo.stringify(grid.getOptions());
                    //var options = localStorage["kendo-grid-options"];
                    //if (options) {
                    //    grid.setOptions(JSON.parse(options));
                    //}
                }
            }).data("kendoGrid");
        }
        function createSensorsGrid() {
            //me.gridSensorsStateManager = new GridStateManager("gridSensors");

            $("#gridSensors").kendoGrid({
                //height: 400,
                groupable: true,
                sortable: true,
                reorderable: true,
                resizable: true,
                scrollable: true,
                selectable: false,
                navigatable: false,
                editable: true,
                pageable: {
                    pageSizes: [50, 100, 500, 1000],
                    pageSize: 50
                },
                columns:
                    [
                        { field: "NodeNo", title: "ID узла", width: 55, editor: getSensorEditor, attributes: { "class": "text-right" } },
                        { field: "SensorNo", title: "ID", width: 35, groupable: false, editor: getSensorEditor, attributes: { "class": "text-right" } },
                        { field: "Name", title: "Имя", groupable: false, editor: getSensorEditor },
                        { field: "TypeName", title: "Тип", width: 95, editor: getSensorEditor },
                        { field: "SensorValue.Value", title: "Состояние", width: 80, groupable: false, editor: getSensorEditor, attributes: { "class": "text-right" }, template: kendo.template($("#sensorValueCellTemplate").html()) },
                        { field: "ProtocolVersion", title: "Версия протокола", width: 120, editor: getSensorEditor, attributes: { "class": "text-center" } },
                        {
                            title: "&nbsp;", width: 80, reorderable: false, filterable: false, sortable: false, editor: getSensorEditor, attributes: { "class": "text-center" },
                            command: [
                                {
                                    text: "Удалить",
                                    click: function (e) {
                                        e.preventDefault();
                                        e.stopPropagation();
                                        var item = this.dataItem($(e.currentTarget).closest("tr"));
                                        //var item = $("#gridSensors").data("kendoGrid").dataItem($(e.target).closest("tr"));
                                        api.deleteSensor(item.Id);
                                    }
                                }
                            ]
                        }
                    ],
                detailTemplate: kendo.template($("#sensorDetailsTemplate").html()),
                detailInit: function (e) {
                    //createSensorValuesChart(e.detailRow.find(".sensorDetailsValues"), e.data);
                    //kendo.bind(e.detailRow, e.data);
                },
                detailExpand: function (e) {
                    //me.gridSensorsStateManager.onDetailExpand(e);
                },
                detailCollapse: function (e) {
                    //me.gridSensorsStateManager.onDetailCollapse(e);
                },
                dataBinding: function (e) {
                    //if (e.action == "itemchange") {
                    //    e.preventDefault();

                    //    var item = e.items[0];

                    //    //get the current column names, in their current order
                    //    var grid = $("#gridSensors").data('kendoGrid');
                    //    var columns = grid.columns;
                    //    var columnNames = $.map(columns, function (column) {
                    //        return column.field;
                    //    });

                    //    //get the column tds for update
                    //    var masterRow = $('#gridSensors > div.k-grid-content > table > tbody > tr[data-uid="' + item.uid + '"]');
                    //    var tds = masterRow.find('td:not(.k-hierarchy-cell)');

                    //    //collapse the detail row that was saved.
                    //    grid.collapseRow(masterRow);

                    //    //update the tds with the value from the current item stored in items
                    //    for (var i = 0 ; i < tds.length ; i++) {
                    //        $(tds[i]).html(item[columnNames[i]]);
                    //    }
                    //}
                },
                dataBound: function (e) {
                    //me.gridSensorsStateManager.onDataBound();
                }
            });
        }
        function createUnitSystemSelector() {
            $("#ddlUnitSystem").kendoDropDownList({
                dataSource: [
                    { value: "M", text: "Метрическая" },
                    { value: "I", text: "Эмпирическая" }
                ],
                dataTextField: "text",
                dataValueField: "value"
            });
        }
        function showDialog(txt, title) {
            var win = $("#dlg").kendoWindow({
                actions: ["Close"],
                width: "400px",
                height: "200px",
                title: "Smart Hub",
                visible: false,
                draggable: true,
                resizable: false,
                modal: true
            }).data("kendoWindow");

            if (txt)
                win.content(txt);
            if (title)
                win.title(title);

            win.center().open();
        }
        function createTabStrip(selector) {
            selector.kendoTabStrip({
                animation: {
                    open: { effects: "fadeIn" }
                }
            });
        }
        function getNodeEditor(container, options) {
            return getEditor(container, options, true);
        }
        function getSensorEditor(container, options) {
            return getEditor(container, options, false);
        }
        function getEditor(container, options, isNodes) {
            var grid = container.closest(".k-grid").data("kendoGrid");

            if (options.field == "Name") {
                var editor = $("<input type='text' class='k-textbox' style='width:100%;'/>");
                editor.attr("name", options.field);

                //var oldValue = options.model[options.field];

                editor.appendTo(container)
                    .show().focus()
                    .unbind("keydown").keydown(preventEnter);
                    //.val(oldValue)
                    //.blur(save);
            }
            else
                grid.closeCell();

            function save(e) {
                //var newValue = editor.val();
                //if (newValue != oldValue) {
                //    if (isNodes)
                //        api.setNodeName(options.model.Id, newValue);
                //    else
                //        api.setSensorName(options.model.Id, newValue);
                //}
            }
            function preventEnter(e) {
                if (e.keyCode == 13 /*$.ui.keyCode.ENTER*/) {
                    e.preventDefault();
                    e.stopPropagation();
                    $(e.target).blur(); //run saving
                }
            }
        }
        function adjustSizes() {
            $("#content").height($(window).height() - $("#header").outerHeight() - $("#footer").outerHeight());

            adjustGrid($("#gridNodes"));
            adjustGrid($("#gridSensors"));
            //adjustGrid($("#gridModules"));

            $("#tabstrip").height($(window).height() - getY($("#tabstrip")) - 5);

            function adjustGrid(grid) {
                //grid.height($(window).height() - getY(grid) /*- $("#footer").outerHeight()*/ - 8/*don't change!*/);
                grid.height($(window).height() - getY(grid) /*- $("#footer").outerHeight()*/ - 15/*don't change!*/);
                arrangeGridContent(grid);

                function arrangeGridContent() {
                    var newHeight = grid.innerHeight(),
                        otherElements = grid.children().not(".k-grid-content"),
                        otherElementsHeight = 0;

                    otherElements.each(function () { otherElementsHeight += $(this).outerHeight(); });
                    grid.children(".k-grid-content").height(newHeight - otherElementsHeight);
                }
            }
            function getY(selector) {
                var el = selector[0];
                var yPosition = el.offsetTop;
                while (el = el.offsetParent)
                    yPosition += el.offsetTop;
                return yPosition;
            }
        }

        var layoutView = marionette.LayoutView.extend({
            template: _.template(templates),
            onShow: function () {
                initKendoCustomGrid();

                createTabStrip($("#tabstrip"));

                createNodesGrid();
                createSensorsGrid();
                createUnitSystemSelector();

                //$(window).bind("resize", adjustSizes);
                //$(window).resize(adjustSizes);

                //kendo.bind($("#content"), viewModel);

                //adjustSizes();
            }
        });

	    return {
	        layoutView: layoutView
	        //nodesView: mNodesView
	    };
	});