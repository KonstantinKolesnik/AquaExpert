
var thousandsSeparator = "";
var decimalSeparator = ".";
var serverAPICaller;

function initApplication() {
    initKendoCulture();

    function initKendoCulture() {
        kendo.culture("en-US");
        kendo.culture().numberFormat[","] = thousandsSeparator;
        kendo.culture().numberFormat["."] = decimalSeparator;
        kendo.culture().numberFormat.percent[","] = thousandsSeparator;
        kendo.culture().numberFormat.percent["."] = decimalSeparator;
        kendo.culture().numberFormat.currency[","] = thousandsSeparator;
        kendo.culture().numberFormat.currency["."] = decimalSeparator;
    }
}






$(document).ready(function () {
    //popupNotification = new PopupNotification();
    //popupDialog = new PopupDialog();
    //waitIndicator = new WaitIndicator();
    //waitForm = new WaitForm();

    serverAPICaller = new ServerAPICaller();
});
