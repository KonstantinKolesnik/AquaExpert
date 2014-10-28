
function WSClient(port, resourcePath) {
    var me = this;
    var socket = null;

    this.onOpen = null;
    this.onMessage = null;
    this.onClose = null;
    this.onError = null;

    this.start = function () {
        createWebSocket();
    }
    this.stop = function () {
        closeWebSocket();
    }
    this.send = function (text) {
        if (socket && text)
            socket.send(text);
    }

    function createWebSocket() {
        var support = "MozWebSocket" in window ? 'MozWebSocket' : ("WebSocket" in window ? 'WebSocket' : null);
        if (support) {
            //socket = new WebSocket("ws://" + document.location.hostname + ":" + port + '/' + resourcePath);
            socket = new window[support]("ws://" + document.location.hostname + ":" + port + '/' + resourcePath);

            socket.onopen = onSocketOpen;
            socket.onmessage = onSocketMessage;
            socket.onclose = onSocketClose;
            socket.onerror = onSocketError;
        }
        else
            //showDialog("No WebSocket support! Please try another browser.", "Warning");
            alert("No WebSocket support! Please try another browser.");

        function onSocketOpen() {
            if (me.onOpen)
                me.onOpen();
        }
        function onSocketMessage(e) {
            if (me.onMessage)
                me.onMessage(e.data);
        }
        function onSocketClose() {
            closeWebSocket();
            if (me.onClose)
                me.onClose();
        }
        function onSocketError() {
            if (me.onError)
                me.onError();
        }
    }
    function closeWebSocket() {
        if (socket)
            socket.close();
        socket = null;
    }
}