
define(
    ['app', 'webapp/zones/zone-model', 'webapp/zones/zone-view'],
    function (application, models, views) {
        var module = {
            runScript: function (id) {
                models.runScript(id, function () { });
            },
            reload: function (id) {
                if (application.SignalRReceivers.indexOf(models.ViewModel) == -1)
                    application.SignalRReceivers.push(models.ViewModel);

                models.ViewModel.update(id, function () {
                    var view = new views.LayoutView({ viewModel: models.ViewModel });
                    view.on('script:run', module.runScript);
                    application.setContentView(view);
                });
            },
        };

        return {
            start: function (id) {
                module.reload(id);
            }
        };
    });