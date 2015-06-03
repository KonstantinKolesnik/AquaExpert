
define(
    ['app', 'webapp/aquacontroller/editor-model', 'webapp/aquacontroller/editor-view'],
    function (application, models, views) {
        var module = {
            save: function (data) {
                this.model.set(data);
                models.saveScript(this.model).done(api.redirectToList);
            },
            redirectToList: function () {
                application.navigate('webapp/aquacontroller/settings');
            },
            reload: function (id) {
                models.getController(id, function (data) {
                    models.ViewModel = data;

                    var view = new views.LayoutView();
                    view.on('editor:cancel', module.redirectToList);
                    view.on('editor:save', module.save);
                    application.setContentView(view);

                    view.bindModel(models.ViewModel);
                });
            },
        };

        return {
            start: function (id) {
                module.reload(id);
            }
        };
    });