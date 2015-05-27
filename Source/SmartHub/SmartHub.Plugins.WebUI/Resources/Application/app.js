
define(
    ['marionette', 'backbone', 'jquery', 'signalR'],
    function (marionette, backbone, $, signalR) {
	    var api = {
		    parseParameters: function (queryString) {
			    var result = [];

			    if (queryString !== null && queryString !== undefined) {
				    var params = (queryString + '').split('/');

				    for (var i = 0; i < params.length; i++) {
					    var decodedValue = decodeURIComponent(params[i]);
					    result.push(decodedValue);
				    }
			    }

			    return result;
		    },
		    loadRoute: function (route, args) {
			    if (route) {
				    require([route], function (obj) {
					    obj.start.apply(obj, args);

					    if (args && args.length) {
						    var encoded = [];
						    for (var i = 0; i < args.length; i++)
							    encoded.push(encodeURIComponent(args[i]));

						    route += '?' + encoded.join('/');
					    }

					    backbone.history.navigate(route);
				    });
			    }
		    }
	    };

	    var app = new marionette.Application();
	    app.addRegions({ regionContent: "#region-page-content" });
	    app.setContentView = function (view) {
		    app.regionContent.show(view);
	    };
	    app.addTile = function (typeFullName, parameters) {
	        var args = {
	            typeFullName: typeFullName,
	            parameters: JSON.stringify(parameters)
	        };
	        $.post('/api/webui/tiles/add', args)
			    .done(function () {
				    app.navigate('tiles');
			    });
	    };
	    app.navigate = function (route) {
		    var args = Array.prototype.slice.call(arguments, 1);
		    api.loadRoute.call(this, route, args);
	    };
	    app.loadPath = function (route, args) {
		    api.loadRoute.call(this, route, args);
	    };
	    app.router = new marionette.AppRouter({
		    appRoutes: { '*path': 'loadPage' },
		    controller: {
			    loadPage: function (route, queryString) {
				    var args = api.parseParameters(queryString);
				    api.loadRoute.call(this, route, args);
			    }
		    }
	    });


	    app.SignalRReceivers = [];
	    app.SignalRSend = function (data) { };
	    function initSignalRPersistent(onComplete) {
	        $.support.cors = true;

	        var hostName = document.location.hostname;

	        var connection = $.connection("http://" + hostName + ":55556/chat");
	        connection.received(function (data) {
	            $.each(app.SignalRReceivers, function (idx, receiver) {
	                console.log(data);
	                if (receiver && receiver.SignalRReceiveHandler) {
	                    try {
	                        receiver.SignalRReceiveHandler(receiver, data);
	                    }
	                    catch (ex) {
                            // most likely it's a tile's handler and we aren't at the main page
	                        var idx = app.SignalRReceivers.indexOf(receiver);
	                        app.SignalRReceivers.splice(idx, 1);
	                    }
	                }
	            })
	        });
	        connection.error(function (error) {
	            console.warn(error);
	        });
	        connection.stateChanged(function (change) {
	            if (change.newState === $.signalR.connectionState.reconnecting)
	                console.log('Reconnecting');
	            else if (change.newState === $.signalR.connectionState.connected)
	                console.log('Connected');
	        });
	        connection.reconnected(function () {
	            console.log('Reconnected');
	        });
	        connection.connectionSlow(function () {
	            console.log('Connection is slow');
	        });

	        connection.start()
                .done(function () {
	                app.SignalRSend = function (data) {
	                    if (data)
	                        connection.send(JSON.stringify(data));
	                };

	                if (onComplete)
                        onComplete();
	            });
	    }


	    //app.wsClient = null;
	    //app.wsServer = null;
	    //app.onServerMessage = function (name, message) {}
	    //app.sendSignalRMessage = function (name, message) { app.wsServer.onClientMessage(name, message); }
	    //function initSignalRHubs(onComplete) {
	    //    var hostName = document.location.hostname;

	    //    $.getScript('http://' + hostName + ':55556/signalr/hubs', function () {
	    //        //Set the hubs URL for the connection
	    //        $.connection.hub.url = "http://" + hostName + ":55556/signalr";

	    //        // Create a function that the hub can call to broadcast messages.
	    //        $.connection.myHub.client.onServerMessage = app.onServerMessage;

	    //        // Start the connection.
        //        $.connection.hub.start({ jsonp: true }).done(function () {
	    //            app.wsClient = $.connection.myHub.client;
	    //            app.wsServer = $.connection.myHub.server;

	    //            if (onComplete)
	    //                onComplete();
	    //        });
	    //    });
	    //}

	    app.on('start', function () {
	        initSignalRPersistent();

	        if (backbone.history) {
			    backbone.history.start();

			    if (Backbone.history.fragment === '')
				    app.navigate('tiles');
		    }
	    });

	    return app;
    });