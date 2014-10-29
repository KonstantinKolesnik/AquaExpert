
var msgManager;
var wsClient;
var mainView;
//----------------------------------------------------------------------------------------------------------------------
function onWSClientOpen() {
    //alert("open!");
    msgManager.HelloWorld();
}
function onWSClientMessage(txt) {
    if (msgManager)
        msgManager.onReceive(txt);
}
function onWSClientClose() {
    wsClient.start();
}
function onWSClientError() {
    //showDialog("WebSocket error: " + e);
}

function onMsgManagerSend(txt) {
    wsClient.send(txt);
}
//----------------------------------------------------------------------------------------------------------------------
function MainView() {
    var me = this;

    $(window).bind("resize", onWindowResize);

    this.showDialog = function (txt, title) {
        var win = $("#dlg").kendoWindow({
            actions: ["Close"],
            width: "400px",
            height: "200px",
            title: "Smart Network",
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

    function onWindowResize() {
        //var h1 = $("header").height();
        //alert($("body").height());

        //$("#content").height($(document).height() - $("header").height() - $("footer").height());
    
        //var gridElement = $("#gridLayout"),
        //    newHeight = gridElement.innerHeight(),
        //    otherElements = gridElement.children().not(".k-grid-content"),
        //    otherElementsHeight = 0;
        //otherElements.each(function () {
        //    otherElementsHeight += $(this).outerHeight();
        //});
        //gridElement.children(".k-grid-content").height(newHeight - otherElementsHeight);
    }
}
//----------------------------------------------------------------------------------------------------------------------
function onDocumentReady() {
    msgManager = new MessageManager();
    msgManager.onSend = onMsgManagerSend;

    wsClient = new WSClient(12000, "SmartNetwork");
    wsClient.onOpen = onWSClientOpen;
    wsClient.onMessage = onWSClientMessage;
    wsClient.onClose = onWSClientClose;
    wsClient.onError = onWSClientError;
    wsClient.start();

    mainView = new MainView();
    //mainView.showDialog("test");

    //model = kendo.observable(new Model());
    //kendo.bind($("body"), model);



    //createMainMenu();
    //createLayout();
    //createOperation();
    ////createDecoders();
    //createOptions();


//    onWindowResize();

    // for test!!!
    //var loco1 = new Locomotive(1, "MKV", "aaaaaaa", new LocomotiveAddress(7, false), Protocol.DCC28);
    //var loco2 = new Locomotive(2, "M62", "bbbbbbb", new LocomotiveAddress(3, false), Protocol.DCC28);
    //model.get("OperationList").push(kendo.observable(new LocomotiveOperator(loco1)));
    //model.get("OperationList").push(kendo.observable(new LocomotiveOperator(loco2)));


    //kendo.init($(".SpeedSlider"));
    //kendo.init($(".SpeedGauge"));

//    $("#lvOperation .SpeedSlider").each(function (idx, b) {
//        //        var slider = $(b).kendoSlider({
//        //                increaseButtonTitle: "Right",
//        //                decreaseButtonTitle: "Left",
//        //                min: -10,
//        //                max: 10,
//        //                smallStep: 2,
//        //                largeStep: 1,
//        //            });
////        kendo.init($(b));

//        //$(b).getKendoSlider().value(20);

//        var lo = $('#lvOperation').data('kendoListView').dataSource._data[idx];
//        //kendo.bind($(b).getKendoSlider(), lo);
//    });
//    $("#lvOperation .SpeedGauge").each(function (idx, b) {
//        var gaugeConfig = {
//            theme: "black",
//            pointer: {
//                value: 0
//            },
//            scale: {
//                startAngle: 0,
//                endAngle: 180,
//                labels: {
//                    //font: "10px Georgia, Helvetical, sans-serif",
//                    //template: "#=value# km/h"
//                    position: "outside"
//                },
////                ranges: [
////                    { from: 0, to: 9, color: "#00ab00" }, //green
////                    {from: 9, to: 18, color: "#d3ce37" }, //yellow
////                    {from: 18, to: 28, color: "#ae130f" } //red
////                ],
//                min: 0,
//                max: 28,
//                majorUnit: 2,
//                minorUnit: 1
//            }
//        };

//        //var speedGauge = $(b).kendoRadialGauge(gaugeConfig).data('kendoRadialGauge');

//        //$(b).getKendoSlider().value(20);

//        //var lo = $('#lvOperation').data('kendoListView').dataSource._data[idx];
//        //kendo.bind($(b).getKendoSlider(), lo);
//    });
}
//----------------------------------------------------------------------------------------------------------------------
function createMainMenu() {
    var mainMenuItems = [
        { Name: "Layout", Url: "Database.png", Action: "model.set('UIState', UIStateType.Layout);" },
        { Name: "Operation", Url: "Operation.png", Action: "model.set('UIState', UIStateType.Operation);" },
        { Name: "Decoders", Url: "Decoder.png", Action: "model.set('UIState', UIStateType.Decoders);" },
        { Name: "Settings", Url: "Settings.png", Action: "model.set('UIState', UIStateType.Settings); model.MessageManager.GetOptions();" },
        { Name: "Information", Url: "Info.png", Action: "model.set('UIState', UIStateType.Information);" },
        { Name: "Firmware", Url: "Update.png", Action: "model.set('UIState', UIStateType.Firmware); model.MessageManager.GetVersion();" }
        ];

    $("#lvMainMenu").kendoListView({ template: kendo.template($("#tmpltMainMenuItem").html()), dataSource: { data: mainMenuItems } });
}

//UIStateType = {
//    Main: 0,
//    Layout: 1,
//    Operation: 2,
//    Decoders: 3,
//    Settings: 4,
//    Information: 5,
//    Firmware: 6
//}


function getLayouts() {
    //var crudServiceBaseUrl = "http://demos.kendoui.com/service",
    //dataSource = new kendo.data.DataSource({
    //    transport: {
    //        read: {
    //            url: crudServiceBaseUrl + "/Products",
    //            dataType: "jsonp"
    //        },
    //        update: {
    //            url: crudServiceBaseUrl + "/Products/Update",
    //            dataType: "jsonp"
    //        },
    //        destroy: {
    //            url: crudServiceBaseUrl + "/Products/Destroy",
    //            dataType: "jsonp"
    //        },
    //        create: {
    //            url: crudServiceBaseUrl + "/Products/Create",
    //            dataType: "jsonp"
    //        },
    //        parameterMap: function (options, operation) {
    //            if (operation !== "read" && options.models) {
    //                return { models: kendo.stringify(options.models) };
    //            }
    //        }
    //    },
    //    batch: true,
    //    pageSize: 4, //4
    //    schema: {
    //        model: {
    //            id: "ProductID",
    //            fields: {
    //                ProductID: { editable: false, nullable: true },
    //                ProductName: "ProductName",
    //                UnitPrice: { type: "number" },
    //                Discontinued: { type: "boolean" },
    //                UnitsInStock: { type: "number" }
    //            }
    //        }
    //    }
    //});


    //$("#lvLayoutsPager").kendoPager({ dataSource: dataSource });
    //var listView = $("#lvLayouts").kendoListView({
    //    dataSource: dataSource,
    //    template: kendo.template($("#template").html()),
    //    editTemplate: kendo.template($("#editTemplate").html())
    //}).data("kendoListView");
    //$(".k-add-button").click(function (e) {
    //    listView.add();
    //    e.preventDefault();
    //});
}
function createLayout() {
    var baseUrl = "http://" + document.location.host;//.origin;
    //alert(baseUrl);
    var gridLayout = $("#gridLayout").kendoGrid({
        dataSource: {
            transport: {
                create: {
                    url: baseUrl + "/Content/Layout/Create.json",
                    dataType: "json"
                },
                read: {
                    url: baseUrl + "/Content/Layout.json",
                    dataType: "json"
                }
                //                update: {
                //                    url: baseUrl + "/Content/Update.json",
                //                    dataType: "jsonp"
                //                },
                //                destroy: {
                //                    url: baseUrl + "/Content/Destroy.json",
                //                    dataType: "jsonp"
                //                }
            },
            //serverPaging: true,
            pageSize: 10
        },
        groupable: true,
        scrollable: { virtual: true },
        sortable: true,
        //navigatable: true,
        //selectable: "row",
        //rowTemplate: kendo.template($("#template").html()),

        //pageable: true,
        pageable: {
            refresh: true,
            pageSizes: [5, 10, 20, 50],
            numeric: true, // show numeric buttons
            buttonCount: 10, // default 10
            input: true,
            info: true
        },

        editable: true,
        //editable: "inline",
        //            editable: {
        //                update: false,
        //                destroy: true
        //            },
        //editable: [ "popup", update: false, destroy: true ],

        filterable: true,

        //toolbar: ["create", "save", "cancel"],
        //toolbar: kendo.template($("#gridLayoutToolbar").html()),
        toolbar: [
                "create",
        //{ name: "save", text: "Save This Record" },
        //{ name: "cancel", template: '<img src="/Resources/Locomotive.ico" rel="cancel" height="24px" />' }
                {name: "cancel", template: kendo.template($("#gridLayoutToolbar").html()) }
            ],

        //aggregate: [{ field: "Type", aggregate: "count"}],
        columns: [
              { title: "&nbsp;", filterable: false, sortable: false, width: 50, template: '<img src="Resources/${Type}.ico" height="24px" alt=""/>' },
              { field: "Type", title: "Type", filterable: false, sortable: false },
              { field: "Name", title: "Name" },
              { field: "Description", title: "Description", filterable: false, sortable: false },

              { field: "Data.Protocol", title: "Protocol" },
              { field: "Data.Address", title: "Address" },
              { field: "Data.ExtAddress", title: "Ext. address" },
              { field: "Data.UseExtAddress", title: "Use ext. address", template: '# if (Data.UseExtAddress && Data.UseExtAddress == "true") { # v # } #' },
              { field: "Data.UseInConsist", title: "Use in consist", template: '# if (Data.UseInConsist && Data.UseInConsist == "true") { # v # } #' },


              { command: [ "edit", "destroy", { text: "Details", click: showDetails} ], title: "&nbsp;", width: "250px" }
            ],
        schema: {
            model: {
                id: "LayoutID",
                fields: {
                    Image: { editable: false, nullable: true },
                    Type: { editable: false, nullable: false },
                    Name: { editable: true, nullable: false, validation: { required: true} },
                    Description: { editable: true, nullable: true },
                    Data: { editable: false, nullable: true }

                    //                        ProductName: { validation: { required: true} },
                    //                        UnitPrice: { type: "number", validation: { required: true, min: 1} },
                    //                        Discontinued: { type: "boolean" },
                    //                        UnitsInStock: { type: "number", validation: { min: 0, required: true} }
                }
            }
        }
    });

    //        var wnd2 = $("#dlg").kendoWindow({
    //            title: "Customer Details",
    //            modal: true,
    //            visible: false,
    //            resizable: false,
    //            width: 300
    //        }).data("kendoWindow");
    function showDetails(e) {
        e.preventDefault();

        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        //wnd2.content(detailsTemplate(dataItem));
        //wnd2.center().open();
    }

    var itemTypes = [
            { type: "Locomotive", text: "Locomotives" },
            { type: "Consist", text: "Consists" },
            { type: "Turnout", text: "Turnouts" },
            { type: "Signal", text: "Signals" },
            { type: "Turntable", text: "Turntables" },
            { type: "AccessoryGroup", text: "Accessory groups" }
        ];
    var dropDown = gridLayout.find("#cbItemType").kendoDropDownList({
        dataSource: itemTypes,
        dataValueField: "type",
        dataTextField: "text",
        optionLabel: "All",
        template: kendo.template($("#cbItemTypeTemplate").html()),
        change: function () { onFilterTypeChange(this.value()); }
    }).data("kendoDropDownList");
    dropDown.select(0);
    onFilterTypeChange(dropDown.dataItem().value);

    function onFilterTypeChange(value) {
        var gridData = gridLayout.data("kendoGrid");
        gridData.dataSource.filter(value ? { field: "Type", operator: "eq", value: value} : {});

        showColumn("Type", !value);
        showColumn("Data.Protocol", value == "Locomotive");
        showColumn("Data.Address", value == "Locomotive");
        showColumn("Data.ExtAddress", value == "Locomotive");
        showColumn("Data.UseExtAddress", value == "Locomotive");
        showColumn("Data.UseInConsist", value == "Locomotive");




        var colEditIdx = gridData.columns.length - 1;
        value ? gridData.showColumn(colEditIdx) : gridData.hideColumn(colEditIdx);

        //alert($("#btnAdd"));
        //$("#btnAdd").attr("display", value ? "block" : "none");
        //alert($("#btnAdd").attr("display"));

        function showColumn(colName, visible) { visible ? gridData.showColumn(colName) : gridData.hideColumn(colName); }
    }
}
function createOperation() {
    var lv = $("#lvOperation").kendoListView({
        template: kendo.template($("#tmpltOperationItem").html()),
        dataSource: { data: model.get("OperationList"), change: onDataChange },
        remove: onDataChange
        //scrollable: true,
        //height: 200
    }).data("kendoListView");
    kendo.init($(".SpeedSlider"));
    kendo.init($(".SpeedGauge"));

    function onDataChange(e) {
        $("#lvOperation .SpeedSlider").each(function (idx, b) {
            //        var slider = $(b).kendoSlider({
            //                increaseButtonTitle: "Right",
            //                decreaseButtonTitle: "Left",
            //                min: -10,
            //                max: 10,
            //                smallStep: 2,
            //                largeStep: 1,
            //            });
            
            //kendo.init($(b));


            //var lv = $('#lvOperation').data('kendoListView');
            //kendo.bind($(b), lv.dataSource._data[idx]);
        });
    }
}
function createDecoders() {
    var baseUrl = document.location.origin;
    var grid = $("#gridDecoders").kendoGrid({
        dataSource: {
            transport: {
                read: {
                    url: baseUrl + "/Content/Decoders.json",
                    dataType: "json"
                }
            },
            pageSize: 20
        },

        height: 500,
        rowTemplate: kendo.template($("#gridDecodersRT").html()),
        filterable: true,
        sortable: true,
        pageable: true,
        //detailInit: detailInit,
        //            dataBound: function () {
        //                this.expandRow(this.tbody.find("tr.k-master-row").first());
        //            },
        columns: [
            {
                //field: "Type",
                //title: "Type"
                width: 40
                //format: "{0:MM/dd/yyyy}"
            },
            {
                field: "Type",
                title: "Type"
                //width: 100
                //format: "{0:MM/dd/yyyy}"
            },
            {
                field: "Model",
                title: "Model"
                //width: 200
            },
            {
                field: "Features",
                title: "Features"
                //width: 200
            }
            ]
    });

    //        function detailInit(e) {
    //            $("<div/>").appendTo(e.detailCell).kendoGrid({
    //                dataSource: {
    //                    type: "odata",
    //                    transport: {
    //                        read: "http://demos.kendoui.com/service/Northwind.svc/Orders"
    //                    },
    //                    serverPaging: true,
    //                    serverSorting: true,
    //                    serverFiltering: true,
    //                    pageSize: 6,
    //                    filter: { field: "EmployeeID", operator: "eq", value: e.data.EmployeeID }
    //                },
    //                scrollable: false,
    //                sortable: true,
    //                pageable: true,
    //                columns: [
    //                            { field: "OrderID", width: 70 },
    //                            { field: "ShipCountry", title: "Ship Country", width: 100 },
    //                            { field: "ShipAddress", title: "Ship Address" },
    //                            { field: "ShipName", title: "Ship Name", width: 200 }
    //                        ]
    //            });
    //        }
}
function createOptions() {
    $("#mti").kendoNumericTextBox({ min: 500, max: 3700, step: 100, format: "d4" });
    $("#pti").kendoNumericTextBox({ min: 500, max: 1000, step: 100, format: "d4" });
}

var speedGauge;
var speedSlider;
function createSpeedGauge() {
    var gaugeConfig = {
        theme: "black",
        pointer: {
            value: 0
        },
        scale: {
            startAngle: 0,
            endAngle: 180,
            labels: {
                //font: "10px Georgia, Helvetical, sans-serif",
                //template: "#=value# km/h"
                position: "outside"
            },
            ranges: [
                { from: 0, to: 9, color: "#00ab00" }, //green
                { from: 9, to: 18, color: "#d3ce37" }, //yellow
                { from: 18, to: 28, color: "#ae130f" }, //red

                {from: 0, to: -9, color: "#00ab00" }, //green
                {from: -9, to: -18, color: "#d3ce37" }, //yellow
                {from: -18, to: -28, color: "#ae130f" } //red
                
          ],
          min: -28,
          max: 28,
          majorUnit: 2,
          minorUnit: 1
        }
    };

    speedGauge = $('#speedGaugeF').kendoRadialGauge(gaugeConfig).data('kendoRadialGauge');
    speedSlider = $('#speedSlider').kendoSlider().data('kendoSlider');
    speedSlider.bind('change', function (e) {
        speedGauge.value(e.value);
    });
    //speedGauge.value(speedSlider.value());
}





//----------------------------------------------------------------------------------------------------------------------


//var speed = 0;
//var forward = true;
//var address = new LocomotiveAddress(7, false);

//function ff() {
//    speed++;
//    speed = Math.min(speed, 28);
//    forward = speed > 0;
//    model.MessageManager.SetLocoSpeed28(address, Math.abs(speed), forward);

//    //speedGauge.value(Math.abs(speed));
//    //speedGauge.value(speed);
//}
//function ss() {
//    speed = 0;
//    model.MessageManager.SetLocoSpeed28(address, speed, forward);

//    //speedGauge.value(Math.abs(speed));
//    //speedGauge.value(speed);
//}
//function rr() {
//    speed--;
//    speed = Math.max(speed, -28);
//    forward = speed > 0;
//    model.MessageManager.SetLocoSpeed28(address, Math.abs(speed), forward);

//    //speedGauge.value(Math.abs(speed));
//    //speedGauge.value(speed);
//}
//function lightOn() { model.MessageManager.SetLocoFunctionGroup1(address, true, false, false, false, false); }
//function lightOff() { model.MessageManager.SetLocoFunctionGroup1(address, false, false, false, false, false); }



//function aaa(item) {
//    var a = 0;
//    var b = a;



//    //var a = $(item).kendoSlider().data('kendoSlider');
//    kendo.init($(item));

//    //var b = $(item).parents('.OperationItem:first').find('.SpeedSlider')[1];//.kendoSlider().data('kendoListView');
//    //kendo.init($(b));
    
//    //($(b)).kendoSlider().data("kendoSlider").value(20);

//}