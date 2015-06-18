
define(['jquery', 'text!webapp/controllers/utils.html'],
    function ($, templates) {
        var api = {
            setControllerIsAutoMode: function (controller, onComplete) {
                $.post('/api/controllers/setisautomode', { id: controller.Id, isAutoMode: controller.IsAutoMode })
                    .done(function (data) {
                        if (onComplete)
                            onComplete(data);
                    })
                    .fail(function (data) {
                        onError(data);
                    });
            },
            setControllerConfiguration: function (controller, onComplete) {
                $.post('/api/controllers/setconfiguration', { id: controller.Id, config: JSON.stringify(controller.Configuration) })
                    .done(function (data) {
                        if (onComplete)
                            onComplete(data);
                    })
                    .fail(function (data) {
                        onError(data);
                    });
            },

            createControllerWidget: function (selector, controller) {
                controller.bind("change", function (e) {
                    if (e.field == "IsAutoMode")
                        api.setControllerIsAutoMode(e.sender);
                    if (e.field == "Configuration")
                        api.setControllerConfiguration(e.sender);
                });

                selector.html($(templates));

                var tmpl = selector.find("#tmpl" + controller.Type);
                if (!tmpl.length)
                    tmpl = selector.find("#tmplError");
                selector.find("#controller-body").html(tmpl.html());

                var fn = null;
                switch (controller.Type) {
                    case 0: fn = initHeaterController; break;
                    case 1: fn = initSwitchController; break;


                }
                if (fn)
                    fn();

                //var ctrl = selector.find(".controller-chart").kendoChart(config).data("kendoChart");
                kendo.bind(selector.find(".controller-view"), controller);
                //return ctrl;

                function initHeaterController() {

                }
                function initSwitchController() {

                }
            }
        };

        return {
            createControllerWidget: api.createControllerWidget
        };
});