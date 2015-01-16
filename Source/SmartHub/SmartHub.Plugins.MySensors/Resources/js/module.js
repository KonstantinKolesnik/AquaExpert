
define(
	['app', 'common', 'marionette', 'backbone', 'underscore', 'jquery', /*'webapp/mysensors/views',*/ 'kendo', 'text!webapp/mysensors/templates.html'],
	function (application, commonModule, marionette, backbone, _, $, /*views,*/ kendo, templates) {
	    var api = {
	        getNodes: function (onComplete) {
	            $.getJSON('/api/mysensors/nodes')
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function (data) {
	                    onError(data);
	                });
	        },
	        setNodeName: function (id, name, onComplete) {
	            $.post('/api/mysensors/nodes/setname', { Id: id, Name: name })
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function (data) {
	                    onError(data);
	                });
	        },
	        deleteNode: function (id, onComplete) {
	            $.post('/api/mysensors/nodes/delete', { Id: id })
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function (data) {
	                    onError(data);
	                });
	        },

	        getSensors: function (onComplete) {
	            $.getJSON('/api/mysensors/sensors')
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function (data) {
	                    onError(data);
	                });
	        },
	        setSensorName: function (id, name, onComplete) {
	            $.post('/api/mysensors/sensors/setname', { Id: id, Name: name })
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function (data) {
	                    onError(data);
	                });
	        },
	        deleteSensor: function (id, onComplete) {
	            $.post('/api/mysensors/sensors/delete', { Id: id })
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function (data) {
	                    onError(data);
	                });
	        },

	        getUnitSystem: function (onComplete) {
	            $.getJSON('/api/mysensors/unitsystem')
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function (data) {
	                    onError(data);
	                });
	        },
	        setUnitSystem: function (value, onComplete) {
	            $.post('/api/mysensors/setunitsystem', { Value: value })
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function (data) {
	                    onError(data);
	                });
	        },

	        getBatteryLevels: function (onComplete) {
	            $.getJSON('/api/mysensors/batterylevels')
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function (data) {
	                    onError(data);
	                });
	        },
	    }

	    var viewModel = kendo.observable({
	        UnitSystem: "M",
	        SerialPortName: "",
	        Nodes: [],
	        Sensors: [],
            BatteryLevels: [],
	        update: function () {
	            api.getUnitSystem(function (data) {
	                viewModel.set("UnitSystem", data.Value);
	                api.getNodes(function (data) {
	                    viewModel.set("Nodes", data);
                        api.getSensors(function (data) {
                            viewModel.set("Sensors", data);
                        });
                    });
	            });
	        }
	    });

	    var layoutView = marionette.LayoutView.extend({
	        template: _.template(templates),
	        onShow: function () {
	            createTabStrip($("#tabstrip"));
	            createUnitSystemSelector();
	            createNodesGrid();
	            createSensorsGrid();

	            $(window).bind("resize", adjustSizes);
	            $(window).resize(adjustSizes);
	            adjustSizes();

	            var ctrlNodesGrid;

	            function createTabStrip(selector) {
	                selector.kendoTabStrip({
	                    animation: {
	                        open: { effects: "fadeIn" }
	                    },
	                    activate: function () {
	                        if (selector = $("#tabstrip"))
                                adjustSizes();
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
	                    dataValueField: "value",
	                    change: function (e) {
	                        api.setUnitSystem(e.sender.value());
	                    }
	                });
	            }
	            function createNodesGrid() {
	                //me.gridNodesStateManager = new GridStateManager("gridNodes");

	                ctrlNodesGrid = $("#gridNodes").kendoGrid({
	                    groupable: true,
	                    sortable: true,
	                    reorderable: true,
	                    resizable: true,
	                    editable: true,
	                    pageable: {
	                        pageSizes: [10, 20, 50, 100, 300],
	                        pageSize: 20
	                    },
	                    columns: [
                            { field: "NodeNo", title: "ID", width: 35, groupable: false, editor: getNodeEditor, attributes: { "class": "text-right" } },
                            { field: "Name", title: "Имя", groupable: false, editor: getNodeEditor },
                            { field: "TypeName", title: "Тип", width: 95, editor: getNodeEditor },
                            {
                                title: "Прошивка",
                                columns:
                                    [
                                        { field: "SketchName", title: "Имя", width: 140, editor: getNodeEditor },
                                        { field: "SketchVersion", title: "Версия", width: 60, editor: getNodeEditor, attributes: { "class": "text-center" } }
                                    ]
                            },
                            { field: "BatteryLevel.Level", title: "Батарея, %", width: 80, editor: getNodeEditor, attributes: { "class": "text-center" }, template: kendo.template($("#batteryLevelCellTemplate").html()) },
                            { field: "ProtocolVersion", title: "Версия протокола", width: 120, editor: getNodeEditor, attributes: { "class": "text-center" } },
                            {
                                title: "&nbsp;", width: 80, reorderable: false, filterable: false, sortable: false, editor: getNodeEditor, attributes: { "class": "text-center" },
                                command: [
                                    {
                                        text: "Удалить",
                                        click: function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();

                                            var item = this.dataItem($(e.currentTarget).closest("tr"));
                                            //var item = $("#gridNodes").data("kendoGrid").dataItem($(e.target).closest("tr"));

                                            if (commonModule.utils.confirm('Удалить элемент "{0}"?', item.Name))
                                                api.deleteNode(item.Id);
                                        }
                                    }
                                ]
                            }
                        ],
	                    detailTemplate: kendo.template($("#nodeDetailsTemplate").html()),
	                    detailInit: function (e) {
	                        createTabStrip(e.detailRow.find(".nodeDetailsTabStrip"));
	                        createSensorsGrid();
	                        createBatteryLevelsChart(e.detailRow.find(".nodeDetailsBatteryLevels"));
	                        kendo.bind(e.detailRow, e.data);

	                        api.getBatteryLevels(function (data) {
	                            viewModel.set("BatteryLevels", data);
	                        });


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
	                    dataBinding: function (e) {
	                        //if (ctrlNodesGrid)
	                        //    localStorage["kendo-grid-options"] = kendo.stringify(ctrlNodesGrid.getOptions());
	                    },
	                    dataBound: function (e) {
	                        //me.gridNodesStateManager.onDataBound();
	                        //var options = localStorage["kendo-grid-options"];
	                        //if (options && ctrlNodesGrid) {
	                        //    ctrlNodesGrid.setOptions(JSON.parse(options));
	                        //}
	                    }
	                }).data("kendoGrid");
	            }
	            function createSensorsGrid() {
	                //me.gridSensorsStateManager = new GridStateManager("gridSensors");

	                $("#gridSensors").kendoGrid({
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

                                                if (commonModule.utils.confirm('Удалить элемент "{0}"?', item.Name))
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
	            function showDialog(txt, title) {
	                var win = $("#dlg").kendoWindow({
	                    actions: ["Close"],
	                    width: "400px",
	                    //height: "200px",
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
	                    .blur(save);
	                }
	                else
	                    grid.closeCell();

	                function save(e) {
	                    var newValue = editor.val();
	                    if (newValue != oldValue) {
	                        if (isNodes)
	                            api.setNodeName(options.model.Id, newValue);
	                        else
	                            api.setSensorName(options.model.Id, newValue);
	                    }
	                }
	                function preventEnter(e) {
	                    if (e.keyCode == 13) {
	                        e.preventDefault();
	                        e.stopPropagation();
	                        $(e.target).blur(); //run saving
	                    }
	                }
	            }
	            function adjustSizes() {
	                var bottomOffset = 15;

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
	                    var yPosition = el.offsetTop;
	                    while (el = el.offsetParent)
	                        yPosition += el.offsetTop;
	                    return yPosition;
	                }
	            }
	            function createBatteryLevelsChart(selector) {
	                selector.kendoChart({
	                    //dataSource: {
	                    //    filter: { field: "NodeNo", operator: "eq", value: e.data.NodeNo }
	                    //},

	                    //theme: "blueOpal",
	                    transitions: true,
	                    style: "smooth",
	                    //title: { text: "Internet Users in United States" },
	                    legend: { visible: true, position: "bottom" },
	                    //seriesDefaults: {
	                    //    type: "line",
	                    //    labels: {
	                    //        visible: true,
	                    //        format: "{0}%",
	                    //        background: "transparent"
	                    //    }
	                    //},
	                    series: [
                            {
                                //name: "Levels",
                                categoryField: "Time",
                                field: "Percent",
                                //axis: "levels",
                                type: "area",//"line",
                                labels: {
                                    visible: true,
                                    format: "{0}%",
                                    background: "transparent"
                                }
                            }
	                    ],
	                    valueAxis: {
	                        //name: "levels",
	                        labels: { format: "{0}%", visible: true },
	                        line: { visible: true },
	                        majorGridLines: { visible: true },
	                        min: 0,
	                        max: 120,
	                        color: "#000000"
	                    },
	                    categoryAxis: {
	                        //field: "Time",
	                        // or
	                        //categories: [2005, 2006, 2007, 2008, 2009],

	                        //name: "levels",

	                        //axisCrossingValue: [0, 3],

	                        type: "date",

	                        baseUnit: "hours",
	                        //baseUnit: "days",
	                        //baseUnit: "months",
	                        //baseUnit: "weeks",
	                        //baseUnit: "years",

	                        labels: {
	                            dateFormats: {
	                                hours: "HH:mm",
	                                days: "MMM, d",
	                                months: "MMM-yy",
	                                weeks: "M-d",
	                                years: "yyyy"
	                            },
	                            //format: "{0} aa}",
	                            visible: true
	                        },

	                        line: { visible: true },
	                        majorGridLines: { visible: true },
	                        color: "#000000"
	                    }
	                });
	            }
	            function createSensorValuesChart(selector, sensor) {
	                selector.kendoChart({
	                    //theme: "blueOpal",
	                    transitions: true,
	                    //style: "step",//"smooth",
	                    title: { text: sensor.TypeName() + " statistics" },
	                    legend: { visible: true, position: "bottom" },
	                    series: [
                            {
                                categoryField: "Time",
                                field: "Value",
                                type: "area",
                                labels: {
                                    format: "{0}" + me.getSensorValueUnit(sensor),
                                    visible: true
                                    //background: "transparent"
                                },
                                line: {
                                    color: "cornflowerblue",
                                    //opacity: 0.5,
                                    width: 0.5,
                                    style: "smooth" // "step", ""
                                }
                            }
	                    ],
	                    valueAxis: {
	                        labels: {
	                            format: "{0}" + me.getSensorValueUnit(sensor),
	                            visible: true
	                        },
	                        line: { visible: true },
	                        majorGridLines: { visible: true },
	                        //title: {
	                        //    text: "wwwww",
	                        //    background: "green",
	                        //    border: {
	                        //        width: 1,
	                        //    }
	                        //}
	                        //min: 0,
	                        //max: 120
	                    },
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
	        }
	    });

	    var signalRReceiveHandler = {
	        handler: function (data) {
	            switch (data.MsgId)
	            {
	                case "NodePresentation": onNodePresentation(data); break;
	                case "NodeNameChanged": onNodeNameChanged(data); break;
	                case "NodeDeleted": onNodeDeleted(data); break;
	                case "BatteryLevel": onBatteryLevel(data); break;
	                case "SensorPresentation": onSensorPresentation(data); break;
	                case "SensorNameChanged": onSensorNameChanged(data); break;
	                case "SensorDeleted": onSensorDeleted(data); break;
	                case "SensorValue": onSensorValue(data); break;
	                case "UnitSystemChanged": onUnitSystemChanged(data); break;
	                default: break;
	            }

	            function onNodePresentation(data) {
	                for (var i = 0; i < viewModel.Nodes.length; i++) {
	                    if (viewModel.Nodes[i].Id == data.Value.Id) {
	                        viewModel.Nodes[i].set("NodeNo", data.Value.NodeNo);
	                        viewModel.Nodes[i].set("TypeName", data.Value.TypeName);
	                        viewModel.Nodes[i].set("ProtocolVersion", data.Value.ProtocolVersion);
	                        viewModel.Nodes[i].set("SketchName", data.Value.SketchName);
	                        viewModel.Nodes[i].set("SketchVersion", data.Value.SketchVersion);
	                        viewModel.Nodes[i].set("Name", data.Value.Name);
	                        viewModel.Nodes[i].set("BatteryLevel", data.Value.BatteryLevel);
	                        return;
	                    }
	                }

	                viewModel.Nodes.push(data.Value);
	            }
	            function onNodeNameChanged(data) {
	                for (var i = 0; i < viewModel.Nodes.length; i++) {
	                    if (viewModel.Nodes[i].Id == data.Value.Id) {
	                        viewModel.Nodes[i].set("Name", data.Value.Name);
	                        break;
	                    }
	                }
	            }
	            function onNodeDeleted(data) {
	                var nodeNo = null;
	                for (var i = 0; i < viewModel.Nodes.length; i++) {
	                    if (viewModel.Nodes[i].Id == data.Value.Id) {
	                        nodeNo = viewModel.Nodes[i].NodeNo;
	                        viewModel.Nodes.splice(i, 1);
	                        break;
	                    }
	                }

	                if (nodeNo) {
	                    for (var i = 0; i < viewModel.Sensors.length;) {
	                        if (viewModel.Sensors[i].NodeNo == nodeNo)
	                            viewModel.Sensors.splice(i, 1);
	                        else
	                            i++;
	                    }
	                }
	            }
	            function onBatteryLevel(data) {
	                for (var i = 0; i < viewModel.Nodes.length; i++) {
	                    if (viewModel.Nodes[i].NodeNo == data.Value.NodeNo) {
	                        viewModel.Nodes[i].set("BatteryLevel", data.Value);
	                        break;
	                    }
	                }
	            }
	            function onSensorPresentation(data) {
	                for (var i = 0; i < viewModel.Sensors.length; i++) {
	                    if (viewModel.Sensors[i].Id == data.Value.Id) {
	                        viewModel.Sensors[i].set("NodeNo", data.Value.NodeNo);
	                        viewModel.Sensors[i].set("SensorNo", data.Value.SensorNo);
	                        viewModel.Sensors[i].set("TypeName", data.Value.TypeName);
	                        viewModel.Sensors[i].set("ProtocolVersion", data.Value.ProtocolVersion);
	                        viewModel.Sensors[i].set("Name", data.Value.Name);
	                        viewModel.Sensors[i].set("SensorValue", data.Value.SensorValue);
	                        return;
	                    }
	                }

	                viewModel.Sensors.push(data.Value);
	            }
	            function onSensorNameChanged(data) {
	                for (var i = 0; i < viewModel.Sensors.length; i++) {
	                    if (viewModel.Sensors[i].Id == data.Value.Id) {
	                        viewModel.Sensors[i].set("Name", data.Value.Name);
	                        break;
	                    }
	                }
	            }
	            function onSensorDeleted(data) {
	                for (var i = 0; i < viewModel.Sensors.length; i++) {
	                    if (viewModel.Sensors[i].Id == data.Value.Id) {
	                        viewModel.Sensors.splice(i, 1);
	                        break;
	                    }
	                }
	            }
	            function onSensorValue(data) {
	                console.log(data.Value.Type + ": " + data.Value.Value);
	                for (var i = 0; i < viewModel.Sensors.length; i++) {
	                    if (viewModel.Sensors[i].NodeNo == data.Value.NodeNo && viewModel.Sensors[i].SensorNo == data.Value.SensorNo) {
	                        viewModel.Sensors[i].set("SensorValue", data.Value);
	                        break;
	                    }
	                }
	            }
	            function onUnitSystemChanged(data) {
	                viewModel.set("UnitSystem", data.Value);
	            }
	        }
	    }

	    function onError(data) {
	        //alert(data.responseJSON.ExceptionMessage);
	        //alert(data.statusText);
	        alert(data.responseText);
	    }

	    return {
	        start: function () {
	            application.SignalRReceiveHandlers.push(signalRReceiveHandler.handler);

                //var layoutView = new views.layoutView();
	            //application.setContentView(layoutView);

	            application.setContentView(new layoutView());

	            viewModel.update();
                kendo.bind($("#content"), viewModel);

                //var view = new views.nodesView({ collection: nodes });
                //application.setContentView(view);

                //// создаем экземпляр layout view и добавляем его на страницу
                //var layoutView = new views.layoutView();
                //application.setContentView(layoutView);

                //// создаем экземпляры дочерних представлений
                ////var filterView = new myFilterView( ... );
                //var listView = new views.nodesView({ collection: nodes });

                //// отображаем дочерние представления на странице
                ////layoutView.filter.show(filterView);
                //layoutView.list.show(listView);
	        }
	    };
	});