
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

	var SensorType = {
	    Door: 0,      // Door and window sensors
	    Motion: 1,      // Motion sensors
	    Smoke: 2,      // Smoke sensor
	    //Light: 3,      // Light Actuator (on/off)
	    Switch: 3,      // Light Actuator (on/off)
	    Dimmer: 4,      // Dimmable device of some kind
	    Cover: 5,      // Window covers or shades
	    Temperature: 6,      // Temperature sensor
	    Humidity: 7,      // Humidity sensor
	    Barometer: 8,      // Barometer sensor (Pressure)
	    Wind: 9,      // Wind sensor
	    Rain: 10,     // Rain sensor
	    UV: 11,     // UV sensor
	    Weight: 12,     // Weight sensor for scales etc.
	    Power: 13,     // Power measuring device, like power meters
	    Heater: 14,     // Heater device
	    Distance: 15,     // Distance sensor
	    LightLevel: 16,     // Light sensor
	    /*ArduinoNode*/Device: 17,     // Arduino node device
	    /*ArduinoRelay*/Repeater: 18,     // Arduino repeating node device
	    Lock: 19,     // Lock device
	    IR: 20,     // IR sender/receiver device
	    Water: 21,     // Water meter
	    AirQuality: 22,     // Air quality sensor e.g. MQ-2
	    Custom: 23,     // Use this for custom sensors where no other fits.
	    Dust: 24,     // Dust level sensor
	    SceneController: 25,      // Scene controller device
	    PH: 26,      // PH sensor
	    ORP: 27,      // ORP sensor
	};

	var SensorValueType = {
	    Temperature: 0,      // Temperature
	    Humidity: 1,      // Humidity
	    Switch: 2,      // Light status. 0=off 1=on
	    Dimmer: 3,      // Dimmer value. 0-100%
	    Pressure: 4,      // Atmospheric Pressure
	    Forecast: 5,      // Whether forecast. One of "stable", "sunny", "cloudy", "unstable", "thunderstorm" or "unknown"
	    Rain: 6,      // Amount of rain
	    RainRate: 7,      // Rate of rain
	    Wind: 8,      // Windspeed
	    Gust: 9,      // Gust
	    Direction: 10,     // Wind direction
	    UV: 11,     // UV light level
	    Weight: 12,     // Weight (for scales etc)
	    Distance: 13,     // Distance
	    Impedance: 14,     // Impedance value
	    Armed: 15,     // Armed status of a security sensor. 1=Armed, 0=Bypassed
	    Tripped: 16,     // Tripped status of a security sensor. 1=Tripped, 0=Untripped
	    Watt: 17,     // Watt value for power meters
	    KWH: 18,     // Accumulated number of KWH for a power meter
	    SceneOn: 19,     // Turn on a scene
	    SceneOff: 20,     // Turn of a scene
	    Heater: 21,     // Mode of header. One of "Off", "HeatOn", "CoolOn", or "AutoChangeOver"
	    HeaterSW: 22,     // Heater switch power. 1=On, 0=Off
	    LightLevel: 23,     // Light level. 0-100%
	    Var1: 24,     // Custom value
	    Var2: 25,     // Custom value
	    Var3: 26,     // Custom value
	    Var4: 27,     // Custom value
	    Var5: 28,     // Custom value
	    Up: 29,     // Window covering. Up.
	    Down: 30,     // Window covering. Down.
	    Stop: 31,     // Window covering. Stop.
	    IRSend: 32,     // Send out an IR-command
	    IRReceive: 33,     // This message contains a received IR-command
	    Flow: 34,     // Flow of water (in meter)
	    Volume: 35,     // Water volume
	    LockStatus: 36,     // Set or get lock status. 1=Locked, 0=Unlocked
	    DustLevel: 37,     // Dust level
	    Voltage: 38,     // Voltage level
	    Current: 39,     // Current level
	    PH: 40,     // Ph level
	    ORP: 41,     // ORP level
	};

	var viewModel = kendo.observable({
	    UnitSystem: "M",
	    Nodes: [],
	    Sensors: [],
	    BatteryLevels: [],
	    SensorValues: [],

	    getSensorValueUnit: function (type) {
	        //$.each(SensorValueType, function (idx, t) {
	        //    if (SensorValueType[t] == type)
	        //        return t;
	        //});

	        if (!type)
	            return "";

	        switch (type) {
	            case SensorValueType.Temperature: return viewModel.Settings.UnitSystem == "M" ? "°C" : "°F";
	            case SensorValueType.Humidity: return "%";
	                //case SensorValueType.Switch:                 2,      // Light status. 0=off 1=on
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
	            case SensorValueType.Distance: return viewModel.Settings.UnitSystem == "M" ? "cm" : "in";
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
	    },

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
            var me = model;

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
	            //console.log("["+data.Data.NodeNo+"]["+data.Data.SensorNo+"] "+ data.Data.TypeName + ": " + data.Data.Value);

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