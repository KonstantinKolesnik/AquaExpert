
define(['jquery', 'text!webapp/controllers/utils.html'],
    function ($, templates) {
        var api = {
            createControllerWidget: function (selector, controller) {
                $(selector).html("controller");
                return;

                selector.empty();
                selector.html($(templates));

                var config = this.getDefaultConfiguration();
                if (controller && controller.Configuration)
                    //try {
                    config = JSON.parse(controller.Configuration);
                    //}
                    //catch(e) {
                    //    config = this.getDefaultConfiguration();
                    //}

                kendo.bind(selector.find(".controller-view"), controller);

                return selector.find(".controller-chart").kendoChart(config).data("kendoChart");
            }
        };

        return {
            createControllerWidget: api.createControllerWidget
        };
});