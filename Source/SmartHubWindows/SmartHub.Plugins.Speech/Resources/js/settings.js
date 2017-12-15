define(
	['app', 'webapp/speech/settings-model', 'webapp/speech/settings-view'],
	function (application, models, views) {
	    var view;

	    var module = {
	        addVoiceCommand: function (text, scriptId) {
	            if (!text) {
	                alert("Не указан текст команды!");
	                return;
	            }
	            if (!scriptId) {
	                alert("Не указан скрипт команды!");
	                return;
	            }

	            models.addVoiceCommand(text, scriptId, function () { view.refreshGrid(); });
	        },
	        setVoiceCommandText: function (id, text) {
	            if (!text) {
	                alert("Не указан текст команды!");
	                return;
	            }

	            models.setVoiceCommandText(id, text, function () { view.refreshGrid(); });
	        },
	        deleteVoiceCommand: function (id) {
	            models.deleteVoiceCommand(id, function () { view.refreshGrid(); });
	        },

	        reload: function () {
	            view = new views.LayoutView({ viewModel: models.ViewModel });
	            view.on('voiceCommand:add', module.addVoiceCommand);
	            view.on('voiceCommand:setText', module.setVoiceCommandText);
	            view.on('voiceCommand:delete', module.deleteVoiceCommand);
	            application.setContentView(view);
	        }
	    };

	    return {
	        start: function () {
	            module.reload();
	        }
	    };
	});