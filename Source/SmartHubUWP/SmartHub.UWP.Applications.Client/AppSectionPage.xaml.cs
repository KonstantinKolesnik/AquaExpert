using SmartHub.UWP.Applications.Client.Common;
using SmartHub.UWP.Core;
using SmartHub.UWP.Plugins.UI.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace SmartHub.UWP.Applications.Client
{
    public sealed partial class AppSectionPage : Page
    {
        #region Properties
        public AppSectionItemAttribute SelectedItem
        {
            get { return LocalUtils.AppState.IsAppsSection ? LocalUtils.AppState.SelectedItemApps : LocalUtils.AppState.SelectedItemSystem; }
            set
            {
                if (LocalUtils.AppState.IsAppsSection)
                    LocalUtils.AppState.SelectedItemApps = value;
                else
                    LocalUtils.AppState.SelectedItemSystem = value;
            }
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

            if (e.Parameter != null)
                LocalUtils.AppState.IsAppsSection = (AppSectionType)e.Parameter == AppSectionType.Applications;

            AppShell.Current.SetNavigationInfo(LocalUtils.AppState.IsAppsSection ? "Applications" : "System", LocalUtils.AppState.IsAppsSection ? "menuApplications" : "menuSystem");
            AppShell.Current.SetPrimaryBackRequestHandler(OnBackRequested);
            UpdateForVisualState(AdaptiveStates.CurrentState);

            await UpdateList();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            AppShell.Current.SetPrimaryBackRequestHandler(null);
            base.OnNavigatedFrom(e);
        }
        #endregion

        #region Event handlers
        private void AdaptiveStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            UpdateForVisualState(e.NewState, e.OldState);
        }
        private void lvItems_ItemClicked(object sender, AppSectionItemEventArgs e)
        {
            if (SelectedItem != e.Item)
            {
                SelectedItem = e.Item;
                DetailContentPresenter.Content = Activator.CreateInstance(e.Item.UIModuleType);
            }

            if (AdaptiveStates.CurrentState == NarrowState)
            {
                // hide master panel, show only details panel:
                MasterColumn.Width = new GridLength(0);
                DetailColumn.Width = new GridLength(1, GridUnitType.Star);
            }
        }
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!e.Handled && AdaptiveStates.CurrentState == NarrowState && MasterColumn.Width.Value == 0) // narrow details view
            {
                // hide details panel, show only master panel:
                MasterColumn.Width = new GridLength(1, GridUnitType.Star);
                DetailColumn.Width = new GridLength(0);

                SelectedItem = null;
                DetailContentPresenter.Content = null;

                e.Handled = true;
            }
        }
        #endregion

        #region Private methods
        private void UpdateForVisualState(VisualState newState, VisualState oldState = null)
        {
            var isNarrow = newState == NarrowState;

            if (!isNarrow)
            {
                MasterColumn.Width = (GridLength) Application.Current.Resources["MasterColumnWidth"];
                DetailColumn.Width = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                MasterColumn.Width = SelectedItem != null ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                DetailColumn.Width = SelectedItem != null ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
            }

            EntranceNavigationTransitionInfo.SetIsTargetElement(lvItems, isNarrow);
            if (DetailContentPresenter != null)
                EntranceNavigationTransitionInfo.SetIsTargetElement(DetailContentPresenter, !isNarrow);
        }
        private async Task UpdateList()
        {
            var items = await CoreUtils.RequestAsync<List<AppSectionItemAttribute>>(LocalUtils.AppState.IsAppsSection ? "/api/ui/sections/apps" : "/api/ui/sections/system");

            Items.Clear();
            if (items != null)
                foreach (var item in items)
                {
                    Items.Add(item);
                    if (SelectedItem != null && SelectedItem.UIModuleType == item.UIModuleType)
                        DetailContentPresenter.Content = Activator.CreateInstance(SelectedItem.UIModuleType);
                }
        }
        #endregion
    }
}
