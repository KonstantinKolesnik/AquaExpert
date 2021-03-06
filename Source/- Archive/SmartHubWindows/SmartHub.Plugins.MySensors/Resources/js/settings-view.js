﻿
define(
	['common', 'lib', 'webapp/monitors/utils', 'text!webapp/mysensors/settings.html'],
    function (common, lib, monitorUtils, templates) {
        var ctrlNodesGrid;
        var ctrlSensorsGrid;
        var ctrlDateTo;

        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            events: {
                'click .js-btn-clear': 'deleteValues'
            },
            deleteValues: function (e) {
                e.preventDefault();
                this.trigger('values:delete', ctrlDateTo.value());
            },

            onShow: function () {
                var me = this;
                var viewModel = me.options.viewModel;

                //initKendoCustomGrid();

                createTabStrip($("#tabstrip"));
                ctrlNodesGrid = createNodesGrid($("#gridNodes"), null, { field: "NodeNo", dir: "asc" });
                ctrlSensorsGrid = createSensorsGrid($("#gridSensors"), null, [{ field: "NodeNo", dir: "asc" }, { field: "SensorNo", dir: "asc" }]);
                createUnitSystemSelector();
                createDatePicker();

                //$(window).bind("resize", adjustSizes);
                //$(window).resize(adjustSizes);
                //adjustSizes();

                kendo.bind($("#content"), viewModel);

                function initKendoCustomGrid() {
                    // add "beforeEdit" event:
                    lib.kendo.ui.Grid.fn.editCell = (function (editCell) {
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
                    })(lib.kendo.ui.Grid.fn.editCell);
                }

                function createTabStrip(selector) {
                    selector.kendoTabStrip({
                        animation: {
                            open: { effects: "fadeIn" }
                        },
                        //activate: function () {
                        //    if (selector = $("#tabstrip"))
                        //        adjustSizes();
                        //}
                    });
                }
                function createNodesGrid(selector, filter, sort) {
                    return selector.kendoGrid({
                        dataSource: { filter: filter, sort: sort },
                        groupable: true,
                        sortable: {
                            mode: "multiple"
                        },
                        resizable: true,
                        editable: true,
                        pageable: {
                            pageSizes: [10, 20, 50, 100, 300],
                            pageSize: 20
                        },
                        columns: createNodeGridColumns(),
                        detailTemplate: lib.kendo.template($("#nodeDetailsTemplate").html()),
                        detailInit: function (e) {
                            createTabStrip(e.detailRow.find(".nodeDetailsTabStrip"));
                            createSensorsGrid(e.detailRow.find(".nodeDetailsSensors"),
                                { field: "NodeNo", operator: "eq", value: e.data.NodeNo },
                                [{ field: "NodeNo", dir: "asc" }, { field: "SensorNo", dir: "asc" }]
                            );
                            createBatteryLevelsChart(e.detailRow.find(".nodeDetailsBatteryLevels"),
                                { field: "NodeNo", operator: "eq", value: e.data.NodeNo }
                            );

                            lib.kendo.bind(e.detailRow, e.data);
                        }
                    }).data("kendoGrid");

                    function createNodeGridColumns() {
                        return [
                            { field: "NodeNo", title: "ID", width: 40, groupable: false, editor: getNodeEditor, attributes: { "class": "text-right" } },
                            { field: "Name", title: "Имя", groupable: false, editor: getNodeEditor },
                            { field: "TypeName", title: "Тип", width: 95, editor: getNodeEditor },
                            {
                                title: "Прошивка",
                                columns: [
                                    { field: "SketchName", title: "Имя", width: 140, editor: getNodeEditor },
                                    { field: "SketchVersion", title: "Версия", width: 60, editor: getNodeEditor, attributes: { "class": "text-center" } }
                                ]
                            },
                            {
                                title: "Батарея",
                                columns: [
                                    { field: "BatteryLevelLevel", title: "Уровень, %", width: 80, sortable: false, groupable: false, editor: getNodeEditor, attributes: { "class": "text-right" }, template: lib.kendo.template($("#batteryLevelLevelCellTemplate").html()) },
                                    { field: "BatteryLevelTimeStamp", title: "Дата", width: 120, groupable: false, editor: getNodeEditor, attributes: { "class": "text-center" }, template: lib.kendo.template($("#batteryLevelTimeStampCellTemplate").html()) }
                                ]
                            },
                            { field: "ProtocolVersion", title: "Версия протокола", width: 120, editor: getNodeEditor, attributes: { "class": "text-center" } },
                            {
                                title: "&nbsp;", width: 80, reorderable: false, sortable: false, editor: getNodeEditor, attributes: { "class": "text-center" },
                                command: [
                                    {
                                        text: "Удалить",
                                        click: function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();

                                            var item = this.dataItem($(e.currentTarget).closest("tr"));
                                            //var item = $("#gridNodes").data("kendoGrid").dataItem($(e.target).closest("tr"));

                                            if (common.utils.confirm('Удалить элемент "{0}"?', item.Name))
                                                me.trigger('node:delete', item.Id);
                                        }
                                    }
                                ]
                            }
                        ];
                    }
                }
                function createSensorsGrid(selector, filter, sort) {
                    return selector.kendoGrid({
                        dataSource: { filter: filter, sort: sort },
                        groupable: true,
                        sortable: {
                            mode: "multiple"
                        },
                        resizable: true,
                        editable: true,
                        pageable: {
                            pageSizes: [10, 20, 50, 100, 300],
                            pageSize: 20
                        },
                        columns: createSensorsGridColumns(),
                        detailTemplate: lib.kendo.template($("#sensorDetailsTemplate").html()),
                        detailInit: function (e) {
                            createSensorValuesChart();
                            lib.kendo.bind(e.detailRow, e.data);

                            function createSensorValuesChart() {
                                viewModel.getSensorMonitor(e.data.Id, function (data) {
                                    if (data)
                                        setTimeout(function () { monitorUtils.createMonitorWidget(e.detailRow.find(".sensorDetailsValues"), data); }, 0);
                                });


                                return;








                                e.detailRow.find(".sensorDetailsOptions").bind("change", function () {
                                    var chart = e.detailRow.find(".sensorDetailsValues").data("kendoChart"),
                                        series = chart.options.series,
                                        categoryAxis = chart.options.categoryAxis,
                                        baseUnitInputs = e.detailRow.find(".sensorDetailsOptions input:radio[name=baseUnit]"),
                                        aggregateInputs = e.detailRow.find(".sensorDetailsOptions input:radio[name=aggregate]");

                                    for (var i = 0, length = series.length; i < length; i++)
                                        series[i].aggregate = aggregateInputs.filter(":checked").val();

                                    categoryAxis.baseUnit = baseUnitInputs.filter(":checked").val();

                                    chart.refresh();
                                });

                                e.detailRow.find(".sensorDetailsValues").kendoChart({
                                    dataSource: {
                                        filter: [
                                            { field: "NodeNo", operator: "eq", value: e.data.NodeNo },
                                            { field: "SensorNo", operator: "eq", value: e.data.SensorNo }
                                        ]
                                    },
                                    series: [{
                                        line: {
                                            style: "smooth"
                                        },
                                        type: "area",
                                        //aggregate: "avg",
                                        field: "Value",
                                        categoryField: "TimeStamp"
                                    }],

                                    //tooltip: {
                                    //    visible: true,
                                    //    //format: "{0}qqqqq",
                                    //    //template: "#= lib.kendo.toString(category, 'dd.MM.yyyy') #: #= value #"
                                    //    template: "#= value #"
                                    //},


                                    ////style: "step",//"smooth",
                                    //title: { text: e.data.TypeName + " статистика" },
                                    //legend: { visible: true, position: "bottom" },
                                    //series: [
                                    //    {
                                    //        categoryField: "TimeStamp",
                                    //        field: "Value",
                                    //        type: "area",
                                    //        labels: {
                                    //            //format: "{0}" + me.getSensorValueUnit(e.data),
                                    //            visible: true
                                    //            //background: "transparent"
                                    //        },
                                    //        line: {
                                    //            color: "cornflowerblue",
                                    //            //opacity: 0.5,
                                    //            width: 0.5,
                                    //            style: "smooth" // "step", ""
                                    //        }
                                    //    }
                                    //],
                                    //valueAxis: {
                                    //    labels: {
                                    //        //format: "{0}" + me.getSensorValueUnit(e.data),
                                    //        visible: true
                                    //    },
                                    //    line: { visible: true },
                                    //    majorGridLines: { visible: true },
                                    //    //title: {
                                    //    //    text: "wwwww",
                                    //    //    background: "green",
                                    //    //    border: {
                                    //    //        width: 1,
                                    //    //    }
                                    //    //}
                                    //    //min: 0,
                                    //    //max: 120
                                    //},
                                    categoryAxis: {
                                        type: "date",

                                        baseUnit: "fit",
                                        //baseUnit: "seconds",
                                        //baseUnit: "minutes",
                                        //baseUnit: "hours",
                                        //baseUnit: "days",
                                        //baseUnit: "weeks",
                                        //baseUnit: "months",
                                        //baseUnit: "years",

                                        labels: {
                                            dateFormats: {
                                                hours: "MMM d HH:mm",
                                                //hours: "HH:mm",
                                                days: "MMM d",
                                                weeks: "MMM d",
                                                months: "yyyy MMM",
                                                years: "yyyy"
                                            },
                                            visible: true,
                                            rotation: 270,
                                            step: 3
                                            //min: 5//new Date()
                                        },
                                        line: { visible: true },
                                        majorGridLines: { visible: true }
                                    }
                                });
                            }
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
                        }
                    }).data("kendoGrid");

                    function createSensorsGridColumns() {
                        return [
                            { field: "NodeNo", title: "ID узла", width: 65, editor: getSensorEditor, attributes: { "class": "text-right" } },
                            { field: "SensorNo", title: "ID", width: 40, groupable: false, editor: getSensorEditor, attributes: { "class": "text-right" } },
                            { field: "Name", title: "Имя", groupable: false, editor: getSensorEditor },
                            { field: "TypeName", title: "Тип", width: 95, editor: getSensorEditor },
                            {
                                title: "Состояние",
                                columns: [
                                    { field: "SensorValueValue", title: "Значение", width: 80, sortable: false, groupable: false, editor: getSensorEditor, attributes: { "class": "text-right" }, template: lib.kendo.template($("#sensorValueValueCellTemplate").html()) },
                                    { field: "SensorValueTimeStamp", title: "Дата", width: 150, groupable: false, editor: getSensorEditor, attributes: { "class": "text-center" }, template: lib.kendo.template($("#sensorValueTimeStampCellTemplate").html()) }
                                ]
                            },
                            { field: "ProtocolVersion", title: "Версия протокола", width: 120, editor: getSensorEditor, attributes: { "class": "text-center" } },
                            {
                                title: "&nbsp;", width: 80, reorderable: false, sortable: false, editor: getSensorEditor, attributes: { "class": "text-center" },
                                command: [
                                    {
                                        text: "Удалить",
                                        click: function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();

                                            var item = this.dataItem($(e.currentTarget).closest("tr"));
                                            //var item = $("#gridSensors").data("kendoGrid").dataItem($(e.target).closest("tr"));

                                            if (common.utils.confirm('Удалить элемент "{0}"?', item.Name))
                                                me.trigger('sensor:delete', item.Id);
                                        }
                                    }
                                ]
                            }
                        ];
                    }
                }
                function createUnitSystemSelector() {
                    $("#ddlUnitSystem").kendoDropDownList({
                        dataSource: [
                            { value: "M", text: "Метрическая" },
                            { value: "I", text: "Эмпирическая" }
                        ],
                        dataValueField: "value",
                        dataTextField: "text",
                        valuePrimitive: true,
                        change: function (e) {
                            me.trigger('unitSystem:set', e.sender.value());
                        }
                    });
                }
                function createDatePicker() {
                    ctrlDateTo = $("#dpTo").kendoDatePicker({
                        format: "dd MMM yyyy",
                        value: new Date()
                    }).data("kendoDatePicker");
                }
                function createBatteryLevelsChart(selector, filter, sort) {
                    selector.kendoChart({
                        dataSource: { filter: filter, sort: sort },
                        series: [
                            {
                                field: "Level",
                                axis: "axisValue",
                                type: "area",//"line",
                                style: "smooth",
                                color: "red",
                                tooltip: {
                                    visible: true,
                                    template: "#= data.value #%"
                                }
                            }
                        ],
                        valueAxis: {
                            name: "axisValue",
                            labels: {
                                font: "10px Segoe UI",
                                template: "#= kendo.toString(data.value, 'n0') #"
                            },
                            min: 0,
                            max: 100,
                            majorUnit: 20
                        },
                        categoryAxis: {
                            field: "TimeStamp",

                            //type: "date",
                            type: "category",

                            baseUnit: "fit",
                            //baseUnit: "seconds",
                            //baseUnit: "minutes",
                            //baseUnit: "hours",
                            //baseUnit: "days",
                            //baseUnit: "weeks",
                            //baseUnit: "months",
                            //baseUnit: "years",

                            labels: {
                                rotation: 315,
                                font: "9px Segoe UI",
                                dateFormats: {
                                    minutes: "mm:ss",
                                    hours: "d.MM HH:mm",
                                    days: "MMM d",
                                    weeks: "MMM d",
                                    months: "yyyy MMM",
                                    years: "yyyy"
                                },
                                template: "#: kendo.toString(new Date(value), 'HH:mm') #" //"#: kendo.toString(new Date(value), 'd.MM - HH:mm') #"
                            },

                            line: { visible: true },
                            majorGridLines: { visible: true }
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
                        var oldValue = options.model[options.field];

                        var editor = $("<input type='text' class='k-textbox' style='width:100%;'/>");
                        editor.appendTo(container)
                            .show().focus()
                            .unbind("keydown").keydown(preventEnter)
                            .val(oldValue)
                            .blur(function () {
                                var newValue = editor.val();
                                if (newValue != oldValue) {
                                    if (isNodes)
                                        me.trigger('node:setName', options.model.Id, newValue);
                                    else
                                        me.trigger('sensor:setName', options.model.Id, newValue);
                                }
                            });
                    }
                    else
                        grid.closeCell();

                    function preventEnter(e) {
                        if (e.keyCode == 13) {
                            e.preventDefault();
                            e.stopPropagation();
                            $(e.target).blur(); //run saving
                        }
                    }
                }

                function adjustSizes() {
                    var bottomOffset = 25;

                    adjustGrid($("#gridNodes"));
                    adjustGrid($("#gridSensors"));
                    $("#pnlSettings").height($(window).height() - getY($("#pnlSettings")) - bottomOffset);

                    function adjustGrid(grid) {
                        grid.height($(window).height() - getY(grid) - bottomOffset);
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
                        if (!el)
                            return 0;

                        var yPosition = el.offsetTop;
                        while (el = el.offsetParent)
                            yPosition += el.offsetTop;
                        return yPosition;
                    }
                }
            },
            onDestroy: function () {
                ctrlNodesGrid.destroy();
                ctrlSensorsGrid.destroy();
            }
        });

	    return {
	        LayoutView: layoutView
	    };
	});