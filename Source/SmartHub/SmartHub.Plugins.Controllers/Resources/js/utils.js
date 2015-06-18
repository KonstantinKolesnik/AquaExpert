
define(['jquery', 'text!webapp/controllers/utils.html'],
    function ($, templates) {
        var api = {
            createControllerWidget: function (selector, controller) {
                //$(selector).html("controller");
                //return;

                //var config = this.getDefaultConfiguration();
                //if (controller && controller.Configuration)
                //    //try {
                //    config = JSON.parse(controller.Configuration);
                //    //}
                //    //catch(e) {
                //    //    config = this.getDefaultConfiguration();
                //    //}

                selector.html($(templates));
                //var ctrl = selector.find(".controller-chart").kendoChart(config).data("kendoChart");
                kendo.bind(selector.find(".controller-view"), controller);
                //return ctrl;
            }
        };

        return {
            createControllerWidget: api.createControllerWidget
        };
});