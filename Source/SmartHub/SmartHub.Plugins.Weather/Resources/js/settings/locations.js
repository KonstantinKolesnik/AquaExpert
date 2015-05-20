﻿define(
	['app', 'common',
		'webapp/weather/locations-model',
		'webapp/weather/locations-view'],
	function (application, common, models, views) {

		var layoutView;

		var api = {

			addWeatherTile: function (view) {

				var locationId = view.model.get('id');
				application.addTile('SmartHub.Plugins.Weather.WeatherTile', { cityId: locationId });
			},

			addLocation: function () {

				var displayName = this.model.get('displayName');
				var query = this.model.get('query');

				if (displayName && query) {

					models.addLocation(displayName, query).done(api.reloadList);
				}
			},

			deleteLocation: function (childView) {

				var displayName = childView.model.get('displayName');

				if (common.utils.confirm('Удалить локацию "{0}" и все ее данные?', displayName)) {

					var locationId = childView.model.get('id');

					models.deleteLocation(locationId).done(api.reloadList);
				}
			},

			updateLocation: function (childView) {

				var locationId = childView.model.get('id');

				childView.showSpinner();

				models.updateLocation(locationId)
					.done(function () {
						childView.hideSpinner();
					});
			},

			reloadForm: function () {

				var formData = new models.Location();

				var form = new views.WeatherSettingsFormView({ model: formData });
				form.on('weather:location:add', api.addLocation);
				layoutView.regionForm.show(form);
			},

			reloadList: function () {

				models.loadLocations()
					.done(function (list) {

						var view = new views.LocationListView({ collection: list });
						view.on('childview:weather:location:delete', api.deleteLocation);
						view.on('childview:weather:location:update', api.updateLocation);
						view.on('childview:weather:location:add-tile', api.addWeatherTile);

						layoutView.regionList.show(view);
					});
			}
		};

		return {
			start: function () {

				// init layout
				layoutView = new views.WeatherSettingsLayout();
				application.setContentView(layoutView);

				api.reloadForm();
				api.reloadList();
			}
		};
	});