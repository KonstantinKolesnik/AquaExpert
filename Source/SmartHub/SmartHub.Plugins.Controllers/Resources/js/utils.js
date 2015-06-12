
define(['jquery', 'text!webapp/controllers/utils.html'],
    function ($, templates) {
        var api = {
            createControllerWidget: function (selector, monitor) {
                $(selector).html("controller");
                return;

                selector.empty();
                selector.html($(templates));

                var config = this.getDefaultConfiguration();
                if (monitor && monitor.Configuration)
                    //try {
                    config = JSON.parse(monitor.Configuration);
                    //}
                    //catch(e) {
                    //    config = this.getDefaultConfiguration();
                    //}

                kendo.bind(selector.find(".monitor-view"), monitor);

                return selector.find(".monitor-chart").kendoChart(config).data("kendoChart");
            }
        };

        return {
            createControllerWidget: api.createControllerWidget
        };
});