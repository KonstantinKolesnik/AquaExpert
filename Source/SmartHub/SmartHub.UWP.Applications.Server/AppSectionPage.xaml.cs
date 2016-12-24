using SmartHub.UWP.Applications.Server.Common;
using SmartHub.UWP.Plugins.UI.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace SmartHub.UWP.Applications.Server
{
    public sealed partial class AppSectionPage : Page
    {
        #region Fields
        private bool isAppsSection;
        #endregion

        #region Properties
        private string ApiCommandName
        {
            get { return isAppsSection ? "/api/ui/sections/apps" : "/api/ui/sections/system"; }
        }
        public ObservableCollection<AppSectionItemAttribute> Items
        {
            get;
        } = new ObservableCollection<AppSectionItemAttribute>();
        #endregion

        #region Constructor
        public AppSectionPage()
        {
            InitializeComponent();
            DataContext = this;
        }
        #endregion

        #region Navigation
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            isAppsSection = (AppSectionType) (e.Parameter != null ? e.Parameter : CoreApplication.Properties[nameof(isAppsSection)]) == AppSectionType.Applications;
            CoreApplication.Properties[nameof(isAppsSection)] = isAppsSection;

            AppShell.Current.SetNavigationInfo(isAppsSection ? "Applications" : "System", isAppsSection ? "menuApplications" : "menuSystem");
            AppShell.Current.SetPrimaryBackRequestHandler(OnBackRequested);
            UpdateForVisualState(AdaptiveStates.CurrentState);

            //if (!CoreApplication.Properties.ContainsKey(isAppsSection ? "ApplicationItems" : "SystemItems"))
            {
                var items = await AppShell.Current.ApiClient.RequestAsync<IEnumerable<AppSectionItemAttribute>>(ApiCommandName);

                Items.Clear();
                foreach (var item in items)
                {
                    Items.Add(item);
                    if (AppShell.Current.SelectedAppSectionItem?.TypeFullName == item.TypeFullName)
                        DetailContentPresenter.Content = Activator.CreateInstance(item.TypeFullName);
                }
            }
        }
        #endregion

        #region Event handlers
        private void AdaptiveStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            UpdateForVisualState(e.NewState, e.OldState);
        }
        private void lvItems_ItemClicked(object sender, AppSectionItemEventArgs e)
        {
            if (AdaptiveStates.CurrentState == NarrowState)
            {
                // hide master panel, show only details panel:
                MasterColumn.Width = new GridLength(0);
                DetailColumn.Width = new GridLength(1, GridUnitType.Star);
            }

            if (AppShell.Current.SelectedAppSectionItem != e.Item)
            {
                AppShell.Current.SelectedAppSectionItem = e.Item;
                DetailContentPresenter.Content = Activator.CreateInstance(e.Item.TypeFullName);
            }
        }
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!e.Handled && AdaptiveStates.CurrentState == NarrowState && MasterColumn.Width.Value == 0) // narrow details view
            {
                // hide details panel, show only master panel:
                MasterColumn.Width = new GridLength(1, GridUnitType.Star);
                DetailColumn.Width = new GridLength(0);

                AppShell.Current.SelectedAppSectionItem = null;
                DetailContentPresenter.Content = null;

                e.Handled = true;
            }
        }
        #endregion

        #region Private methods
        private void UpdateForVisualState(VisualState newState, VisualState oldState = null)
        {
            var selectedItem = AppShell.Current.SelectedAppSectionItem;
            var isNarrow = newState == NarrowState;

            if (!isNarrow)
            {
                MasterColumn.Width = (GridLength) Application.Current.Resources["MasterColumnWidth"];
                DetailColumn.Width = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                MasterColumn.Width = selectedItem != null ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                DetailColumn.Width = selectedItem != null ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
            }

            EntranceNavigationTransitionInfo.SetIsTargetElement(lvItems, isNarrow);
            if (DetailContentPresenter != null)
                EntranceNavigationTransitionInfo.SetIsTargetElement(DetailContentPresenter, !isNarrow);
        }
        #endregion
    }
}
