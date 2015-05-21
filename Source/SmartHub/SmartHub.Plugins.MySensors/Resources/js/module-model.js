
define(['jquery'], function ($) {
	var api = {
	    getNodes: function (onComplete) {
	        $.getJSON('/api/mysensors/nodes')
				.done(function (data) {
					$.each(data, function (idx, item) {
					    if (item.BatteryLevelTimeStamp)
                            item.BatteryLevelTimeStamp = new Date(item.BatteryLevelTimeStamp);
					});

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

	    getSensors: function (onComplete) {
	        $.getJSON('/api/mysensors/sensors')
				.done(function (data) {
					$.each(data, function (idx, item) {
					    if (item.SensorValueTimeStamp)
					        item.SensorValueTimeStamp = new Date(item.SensorValueTimeStamp);
					});

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
	    },
	    setUnitSystem: function (value, onComplete) {
	        $.post('/api/mysensors/setunitsystem', { Value: value })
				.done(function (data) {
					if (onComplete)
					    onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
	    },

	    getBatteryLevels: function (onComplete) {
	        $.getJSON('/api/mysensors/batterylevels')
				.done(function (data) {
					$.each(data, function (idx, item) { item.TimeStamp = new Date(item.TimeStamp); });

					if (onComplete)
					    onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
	    },
	    getSensorValues: function (onComplete) {
	        $.getJSON('/api/mysensors/sensorvalues')
				.done(function (data) {
					$.each(data, function (idx, item) { item.TimeStamp = new Date(item.TimeStamp); });

					if (onComplete)
					    onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
	    },
	};

	var viewModel = kendo.observable({
	    UnitSystem: "M",
	    Nodes: [],
	    Sensors: [],
	    BatteryLevels: [],
	    SensorValues: [],

        update: function (onComplete) {
            var me = this;

	        api.getUnitSystem(function (data) {
	            me.set("UnitSystem", data.Value);

	            api.getNodes(function (data) {
	                me.set("Nodes", data);

	                api.getSensors(function (data) {
	                    me.set("Sensors", data);

	                    api.getBatteryLevels(function (data) {
	                        me.set("BatteryLevels", data);

	                        api.getSensorValues(function (data) {
	                            me.set("SensorValues", data);

	                            if (onComplete)
	                                onComplete();
	                        });
	                    });
                    });
                });
	        });
	    },
        SignalRReceiveHandler: function (model, data) {
            var me = viewModel; // not "this" because is called from another context!!!
            //var me = model;

	        switch (data.MsgId) {
	            case "NodePresentation": onNodePresentation(data); break;
	            case "NodeNameChanged": onNodeNameChanged(data); break;
	            case "NodeDeleted": onNodeDeleted(data); break;
	            case "BatteryLevel": onBatteryLevel(data); break;
	            case "SensorPresentation": onSensorPresentation(data); break;
	            case "SensorNameChanged": onSensorNameChanged(data); break;
	            case "SensorDeleted": onSensorDeleted(data); break;
	            case "SensorValue": onSensorValue(data); break;
	            case "UnitSystemChanged": onUnitSystemChanged(data); break;
	            default: break;
	        }

	        function onNodePresentation(data) {
	            for (var i = 0; i < me.Nodes.length; i++) {
	                if (me.Nodes[i].Id == data.Data.Id) {
	                    me.Nodes[i].set("NodeNo", data.Data.NodeNo);
	                    me.Nodes[i].set("TypeName", data.Data.TypeName);
	                    me.Nodes[i].set("ProtocolVersion", data.Data.ProtocolVersion);
	                    me.Nodes[i].set("SketchName", data.Data.SketchName);
	                    me.Nodes[i].set("SketchVersion", data.Data.SketchVersion);
	                    me.Nodes[i].set("Name", data.Data.Name);
	                    me.Nodes[i].set("BatteryLevel", data.Data.BatteryLevel);
	                    return;
	                }
	            }

	            me.Nodes.push(data.Data);
	        }
	        function onNodeNameChanged(data) {
	            for (var i = 0; i < me.Nodes.length; i++) {
	                if (me.Nodes[i].Id == data.Data.Id) {
	                    me.Nodes[i].set("Name", data.Data.Name);
	                    break;
	                }
	            }
	        }
	        function onNodeDeleted(data) {
	            var nodeNo = null;
	            for (var i = 0; i < me.Nodes.length; i++) {
	                if (me.Nodes[i].Id == data.Data.Id) {
	                    nodeNo = me.Nodes[i].NodeNo;
	                    me.Nodes.splice(i, 1);
	                    break;
	                }
	            }

	            if (nodeNo) {
	                for (var i = 0; i < me.Sensors.length;) {
	                    if (me.Sensors[i].NodeNo == nodeNo)
	                        me.Sensors.splice(i, 1);
	                    else
	                        i++;
	                }
	            }
	        }
	        function onBatteryLevel(data) {
	            data.Data.TimeStamp = new Date(data.Data.TimeStamp);

	            for (var i = 0; i < me.Nodes.length; i++) {
	                if (me.Nodes[i].NodeNo == data.Data.NodeNo) {
	                    me.Nodes[i].set("BatteryLevelLevel", data.Data.Level);
	                    me.Nodes[i].set("BatteryLevelTimeStamp", data.Data.TimeStamp);
	                    break;
	                }
	            }

	            me.BatteryLevels.push(data.Data);
	        }
	        function onSensorPresentation(data) {
	            for (var i = 0; i < me.Sensors.length; i++) {
	                if (me.Sensors[i].Id == data.Data.Id) {
	                    me.Sensors[i].set("NodeNo", data.Data.NodeNo);
	                    me.Sensors[i].set("SensorNo", data.Data.SensorNo);
	                    me.Sensors[i].set("TypeName", data.Data.TypeName);
	                    me.Sensors[i].set("ProtocolVersion", data.Data.ProtocolVersion);
	                    me.Sensors[i].set("Name", data.Data.Name);
	                    me.Sensors[i].set("SensorValue", data.Data.SensorValue);
	                    return;
	                }
	            }

	            me.Sensors.push(data.Data);
	        }
	        function onSensorNameChanged(data) {
	            for (var i = 0; i < me.Sensors.length; i++) {
	                if (me.Sensors[i].Id == data.Data.Id) {
	                    me.Sensors[i].set("Name", data.Data.Name);
	                    break;
	                }
	            }
	        }
	        function onSensorDeleted(data) {
	            for (var i = 0; i < me.Sensors.length; i++) {
	                if (me.Sensors[i].Id == data.Data.Id) {
	                    me.Sensors.splice(i, 1);
	                    break;
	                }
	            }
	        }
	        function onSensorValue(data) {
	            console.log(data.Data.Type + ": " + data.Data.Value);

	            data.Data.TimeStamp = new Date(data.Data.TimeStamp);

	            for (var i = 0; i < me.Sensors.length; i++) {
	                if (me.Sensors[i].NodeNo == data.Data.NodeNo && me.Sensors[i].SensorNo == data.Data.SensorNo) {
	                    me.Sensors[i].set("SensorValueValue", data.Data.Value);
	                    me.Sensors[i].set("SensorValueTimeStamp", data.Data.TimeStamp);
	                    break;
	                }
	            }

	            me.SensorValues.push(data.Data);
	        }
	        function onUnitSystemChanged(data) {
	            me.set("UnitSystem", data.Data);
	        }
	    }
	});

	function onError(data) {
	    //alert(data.responseJSON.ExceptionMessage);
	    //alert(data.statusText);
	    alert(data.responseText);
	}

	return {
	    ViewModel: viewModel,

	    getNodes: api.getNodes,
	    setNodeName: api.setNodeName,
	    deleteNode: api.deleteNode,
	    getSensors: api.getSensors,
	    setSensorName: api.setSensorName,
	    deleteSensor: api.deleteSensor,
	    getUnitSystem: api.getUnitSystem,
	    setUnitSystem: api.setUnitSystem,
	    getBatteryLevels: api.getBatteryLevels,
	    getSensorValues: api.getSensorValues
	};
});