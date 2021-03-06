﻿
define(
    ['app', 'webapp/monitors/monitor-editor-model', 'webapp/monitors/monitor-editor-view'],
    function (application, models, views) {
        var module = {
            reload: function (id) {
                if (application.SignalRReceivers.indexOf(models.ViewModel) == -1)
                    application.SignalRReceivers.push(models.ViewModel);

                models.ViewModel.update(id, function () {
                    var view = new views.LayoutView({ viewModel: models.ViewModel });
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