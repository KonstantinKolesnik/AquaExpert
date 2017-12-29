
function SystemView() {
    var me = this;

    initView();


    function initView() {
        //serverAPICaller.apiGetSystemSectionItems(function (items) {
        //    alert(items.length);
        //});

    }



}

var viewSystem;

$(document).ready(function () {
    //preferencesManager = new PreferencesManager();
    //coreEventDispatcher = new CoreEventDispatcher();
    viewSystem = new SystemView();

    //$("#divtest").load("https://mecsoft.com/");

    //alert($.serverUrl());
})
