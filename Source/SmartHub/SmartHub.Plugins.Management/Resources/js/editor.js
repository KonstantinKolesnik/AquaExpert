
define(
    ['app', 'webapp/management/editor-model', 'webapp/management/editor-view'],
    function (application, models, views) {
        var module = {
            reload: function (id) {
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