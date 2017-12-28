var apiCaller = {
    ajaxFailHandler: function (jqXHR) {
        if ((jqXHR.responseText == '' && jqXHR.status == 12029))
            return; //suppress if connection to server failed

        var response = $.parseJSON(jqXHR.responseText);
        if (response) {
            if (jqXHR.status == 404 || jqXHR.status == 400)
                return {
                    ErrorMsg: response.Message,
                    Status: jqXHR.status
                };
            else
                return response.Message;
        }
    },
    callWebApi: function (params, httpVerb, onComplete, onError, indicateWaiting/*optional*/, extendErrorData/*optional*/, sync/*optional*/) {
        var apiPathAndParams, data = null;

        if (typeof params == 'string')
            apiPathAndParams = params; //api path and optional query params
        else {
            apiPathAndParams = params.path/*api path*/ + (params.params/*query params*/ || '');
            data = params.data; //optional JSON object
        }

        if (indicateWaiting)
            waitIndicator.show();

        return $.ajax({
            type: httpVerb,
            url: $.url(apiPathAndParams),
            contentType: "application/json; charset=utf-8",
            async: sync ? false : true,
            data: data ? JSON.stringify(data) : "",

            success: function (result, type, data2) {
                if (indicateWaiting)
                    waitIndicator.hide();
                if (onComplete)
                    onComplete(data2.status == 204/*No Content*/ ? null : result);
            },
            error: function (e) {
                if (indicateWaiting)
                    waitIndicator.hide();

                if ((e.responseText == '' && e.status == 12029))// || onError == null)
                    return; //suppress if connection to server failed

                //var response = $.parseJSON(e.responseText); // response will be null if e.responseText is empty (which can be on connection failure)
                //if (response) {
                //    if (onError)
                //        onError(response.Message);
                //    else {
                //        hideWaitForm();
                //        popupNotification.show(response.Message, NotificationType.Error);
                //    }
                //}

                //if (e.status == 409/*Conflict*/) {
                //    location.href = RMUtils.getUrl('Logout.aspx');
                //    return;
                //}
            }
        });
    },

    callWebApiGET: function (params, onComplete, onError, indicateWaiting/*optional*/) {
        return this.callWebApi(params, 'get', onComplete, onError, indicateWaiting);
    },
    callWebApiPOST: function (params, onComplete, onError, indicateWaiting/*optional*/) {
        return this.callWebApi(params, 'post', onComplete, onError, indicateWaiting);
    },
    callWebApiPUT: function (params, onComplete, onError, indicateWaiting/*optional*/) {
        return this.callWebApi(params, 'put', onComplete, onError, indicateWaiting);
    },
    callWebApiPUTsync: function (params, onComplete, onError, indicateWaiting/*optional*/) {
        return this.callWebApi(params, 'put', onComplete, onError, indicateWaiting, false, true);
    },
    callWebApiDELETE: function (params, onComplete, onError, indicateWaiting/*optional*/) {
        return this.callWebApi(params, 'delete', onComplete, onError, indicateWaiting);
    },
};