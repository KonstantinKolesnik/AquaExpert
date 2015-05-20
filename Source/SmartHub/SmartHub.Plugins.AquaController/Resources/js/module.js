define(
	['app', 'jquery', 'webapp/aquacontroller/views'],
	function (application, $, views) {












	    function onError(data) {
	        //alert(data.responseJSON.ExceptionMessage);
	        //alert(data.statusText);
	        alert(data.responseText);
	    }

	    return {
	        start: function () {
	            //application.SignalRReceiveHandlers.push(viewModel.onSignalRReceived);

	            var layoutView = new views.layoutView();
	            application.setContentView(layoutView);

	            //layoutView.bind(viewModel);
	            //viewModel.update();




	        }
	    };
	});