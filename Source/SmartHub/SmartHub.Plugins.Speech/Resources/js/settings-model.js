
define(['jquery'], function ($) {
    var api = {
        addVoiceCommand: function (text, scriptId, onComplete) {
            $.post('/api/speech/voicecommand/add', { text: text, scriptId: scriptId })
				.done(function (data) {
				    if (onComplete)
				        onComplete(data);
				})
	            .fail(function (data) {
	                onError(data);
	            });
        },
        setVoiceCommandText: function (id, text, onComplete) {
            $.post('/api/speech/voicecommand/settext', { id: id, text: text })
                .done(function (data) {
                    if (onComplete)
                        onComplete(data);
                })
                .fail(function (data) {
                    onError(data);
                });
        },
        deleteVoiceCommand: function (id, onComplete) {
            $.post('/api/speech/voicecommand/delete', { id: id })
                .done(function (data) {
                    if (onComplete)
                        onComplete(data);
                })
                .fail(function (data) {
                    onError(data);
                });
        },
    };

    var viewModel = kendo.observable({ });

    function onError(data) {
        alert(data.statusText);
    }

    return {
        ViewModel: viewModel,

        addVoiceCommand: api.addVoiceCommand,
        setVoiceCommandText: api.setVoiceCommandText,
        deleteVoiceCommand: api.deleteVoiceCommand
    };
});