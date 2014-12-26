
define(
	['app', 'marionette', 'backbone', 'underscore', 'jquery', 'webapp/mysensors/views'],
	function (application, marionette, backbone, _, $, views) {
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
	        setNodeName: function (id, name, onComplete) {
	            $.getJSON('/api/mysensors/nodes/setname', { Id: id, Name: name })
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function (data) {
	                    onError(data);
	                });
	        },
	        setSensorName: function (id, name, onComplete) {
	            $.getJSON('/api/mysensors/sensors/setname', { Id: id, Name: name })
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function (data) {
	                    onError(data);
	                });
	        },
	        deleteNode: function (id, onComplete) {
	            $.getJSON('/api/mysensors/nodes/delete', { Id: id })
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function (data) {
	                    onError(data);
	                });
	        },
	        deleteSensor: function (id, onComplete) {
	            $.getJSON('/api/mysensors/sensors/delete', { Id: id })
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
	        Nodes: [],
	        Sensors: [],

	        update: function() {
	            api.getNodes(function (data) {
                    viewModel.set("Nodes", data);
                    api.getSensors(function (data) {
                        viewModel.set("Sensors", data);
                    });
                });
	        }
	    });
	    //viewModel.bind("change", onViewModelAfterSet);

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
                            command: [
                                {
                                    text: "Удалить",
                                    click: function (e) {
                                        var item = $("#gridNodes").data("kendoGrid").dataItem($(e.target).closest("tr"));
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
	                            filter: { field: "NodeNo", operator: "eq", value: e.data.NodeNo	}
	                        },
	                        groupable: true,
	                        sortable: true,
	                        reorderable: true,
	                        resizable: true,
	                        scrollable: true,
	                        selectable: false,
	                        navigatable: false,
	                        editable: true,
	                        columns: [
                                { field: "SensorNo", title: "ID", width: 35, groupable: false, editor: getSensorEditor, attributes: { "class": "text-right" } },
                                { field: "Name", title: "Имя", groupable: false, editor: getSensorEditor },
                                { field: "TypeName", title: "Тип", width: 95, editor: getSensorEditor },
                                { field: "SensorValue.Value", title: "Состояние", width: 80, groupable: false, editor: getSensorEditor, attributes: { "class": "text-right" }, template: kendo.template($("#sensorValueCellTemplate").html()) }
                                //{ field: "ProtocolVersion", title: "Версия протокола", width: 120, editor: getSensorEditor, attributes: { "class": "text-center" } }
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
                                        var item = $("#gridSensors").data("kendoGrid").dataItem($(e.target).closest("tr"));
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
	            //editor.attr("name", options.field); // bind to model

	            var oldValue = options.model[options.field];

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
	                    api.setNodeName(options.model.Id, newValue, onComplete);
	                else
	                    api.setSensorName(options.model.Id, newValue, onComplete);
                }

	            function onComplete() {
	                options.model[options.field] = newValue;
	                grid.refresh();
	            }
	        }
	        function preventEnter(e) {
	            if (e.keyCode == 13 /*$.ui.keyCode.ENTER*/) {
	                e.preventDefault();
	                e.stopPropagation();
	                $(e.target).blur(); //run saving
	            }
	        }
	    }

	    function onShowLayout() {
	        initKendoCustomGrid();
	        createNodesGrid();
	        createSensorsGrid();

            kendo.bind($("#content"), viewModel);
	    }
	    function onError(data) {
	        alert(data.responseJSON.ExceptionMessage);
	    }
	    function onViewModelAfterSet(e) {
	        switch (e.field) {
	            //case "Settings.WebTheme":
	            //    mainView.applyTheme();
	            //    if (!msgManager.IsFromServer)
	            //        msgManager.SetSettings(viewModel.Settings.WebTheme, viewModel.Settings.UnitSystem);
	            //    break;
	            //case "Settings.UnitSystem":
	            //    if (!msgManager.IsFromServer)
	            //        msgManager.SetSettings(viewModel.Settings.WebTheme, viewModel.Settings.UnitSystem);
	            //    break;
	            //case "Modules":
	            //    if (e.action == "itemchange")
	            //        msgManager.SetModule(e.items[0]);
	            //    $("#gridModules").data("kendoGrid").dataSource.sort({ field: "Name", dir: "asc" });
	            //    break;
	            case "Nodes":
	                if (e.action == "itemchange") {
	                    var model = e.items[0];
	                    api.setNodeName(model.Id, model.Name, function () {
	                        debugger;

	                    })
	                }
	                break;
	            default:
	                break;
	        }
	    }
	    function onSignal(data) {
	        switch (data.MsgId)
	        {
	            case "NodeDeleted": onNodeDeleted(data); break;
	            case "SensorDeleted": onSensorDeleted(data); break;
	                
	            default:
	                break;
	        }
	    }
	    function onNodeDeleted(data) {
	        var nodeId = data.Id;
	        var nodeNo = null;
	        for (var i = 0; i < viewModel.Nodes.length; i++) {
	            if (viewModel.Nodes[i].Id == nodeId) {
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
	    function onSensorDeleted(data) {
	        var sensorId = data.Id;

	        for (var i = 0; i < viewModel.Sensors.length; i++) {
	            if (viewModel.Sensors[i].Id == sensorId) {
	                viewModel.Sensors.splice(i, 1);
	                break;
	            }
	        }
	    }




	    return {
	        start: function () {
                application.SignalRReceiveHandlers.push(onSignal);

                var layout = new views.layoutView();
                layout.onShow = onShowLayout;
                application.setContentView(layout);

	            viewModel.update();

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