
define(
	['common', 'lib', 'text!webapp/zones/dashboard.html'],
    function (common, lib, templates) {
        var layoutView = lib.marionette.LayoutView.extend({
            template: lib._.template(templates),
            events: {
                'click .zone': 'zoneClick'
            },
            zoneClick: function (e) {
                e.preventDefault();
                var id = $(e.currentTarget).attr("zoneId");
                this.trigger('zone:show', id);
            },

            onShow: function () {
                createZonesList();
                kendo.bind($("#content"), this.options.viewModel);

                function createZonesList() {
                    $("#lvZones").kendoListView({
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: {
                                    url: function () { return document.location.origin + "/api/zones/list" },
                                }
                            }
                        })
                    }).data("kendoListView");
                }
            }
        });

        return {
            LayoutView: layoutView
        };
    });