
function ServerAPICaller() {
    var me = this;

    me.apiGetSystemSectionItems = function (onComplete, onError) {
        apiCaller.callWebApiGET("/api/ui/sections/system", onComplete, onError);
    };





}