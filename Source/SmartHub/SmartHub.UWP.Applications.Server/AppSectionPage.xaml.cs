using SmartHub.UWP.Applications.Server.Common;
using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Communication.Stream;
using SmartHub.UWP.Plugins.UI.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
        public static AppSectionItemAttribute SelectedItem
        {
            get; set;
        }
        // CoreApplication.Properties["SelectedAppSectionItem"] as AppSectionItemAttribute;
            //if (!CoreApplication.Properties.ContainsKey("SelectedAppSectionItem"))
            //    CoreApplication.Properties["SelectedAppSectionItem"] = null;
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

            isAppsSection = e.Parameter != null ? (AppSectionType) e.Parameter == AppSectionType.Applications : (bool)CoreApplication.Properties[nameof(isAppsSection)];
            CoreApplication.Properties[nameof(isAppsSection)] = isAppsSection;

            AppShell.Current.SetNavigationInfo(isAppsSection ? "Applications" : "System", isAppsSection ? "menuApplications" : "menuSystem");
            AppShell.Current.SetPrimaryBackRequestHandler(OnBackRequested);
            UpdateForVisualState(AdaptiveStates.CurrentState);

            await UpdateList();
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

            if (SelectedItem != e.Item)
            {
                SelectedItem = e.Item;
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

                CoreApplication.Properties["SelectedAppSectionItem"] = null;
                DetailContentPresenter.Content = null;

                e.Handled = true;
            }
        }
        #endregion

        #region Private methods
        private void UpdateForVisualState(VisualState newState, VisualState oldState = null)
        {
            var selectedItem = SelectedItem;
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
        private async Task UpdateList()
        {
            var selectedItem = SelectedItem;

            var items = await StreamClient.RequestAsync<IEnumerable<AppSectionItemAttribute>>(AppManager.RemoteUrl, AppManager.RemoteServiceName, ApiCommandName);

            Items.Clear();
            if (items != null)
                foreach (var item in items)
                {
                    Items.Add(item);

                    if (selectedItem != null && selectedItem.TypeFullName == item.TypeFullName)
                        DetailContentPresenter.Content = Activator.CreateInstance(item.TypeFullName);
                }
        }
        #endregion
    }
}
