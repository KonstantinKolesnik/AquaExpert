
define(
	['app', 'marionette', 'backbone', 'underscore', 'jquery', 'webapp/mysensors/views'],
	function (application, marionette, backbone, _, $, views) {
	    var api = {
	        getNodes: function (onComplete, onError) {
	            $.getJSON('/api/mysensors/nodes')
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function () {
	                    if (onError)
	                        onError("Error");
	                });
	        },
	        getSensors: function (onComplete, onError) {
	            $.getJSON('/api/mysensors/sensors')
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function () {
	                    if (onError)
	                        onError("Error");
	                });
	        },
	        getLastBatteryLevel: function (nodeNo, onComplete, onError) {
	            $.getJSON('/api/mysensors/lastbatterylevel', { nodeNo: nodeNo })
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function () {
	                    if (onError)
	                        onError("Error");
	                });
	        },
	        deleteNode: function (nodeNo, onComplete, onError) {
	            $.getJSON('/api/mysensors/deletenode', { nodeNo: nodeNo })
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function () {
	                    if (onError)
	                        onError("Error");
	                });
	        },
	    }

	    var viewModel = kendo.observable({
	        Nodes: [],
	        Sensors: [],

	        update: function() {
	            api.getNodes(
                    function (data) {
                        $.each(data, function (idx, item) {
                            item.LastBatteryLevel = null;
                        })
                        viewModel.set("Nodes", data);

                        $.each(viewModel.Nodes, function (idx, item) {
                            api.getLastBatteryLevel(item.NodeNo, function (bl) {
                                //item.set("LastBatteryLevel", bl);
                            });
                        })





                        api.getSensors(function (data) {
                            viewModel.set("Sensors", data);
                        });
                    },
                    function () {
                        showDialog("Ошибка загрузки узлов.");
                    });
	        }
	    });

	    function createNodesGrid() {
	        //me.gridNodesStateManager = new GridStateManager("gridNodes");

	        $("#gridNodes").kendoGrid({
	            //height: 400,
	            groupable: true,
	            sortable: true,
	            reorderable: true,
	            resizable: true,
	            scrollable: true,
	            pageable: {
	                pageSizes: [10, 20, 50, 100, 300],
	                pageSize: 20
	            },
	            columns:
                    [
                        //{ field: "NodeNo", title: "ID", width: 40, groupable: false, attributes: { "class": "text-right" } },
                        //{ field: "Name", title: "Name", groupable: false },
                        //{ field: "TypeName", title: "Type", width: 100 },
                        //{
                        //    title: "Firmware",
                        //    columns:
                        //        [
                        //            { field: "SketchName", title: "Name", width: 120 },
                        //            { field: "SketchVersion", title: "Version", width: 70, attributes: { "class": "text-center" } }
                        //        ]
                        //},
                        ////{ field: "Sensors.length", title: "Sensors Count", width: 110, attributes: { "class": "text-center" } },
                        //{ field: "LastBatteryLevel.Level", title: "Battery, %", width: 90, attributes: { "class": "text-center" }, template: kendo.template($("#batteryLevelCellTemplate").html()) },
                        //{ field: "ProtocolVersion", title: "Protocol Version", width: 110, attributes: { "class": "text-center" } },
                        //{
                        //    title: "&nbsp;", width: 80, reorderable: false, filterable: false, sortable: false, attributes: { "class": "text-center" },
                        //    command: [
                        //        {
                        //            text: "Delete",
                        //            click: function (e) {
                        //                var item = $("#gridNodes").data("kendoGrid").dataItem($(e.target).closest("tr"));
                        //                api.deleteNode(item.NodeNo);
                        //                //msgManager.DeleteNode(item.ID);
                        //            }
                        //        }
                        //    ]
                        //}


                        { field: "NodeNo", title: "ID", groupable: false, attributes: { "class": "text-right" } },
                        { field: "Name", title: "Name", groupable: false },
                        { field: "TypeName", title: "Type" },
                        {
                            title: "Firmware",
                            columns:
                                [
                                    { field: "SketchName", title: "Name" },
                                    { field: "SketchVersion", title: "Version", attributes: { "class": "text-center" } }
                                ]
                        },
                        { field: "LastBatteryLevel.Level", title: "Battery, %", attributes: { "class": "text-center" }, template: kendo.template($("#batteryLevelCellTemplate").html()) },
                        { field: "ProtocolVersion", title: "Protocol Version", attributes: { "class": "text-center" } },
                        {
                            title: "&nbsp;", reorderable: false, filterable: false, sortable: false, attributes: { "class": "text-center" },
                            command: [
                                {
                                    text: "Delete",
                                    click: function (e) {
                                        var item = $("#gridNodes").data("kendoGrid").dataItem($(e.target).closest("tr"));
                                        api.deleteNode(item.NodeNo);
                                        //msgManager.DeleteNode(item.ID);
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

	                ////$(document).bind("kendo:skinChange", createChart);
	                ////e.detailRow.find(".deviceDetailsBatteryLevels").data("kendoChart").setOptions({
	                ////    categoryAxis: {
	                ////        baseUnit: "hours"
	                ////        //baseUnit: "days",
	                ////        //baseUnit: "months",
	                ////        //baseUnit: "weeks",
	                ////        //baseUnit: "years",
	                ////    }
	                ////});


	                function createSensorsGrid() {
	                    e.detailRow.find(".nodeDetailsSensors").kendoGrid({
	                        dataSource: {
	                            filter: { field: "NodeNo", operator: "eq", value: e.data.NodeNo	}
	                        },
	                        groupable: true,
	                        sortable: true,
	                        reorderable: true,
	                        columns: [
                                { field: "SensorNo", title: "ID", width: 40, groupable: false, attributes: { "class": "text-right" } },
                                { field: "Name", title: "Name", groupable: false },
                                { field: "TypeName", title: "Type" },
                                //{ field: "LastValue.Value", title: "Value", groupable: false, attributes: { "class": "text-right" }, template: kendo.template($("#sensorValueCellTemplate").html()) },
                                { field: "ProtocolVersion", title: "Protocol Version", attributes: { "class": "text-center" } }
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
	        });
	    }
	    function createSensorsGrid() {
	        //me.gridSensorsStateManager = new GridStateManager("gridSensors");

	        $("#gridSensors").kendoGrid({
                //height: 400,
	            groupable: true,
	            sortable: true,
	            reorderable: true,
	            pageable: {
	                pageSizes: [50, 100, 500, 1000],
	                pageSize: 50
	            },
	            columns:
                    [
                      { field: "NodeNo", title: "Node ID", width: 70, attributes: { "class": "text-right" } },
                      { field: "SensorNo", title: "ID", groupable: false, width: 40, attributes: { "class": "text-right" } },
                      { field: "Name", title: "Name", width: 80, groupable: false },
                      { field: "TypeName", title: "Type" },
                      //{ field: "LastValue.Value", title: "Value", groupable: false, attributes: { "class": "text-right" }, template: kendo.template($("#sensorValueCellTemplate").html()) },
                      { field: "ProtocolVersion", title: "Protocol Version", attributes: { "class": "text-center" } }
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

	    function onShowLayout() {
	        createNodesGrid();
	        createSensorsGrid();

            kendo.bind($("#content"), viewModel);
	    }

	    return {
	        start: function () {
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