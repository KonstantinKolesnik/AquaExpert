
var msgManager;
var wsClient;
var mainView;
var viewModel;
//----------------------------------------------------------------------------------------------------------------------
function onWSClientOpen() {
    viewModel.set("IsConnected", true);
    msgManager.GetSettings();
    msgManager.GetVersion();
    msgManager.GetNodes();
    msgManager.GetModules();
}
function onWSClientMessage(txt) {
    if (msgManager)
        msgManager.ProcessMessage(txt);
}
function onWSClientClose() {
    viewModel.set("IsConnected", false);
    wsClient.start();
}
function onWSClientError() {
}
function onMsgManagerSend(txt) {
    wsClient.send(txt);
}
function onViewModelGet(e) {
}
function onViewModelBeforeSet(e) {
}
function onViewModelAfterSet(e) {
    switch (e.field) {
        case "Settings.WebTheme":
            mainView.applyTheme();
            if (!msgManager.IsFromServer)
                msgManager.SetSettings(viewModel.Settings.WebTheme, viewModel.Settings.UnitSystem);
            break;
        case "Settings.UnitSystem":
            if (!msgManager.IsFromServer)
                msgManager.SetSettings(viewModel.Settings.WebTheme, viewModel.Settings.UnitSystem);
            break;
        case "Modules":
            if (e.action == "itemchange")
                msgManager.SetModule(e.items[0]);
            break;
        default:
            break;
    }
}
//----------------------------------------------------------------------------------------------------------------------
function MainView() {
    var me = this;
    var lastContent = null;

    createMenu();
    createDevicesGrid();
    createSensorsGrid();
    createModulesGrid();
    createThemeSelector();
    createUnitSystemSelector();

    $(window).bind("resize", adjustSizes);
    $(window).resize(adjustSizes);

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
    this.applyTheme = function () {
        var skinName = viewModel.Settings.WebTheme || "default";
        var animate = false;

        var doc = document,
        kendoLinks = $("link[href*='kendo.']", doc.getElementsByTagName("head")[0]),
        commonLink = kendoLinks.filter("[href*='kendo.common']"),
        skinLink = kendoLinks.filter(":not([href*='kendo.common'])"),
        href = location.href,
        skinRegex = /kendo\.\w+(\.min)?\.css/i,
        extension = skinLink.attr("rel") === "stylesheet" ? ".css" : ".less",
        url = commonLink.attr("href").replace(skinRegex, "kendo." + skinName + "$1" + extension),
        exampleElement = $("#example");

        if (animate)
            preloadStylesheet(url, replaceTheme);
        else
            replaceTheme();

        function preloadStylesheet(file, callback) {
            var element = $("<link rel='stylesheet' media='print' href='" + file + "'").appendTo("head");

            setTimeout(function () {
                callback();
                element.remove();
            }, 100);
        }
        function replaceTheme() {
            var oldSkinName = $(doc).data("kendoSkin"),
                newLink;

            //if ($.browser.msie)
            //newLink = doc.createStyleSheet(url);
            //else
            newLink = skinLink.eq(0).clone().attr("href", url);

            newLink.insertBefore(skinLink[0]);
            skinLink.remove();

            $(doc.documentElement).removeClass("k-" + oldSkinName).addClass("k-" + skinName);
        }
    }
    this.getSensorValueUnit = function (sensor) {
        switch (sensor.LastValueType()) {
            case SensorValueType.Temperature: return viewModel.Settings.UnitSystem == "M" ? "°C" : "°F";
            case SensorValueType.Humidity: return "%";
                //case SensorValueType.Light:                 2,      // Light status. 0=off 1=on
            case SensorValueType.Dimmer: return "%";
                //case SensorValueType.Pressure:              4,      // Atmospheric Pressure
                //case SensorValueType.Forecast:              5,      // Whether forecast. One of "stable", "sunny", "cloudy", "unstable", "thunderstorm" or "unknown"
                //case SensorValueType.Rain:                  6,      // Amount of rain
                //case SensorValueType.RainRate:              7,      // Rate of rain
                //case SensorValueType.Wind:                  8,      // Windspeed
                //case Gust:                  9,      // Gust
                //case Direction:             10,     // Wind direction
                //case UV:                    11,     // UV light level
                //case Weight:                12,     // Weight (for scales etc)
                //case Distance:              13,     // Distance
                //case Impedance:             14,     // Impedance value
                //case Armed:                 15,     // Armed status of a security sensor. 1=Armed, 0=Bypassed
                //case Tripped:               16,     // Tripped status of a security sensor. 1=Tripped, 0=Untripped
                //case Watt:                  17,     // Watt value for power meters
                //case KWH:                   18,     // Accumulated number of KWH for a power meter
                //case SceneOn:               19,     // Turn on a scene
                //case SceneOff:              20,     // Turn of a scene
                //case Heater:                21,     // Mode of header. One of "Off", "HeatOn", "CoolOn", or "AutoChangeOver"
                //case HeaterSW:              22,     // Heater switch power. 1=On, 0=Off
                //case LightLevel:            23,     // Light level. 0-100%
                //case Var1:	                24,     // Custom value
                //case Var2:	                25,     // Custom value
                //case Var3:	                26,     // Custom value
                //case Var4:	                27,     // Custom value
                //case Var5:	                28,     // Custom value
                //case Up:	                    29,     // Window covering. Up.
                //case Down:	                30,     // Window covering. Down.
                //case Stop:	                31,     // Window covering. Stop.
                //case IRSend: 	            32,     // Send out an IR-command
                //case IRReceive:	            33,     // This message contains a received IR-command
                //case Flow:	                34,     // Flow of water (in meter)
                //case Volume: 	            35,     // Water volume
                //case LockStatus: 	        36,     // Set or get lock status. 1=Locked, 0=Unlocked
                //case case DustLevel:	            37,     // Dust level
                //case Voltage:	            38,     // Voltage level
                //case Current:	            39,     // Current level
            default: return "";
        }
    }
    this.adjustSizes = adjustSizes;

    function adjustSizes() {
        $("#content").height($(window).height() - $("#header").outerHeight() - $("#footer").outerHeight());

        adjustGrid($("#gridDevices"));
        adjustGrid($("#gridSensors"));
        adjustGrid($("#gridModules"));

        function adjustGrid(grid) {
            grid.height($(window).height() - getY(grid) - $("#footer").outerHeight() - 8/*don't change!*/);
            arrangeGridContent(grid);

            function getY() {
                var el = grid[0];
                var yPosition = el.offsetTop;
                while (el = el.offsetParent)
                    yPosition += el.offsetTop;
                return yPosition;
            }
            function arrangeGridContent() {
                var newHeight = grid.innerHeight(),
                    otherElements = grid.children().not(".k-grid-content"),
                    otherElementsHeight = 0;

                otherElements.each(function () { otherElementsHeight += $(this).outerHeight(); });
                grid.children(".k-grid-content").height(newHeight - otherElementsHeight);
            }
        }
    }

    function createMenu() {
        $("#panelbar").kendoPanelBar({
            //expandMode: "single",
            animation: {
                collapse: { // fade-out closing items over 1000 milliseconds
                    duration: 1000,
                    effects: "fadeOut"
                },
                expand: { // fade-in and expand opening items over 500 milliseconds
                    duration: 500,
                    effects: "expandVertical fadeIn"
                }
            },
            select: function (e) {
                var pnlContentHolder = $("#pnlContentHolder");
                var pnlContentHeader = $("#pnlContentHeader");

                if (lastContent) {
                    lastContent.insertAfter($("#dlg"));
                    lastContent.toggle(false);
                }

                var contentID = $(e.item).attr("contentid");
                pnlContentHolder.toggle(contentID != null);

                if (contentID) {
                    lastContent = $("#" + contentID);
                    if (lastContent) {
                        lastContent.insertAfter(pnlContentHeader);
                        lastContent.toggle(true);

                        var title = $(e.item).closest("ul").closest("li").find("span.k-link:first").text() + "&nbsp;&nbsp;>&nbsp;&nbsp;" + $(e.item).text();
                        pnlContentHeader.find("label").html(title);

                        adjustSizes();
                    }
                }
            }
        });
    }

    function createDevicesGrid() {
        $("#gridDevices").kendoGrid({
            groupable: true,
            scrollable: true,
            sortable: true,
            reorderable: true,
            //filterable: true,
            //resizable: true,
            pageable: {
                pageSizes: [10, 20, 50, 100, 300],
                pageSize: 50
            },
            columns:
                [
                  //{ title: "&nbsp;", reorderable: false, groupable: false, filterable: false, sortable: false, width: 80, template: '<img src="Resources/Device1.png" height="48px" style="vertical-align: middle;" alt=""/>' },
                  { field: "ID", title: "ID", groupable: false, width: 100 },
                  { field: "TypeName()", title: "Type" },
                  { field: "ProtocolVersion", title: "Protocol Version" },
                  { field: "SketchName", title: "Firmware Name" },
                  { field: "SketchVersion", title: "Firmware Version" },
                  { field: "Sensors.length", title: "Sensors Count" },
                  { field: "LastBatteryLevel()", title: "Battery, %", template: kendo.template($("#batteryLevelCellTemplate").html()) }
                ],
            detailTemplate: kendo.template($("#deviceDetailsTemplate").html()),
            detailInit: function (e) {
                var detailRow = e.detailRow;

                createTabs();
                createSensorsGrid();
                createBatteryLevelsChart(detailRow.find(".deviceDetailsBatteryLevels"));
                kendo.bind(detailRow, e.data);

                //$(document).bind("kendo:skinChange", createChart);
                //detailRow.find(".deviceDetailsBatteryLevels").data("kendoChart").setOptions({
                //    categoryAxis: {
                //        baseUnit: "hours"
                //        //baseUnit: "days",
                //        //baseUnit: "months",
                //        //baseUnit: "weeks",
                //        //baseUnit: "years",
                //    }
                //});

                function createTabs() {
                    detailRow.find(".deviceDetailsTabStrip").kendoTabStrip({
                        animation: {
                            open: { effects: "fadeIn" }
                        }
                    });
                }
                function createSensorsGrid() {
                    detailRow.find(".deviceDetailsSensors").kendoGrid({
                        groupable: true,
                        scrollable: true,
                        sortable: true,
                        reorderable: true,
                        columns: [
                            //{ title: "&nbsp;", reorderable: false, groupable: false, filterable: false, sortable: false, width: 80, template: '<img src="Resources/UltrasonicSonarSensor1.png" height="48px" style="vertical-align: middle;" alt=""/>' },
                            { field: "ID", title: "ID", groupable: false, width: 100 },
                            { field: "TypeName()", title: "Type" },
                            { field: "ProtocolVersion", title: "Protocol Version" },
                            { field: "LastValue()", title: "Value", template: kendo.template($("#sensorValueCellTemplate").html()) }
                        ]
                    });
                }
            }
        });
    }
    function createSensorsGrid() {
        $("#gridSensors").kendoGrid({
            groupable: true,
            scrollable: true,
            sortable: true,
            reorderable: true,
            pageable: {
                pageSizes: [50, 100, 500, 1000],
                pageSize: 100
            },
            columns:
                [
                  //{ title: "&nbsp;", reorderable: false, groupable: false, filterable: false, sortable: false, width: 80, template: '<img src="Resources/UltrasonicSonarSensor1.png" height="48px" style="vertical-align: middle;" alt=""/>' },
                  { field: "NodeID", title: "Device ID", width: 100 },
                  { field: "ID", title: "ID", groupable: false, width: 100 },
                  { field: "TypeName()", title: "Type" },
                  { field: "ProtocolVersion", title: "Protocol Version" },
                  { field: "LastValue()", title: "Value", template: kendo.template($("#sensorValueCellTemplate").html()) }
                ],
            detailTemplate: kendo.template($("#sensorDetailsTemplate").html()),
            detailInit: function (e) {
                var detailRow = e.detailRow;

                createSensorValuesChart(detailRow.find(".sensorDetailsValues"), e.data);
                kendo.bind(detailRow, e.data);
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
        });
    }
    function createModulesGrid() {
        $("#gridModules").kendoGrid({
            scrollable: true,
            sortable: true,
            reorderable: true,
            resizable: true,
            edit: "inline",
            //toolbar: ["create", "save", "cancel"],
            //toolbar: "<p>My string template in a paragraph.</p>",
            //toolbar: kendo.template("<p>My function template.</p>"),
            toolbar: [
                //{ name: "create" },
                //{ name: "save" },
                //{ name: "cancel" }
                //{ template: kendo.template($("#template").html()) }
                { name: "aaa", template: '<a class="k-button" href="\\#" onclick="msgManager.AddModule();">Add new module</a>' }
            ],
            pageable: {
                pageSizes: [10, 20, 50, 100, 500],
                pageSize: 20
            },
            columns:
                [
                  { title: "&nbsp;", reorderable: false, filterable: false, sortable: false, width: 80, template: '<img src="resources/Operation.png" height="48px" style="vertical-align: middle;" alt=""/>' },
                  { field: "Name", title: "Name" },
                  { field: "Description", title: "Description" },
                  {
                      title: "&nbsp;",
                      width: 150,
                      command: [
                        //"edit",
                        //"destroy",
                        {
                            //name: "destroy",
                            text: "Delete",
                            click: function (e) {
                                debugger;
                            }
                        }
                      ]
                  }
                ],
            detailTemplate: kendo.template($("#moduleDetailsTemplate").html()),
            detailInit: function (e) {
                kendo.bind(e.detailRow, e.data);
            },
            dataBinding: function (e) {
                if (e.action == "itemchange") {
                    e.preventDefault();

                    var item = e.items[0];

                    //get the current column names, in their current order
                    var grid = $("#gridModules").data("kendoGrid");
                    var columnNames = $.map(grid.columns, function (column) { return column.field ? column.field : ""; });

                    //get the column tds for update
                    var masterRow = $('#gridModules > div.k-grid-content > table > tbody > tr[data-uid="' + item.uid + '"]');
                    var tds = masterRow.find('td:not(.k-hierarchy-cell)');

                    //collapse the detail row that was saved.
                    //grid.collapseRow(masterRow);
                    //grid.expandRow(masterRow);

                    //update the tds with the value from the current item stored in items
                    for (var i = 0 ; i < tds.length ; i++)
                        if (columnNames[i])
                            $(tds[i]).html(item[columnNames[i]]);
                }
                else if (e.action == "rebind") {
                    if (e.items.length) {
                        //debugger;
                        //e.preventDefault();





                    }
                }
            }
        });
    }

    function createThemeSelector() {
        $("#ddlTheme").kendoDropDownList({
            dataSource: [
                { text: "Black", value: "black" },
                { text: "Blue Opal", value: "blueopal" },
                { text: "Bootstrap", value: "bootstrap" },
                { text: "Default", value: "default" },
                { text: "Flat", value: "flat" },
                { text: "High Contrast", value: "highcontrast" },
                { text: "Metro", value: "metro" },
                { text: "Metro Black", value: "metroblack" },
                { text: "Moonlight", value: "moonlight" },
                { text: "Silver", value: "silver" },
                { text: "Uniform", value: "uniform" }
            ],
            dataTextField: "text",
            dataValueField: "value"
        });
    }
    function createUnitSystemSelector() {
        $("#ddlUnitSystem").kendoDropDownList({
            dataSource: [
                { text: "Metric", value: "M" },
                { text: "Imperial", value: "I" }
            ],
            dataTextField: "text",
            dataValueField: "value"
        });
    }

    function createBatteryLevelsChart(selector) {
        selector.kendoChart({
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
//----------------------------------------------------------------------------------------------------------------------
function onDocumentReady() {
    msgManager = new MessageManager();
    msgManager.onSend = onMsgManagerSend;

    mainView = new MainView();
    mainView.adjustSizes();

    viewModel = kendo.observable(new Model());
    kendo.bind($("body"), viewModel);
    viewModel.bind("get", onViewModelGet);
    viewModel.bind("set", onViewModelBeforeSet);
    viewModel.bind("change", onViewModelAfterSet);

    wsClient = new WSClient(12000, "SmartNetwork");
    wsClient.onOpen = onWSClientOpen;
    wsClient.onMessage = onWSClientMessage;
    wsClient.onClose = onWSClientClose;
    wsClient.onError = onWSClientError;
    wsClient.start();


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

/*
Следующие методы позволяют устанавливать компоненты даты и времени:

setFullYear(year [, month, date])
setMonth(month [, date])
setDate(date)
setHours(hour [, min, sec, ms])
setMinutes(min [, sec, ms])
setSeconds(sec [, ms])
setMilliseconds(ms)
setTime(milliseconds) (устанавливает всю дату по миллисекундам с 01.01.1970 UTC)

Все они, кроме setTime(), обладают также UTC-вариантом, например: setUTCHours().
Как видно, некоторые методы могут устанавливать несколько компонентов даты одновременно, в частности, setHours. При этом если какая-то компонента не указана, она не меняется.
*/

function createMainMenu() {
    var mainMenuItems =
        [
            { Name: "Layout", Url: "Database.png", Action: "model.set('UIState', UIStateType.Layout);" },
            { Name: "Operation", Url: "Operation.png", Action: "model.set('UIState', UIStateType.Operation);" },
            { Name: "Decoders", Url: "Decoder.png", Action: "model.set('UIState', UIStateType.Decoders);" },
            { Name: "Settings", Url: "Settings.png", Action: "model.set('UIState', UIStateType.Settings); model.MessageManager.GetOptions();" },
            { Name: "Information", Url: "Info.png", Action: "model.set('UIState', UIStateType.Information);" },
            { Name: "Firmware", Url: "Update.png", Action: "model.set('UIState', UIStateType.Firmware); model.MessageManager.GetVersion();" }
        ];

    $("#lvMainMenu").kendoListView({
        dataSource: { data: mainMenuItems },
        template: kendo.template($("#tmpltMainMenuItem").html())
    });
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