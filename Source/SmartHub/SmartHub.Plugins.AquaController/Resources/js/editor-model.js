
define(['jquery'],
    function ($) {
        var api = {
            getController: function (id, onComplete) {
                $.getJSON('/api/aquacontroller/controller', { id: id })
                    .done(function (data) {
                        if (onComplete)
                            onComplete(data);
                    })
                    .fail(function (data) {
                        onError(data);
                    });
            },
            //saveScript: function (model) {

            //    var scriptId = model.get('id');
            //    var scriptName = model.get('name');
            //    var scriptBody = model.get('body');

            //    var rq = lib.$.post('/api/scripts/save', {
            //        id: scriptId,
            //        name: scriptName,
            //        body: scriptBody
            //    });

            //    return rq.promise();
            //}
        };

        var viewModel = null;
        //Switch = 3,             // Switch Actuator (on/off)
        //Temperature = 6,        // Temperature sensor

        //api.getSensorsByType(6, function (data) {
        //    me.set("SensorsTemperatureDataSource", data);

        //    api.getSensorsByType(3, function (data) {
        //        me.set("SensorsSwitchDataSource", data);

        //        api.getHeaterControllerConfiguration(function (data) {
        //            me.set("HeaterControllerConfiguration", data);

        //if (onComplete)
        //    onComplete();
        //        });

        //    });
        //});


        function onError(data) {
            alert(data.statusText);
        }

        return {
            // entities
            //ScriptData: scriptData,

            ViewModel: viewModel,

            // requests
            getController: api.getController,
            //saveScript: api.saveScript
        }
    });