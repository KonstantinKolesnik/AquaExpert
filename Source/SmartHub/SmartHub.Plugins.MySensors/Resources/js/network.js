
define(
	['app', 'marionette', 'backbone', 'underscore', 'webapp/mysensors/views'],
	function (application, marionette, backbone, _, views) {
	    var viewModel = kendo.observable({
	        Nodes: [],
            Sensors: []
	    });

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
	        }
	    }

	    function createNodesGrid() {
	        //me.gridNodesStateManager = new GridStateManager("gridNodes");

	        $("#gridNodes").kendoGrid({
	            groupable: true,
	            sortable: true,
	            reorderable: true,
	            pageable: {
	                pageSizes: [10, 20, 50, 100, 300],
	                pageSize: 50
	            },
	            columns:
                    [
                        //{ title: "&nbsp;", reorderable: false, groupable: false, filterable: false, sortable: false, width: 80, template: '<img src="Resources/Device1.png" height="48px" style="vertical-align: middle;" alt=""/>' },
                        { field: "NodeNo", title: "ID", width: 40, groupable: false, attributes: { "class": "text-right" } },
                        { field: "Name", title: "Name", width: 150, groupable: false },
                        //{ field: "TypeName()", title: "Type", width: 80 },
                        { field: "SketchName", title: "Firmware Name", width: 120 },
                        { field: "SketchVersion", title: "Firmware Version", width: 100, attributes: { "class": "text-center" } },
                        //{ field: "Sensors.length", title: "Sensors Count", width: 110, attributes: { "class": "text-center" } },
                        //{ field: "LastBatteryLevel.Value", title: "Battery, %", width: 90, attributes: { "class": "text-center" }, template: kendo.template($("#batteryLevelCellTemplate").html()) },
                        { field: "ProtocolVersion", title: "Protocol Version", width: 100, attributes: { "class": "text-center" } },
                        {
                            title: "&nbsp;", width: 80, reorderable: false, filterable: false, sortable: false, attributes: { "class": "text-center" },
                            command: [
                                {
                                    text: "Delete",
                                    click: function (e) {
                                        var item = $("#gridNodes").data("kendoGrid").dataItem($(e.target).closest("tr"));
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
	                        groupable: true,
	                        sortable: true,
	                        reorderable: true,
	                        columns: [
                                //{ title: "&nbsp;", reorderable: false, groupable: false, filterable: false, sortable: false, width: 80, template: '<img src="Resources/UltrasonicSonarSensor1.png" height="48px" style="vertical-align: middle;" alt=""/>' },
                                //{ field: "NodeID", title: "Node ID", width: 70, attributes: { "class": "text-right" } },
                                { field: "ID", title: "ID", groupable: false, width: 40, attributes: { "class": "text-right" } },
                                //{ field: "TypeName()", title: "Type" },
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

            kendo.bind($("#content"), viewModel);

            api.getNodes(
                function (data) {
                    viewModel.set("Nodes", data);
                },
                function () {
                    showDialog("Ошибка загрузки узлов.");
                });
	    }

	    return {
	        start: function () {
                var layout = new views.layoutView();
                layout.onShow = onShowLayout;
                application.setContentView(layout);

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