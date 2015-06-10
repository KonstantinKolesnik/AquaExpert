
define(['jquery'], function ($) {
    var viewModel = kendo.observable({ });
        //SignalRReceiveHandler: function (model, data) {
        //    if (data.MsgId == "SensorValue") {
        //        data.Data.TimeStamp = new Date(data.Data.TimeStamp);
        //        var sv = data.Data;

        //        if (model.Zone) {
        //            $.each(model.Zone.MonitorsList, function (idx, monitor) {
        //                if (monitor.Sensor.NodeNo == sv.NodeNo && monitor.Sensor.SensorNo == sv.SensorNo) {
        //                    monitor.Sensor.set("SensorValueValue", sv.Value);
        //                    monitor.Sensor.set("SensorValueTimeStamp", sv.TimeStamp);

        //                    monitor.SensorValues.splice(0, 1);
        //                    monitor.SensorValues.push(sv);
        //                }
        //            });

        //        }
        //    }
        //}

    return {
        ViewModel: viewModel
    };
});