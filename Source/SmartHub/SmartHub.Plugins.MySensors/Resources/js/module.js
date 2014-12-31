
define(
	['app', 'marionette', 'backbone', 'underscore', 'jquery', 'webapp/mysensors/views'],
	function (application, marionette, backbone, _, $, views) {
	    var api = {
	        getNodes: function (onComplete) {
	            $.getJSON('/api/mysensors/nodes')
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function (data) {
	                    onError(data);
	                });
	        },
	        getSensors: function (onComplete) {
	            $.getJSON('/api/mysensors/sensors')
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function (data) {
	                    onError(data);
	                });
	        },
	        setNodeName: function (id, name, onComplete) {
	            $.post('/api/mysensors/nodes/setname', { Id: id, Name: name })
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function (data) {
	                    onError(data);
	                });
	        },
	        setSensorName: function (id, name, onComplete) {
	            $.post('/api/mysensors/sensors/setname', { Id: id, Name: name })
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function (data) {
	                    onError(data);
	                });
	        },
	        deleteNode: function (id, onComplete) {
	            $.post('/api/mysensors/nodes/delete', { Id: id })
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function (data) {
	                    onError(data);
	                });
	        },
	        deleteSensor: function (id, onComplete) {
	            $.post('/api/mysensors/sensors/delete', { Id: id })
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function (data) {
	                    onError(data);
	                });
	        },
	        getUnitSystem: function (onComplete) {
	            $.getJSON('/api/mysensors/unitsystem')
					.done(function (data) {
					    if (onComplete)
					        onComplete(data);
					})
	                .fail(function (data) {
	                    onError(data);
	                });
	        }
	    }

	    var viewModel = kendo.observable({
	        Nodes: [],
	        Sensors: [],
	        UnitSystem: null,

	        update: function () {
	            api.getUnitSystem(function (data) {
	                viewModel.set("UnitSystem", data.Value);
	                api.getNodes(function (data) {
                        viewModel.set("Nodes", data);
                        api.getSensors(function (data) {
                            viewModel.set("Sensors", data);
                        });
                    });
	            });
	        }
	    });
	    viewModel.bind("set", onViewModelBeforeSet);
	    viewModel.bind("change", onViewModelAfterSet);

	    function onError(data) {
	        //debugger;
	        //alert(data.responseJSON.ExceptionMessage);
	        //alert(data.statusText);
	        alert(data.responseText);
	    }
	    function onViewModelBeforeSet(e) {
	    }
	    function onViewModelAfterSet(e) {
	        switch (e.field) {
	            case "UnitSystem":
	                if (e.action == "itemchange") {
	                    debugger;
	                }
	                break;
	            case "Nodes":
	                if (e.action == "itemchange") {
	                    var item = e.items[0];
	                    api.setNodeName(item.Id, item.Name, function () { });
	                }
	                else if (e.action == "remove") {
	                    var item = e.items[0];
	                    api.deleteNode(item.Id);
	                }
	                break;
	            case "Sensors":
	                if (e.action == "itemchange") {
	                    var item = e.items[0];
	                    api.setSensorName(item.Id, item.Name, function () { });
	                }
	                else if (e.action == "remove") {
	                    var item = e.items[0];
	                    api.deleteSensor(item.Id);
	                }
                    break;
	            default:
	                break;
	        }
	    }

	    function onSignal(data) {
	        switch (data.MsgId)
	        {
	            case "NodePresentation": onNodePresentation(data); break;
	            case "NodeNameChanged": onNodeNameChanged(data); break;
	            case "NodeDeleted": onNodeDeleted(data); break;
	            case "BatteryLevel": onBatteryLevel(data); break;
	            case "SensorPresentation": onSensorPresentation(data); break;
	            case "SensorNameChanged": onSensorNameChanged(data); break;
	            case "SensorDeleted": onSensorDeleted(data); break;
	            case "SensorValue": onSensorValue(data); break;

	            default:
	                break;
	        }
	    }
	    function onNodePresentation(data) {
	        for (var i = 0; i < viewModel.Nodes.length; i++) {
	            if (viewModel.Nodes[i].Id == data.Value.Id) {
	                viewModel.Nodes[i].set("NodeNo", data.Value.NodeNo);
	                viewModel.Nodes[i].set("TypeName", data.Value.TypeName);
	                viewModel.Nodes[i].set("ProtocolVersion", data.Value.ProtocolVersion);
	                viewModel.Nodes[i].set("SketchName", data.Value.SketchName);
	                viewModel.Nodes[i].set("SketchVersion", data.Value.SketchVersion);
	                viewModel.Nodes[i].set("Name", data.Value.Name);
	                viewModel.Nodes[i].set("BatteryLevel", data.Value.BatteryLevel);
	                return;
	            }
	        }

	        viewModel.Nodes.push(data.Value);
	    }
	    function onNodeNameChanged(data) {
	        for (var i = 0; i < viewModel.Nodes.length; i++) {
	            if (viewModel.Nodes[i].Id == data.Id) {
	                viewModel.Nodes[i].set("Name", data.Name);
	                break;
	            }
	        }
	    }
	    function onNodeDeleted(data) {
	        var nodeNo = null;
	        for (var i = 0; i < viewModel.Nodes.length; i++) {
	            if (viewModel.Nodes[i].Id == data.Id) {
	                nodeNo = viewModel.Nodes[i].NodeNo;
	                viewModel.Nodes.splice(i, 1);
	                break;
	            }
	        }

	        if (nodeNo) {
	            for (var i = 0; i < viewModel.Sensors.length;) {
	                if (viewModel.Sensors[i].NodeNo == nodeNo)
	                    viewModel.Sensors.splice(i, 1);
	                else
	                    i++;
	            }
	        }
	    }
	    function onBatteryLevel(data) {
	        for (var i = 0; i < viewModel.Nodes.length; i++) {
	            if (viewModel.Nodes[i].NodeNo == data.Value.NodeNo) {
	                viewModel.Nodes[i].set("BatteryLevel", data.Value);
	                break;
	            }
	        }
	    }
	    function onSensorPresentation(data) {
	        for (var i = 0; i < viewModel.Sensors.length; i++) {
	            if (viewModel.Sensors[i].Id == data.Value.Id) {
	                viewModel.Sensors[i].set("NodeNo", data.Value.NodeNo);
	                viewModel.Sensors[i].set("SensorNo", data.Value.SensorNo);
	                viewModel.Sensors[i].set("TypeName", data.Value.TypeName);
	                viewModel.Sensors[i].set("ProtocolVersion", data.Value.ProtocolVersion);
	                viewModel.Sensors[i].set("Name", data.Value.Name);
	                viewModel.Sensors[i].set("SensorValue", data.Value.SensorValue);
	                return;
	            }
	        }

	        viewModel.Sensors.push(data.Value);
	    }
	    function onSensorNameChanged(data) {
	        for (var i = 0; i < viewModel.Sensors.length; i++) {
	            if (viewModel.Sensors[i].Id == data.Id) {
	                viewModel.Sensors[i].set("Name", data.Name);
	                break;
	            }
	        }
	    }
	    function onSensorDeleted(data) {
	        for (var i = 0; i < viewModel.Sensors.length; i++) {
	            if (viewModel.Sensors[i].Id == data.Id) {
	                viewModel.Sensors.splice(i, 1);
	                break;
	            }
	        }
	    }
	    function onSensorValue(data) {
	        console.log(data.Value.Type + ": " + data.Value.Value);
	        for (var i = 0; i < viewModel.Sensors.length; i++) {
	            if (viewModel.Sensors[i].NodeNo == data.Value.NodeNo && viewModel.Sensors[i].SensorNo == data.Value.SensorNo) {
	                viewModel.Sensors[i].set("SensorValue", data.Value);
	                break;
	            }
	        }
        }

	    return {
	        start: function () {
                application.SignalRReceiveHandlers.push(onSignal);

                var layoutView = new views.layoutView();
                application.setContentView(layoutView);

                kendo.bind($("#content"), viewModel);
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