﻿define(
	['app', 'common',
		'webapp/scripts/subscriptions-model',
		'webapp/scripts/subscriptions-view'],
	function (application, common, models, views) {

		var layoutView;

		var api = {

			addSubscription: function () {

				var eventAlias = this.model.get('selectedEventAlias');
				var scriptId = this.model.get('selectedScriptId');

				models.addSubscription(eventAlias, scriptId).done(api.reloadList);
			},

			deleteSubscription: function (childView) {

				var eventAlias = childView.model.get('eventAlias');
				var scriptName = childView.model.get('scriptName');

				if (common.utils.confirm('Удалить подписку?\n- событие: "{0}"\n- скрипт: "{1}"', eventAlias, scriptName)) {

					var subscriptionId = childView.model.get('id');
					models.deleteSubscription(subscriptionId).done(api.reloadList);
				}
			},

			reloadList: function () {

				models.loadSubscriptions()
					.done(function (list) {

						var view = new views.SubscriptionListView({ collection: list });
						view.on('childview:scripts:subscription:delete', api.deleteSubscription);
						layoutView.regionList.show(view);
					});
			},

			reloadForm: function () {

				models.loadFormData()
					.done(function (formData) {

						var form = new views.SubscriptionFormView({ model: formData });
						form.on('scripts:subscription:add', api.addSubscription);
						layoutView.regionForm.show(form);
					});
			}
		};

		return {
			start: function () {

				// init layout
				layoutView = new views.SubscriptionLayout();
				application.setContentView(layoutView);

				api.reloadForm();
				api.reloadList();
			}
		};
	});