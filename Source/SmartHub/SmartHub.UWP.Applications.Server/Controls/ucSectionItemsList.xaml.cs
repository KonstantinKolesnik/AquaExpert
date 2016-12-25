using SmartHub.UWP.Applications.Server.Common;
using SmartHub.UWP.Core;
using SmartHub.UWP.Plugins.UI.Attributes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Applications.Server.Controls
{
    public sealed partial class ucSectionItemsList : UserControl
    {
        #region Properties
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<AppSectionItemAttribute>), typeof(ucSectionItemsList), new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));
        public ObservableCollection<AppSectionItemAttribute> ItemsSource
        {
            get { return (ObservableCollection<AppSectionItemAttribute>) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = d as ucSectionItemsList;
            uc.UpdateItemsSource();

            if (e.NewValue is ObservableCollection<AppSectionItemAttribute>)
                (e.NewValue as ObservableCollection<AppSectionItemAttribute>).CollectionChanged += (s, args) => { uc.UpdateItemsSource(); };
        }

        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register("SelectionMode", typeof(ListViewSelectionMode), typeof(ucSectionItemsList), new PropertyMetadata(ListViewSelectionMode.Single, new PropertyChangedCallback(OnSelectionModeChanged)));
        public ListViewSelectionMode SelectionMode
        {
            get { return (ListViewSelectionMode) GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }
        private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = d as ucSectionItemsList;
            uc.SetSelectedItem();
        }

        public static readonly DependencyProperty IsSortedProperty = DependencyProperty.Register("IsSorted", typeof(bool), typeof(ucSectionItemsList), new PropertyMetadata(false, new PropertyChangedCallback(OnItemsSourceChanged)));
        public bool IsSorted
        {
            get { return (bool) GetValue(IsSortedProperty); }
            set { SetValue(IsSortedProperty, value); }
        }

        public static readonly DependencyProperty IsGroupedProperty = DependencyProperty.Register("IsGrouped", typeof(bool), typeof(ucSectionItemsList), new PropertyMetadata(false, new PropertyChangedCallback(OnItemsSourceChanged)));
        public bool IsGrouped
        {
            get { return (bool) GetValue(IsGroupedProperty); }
            set { SetValue(IsGroupedProperty, value); }
        }

        public int Count
        {
            get
            {
                if (itemsViewSource.Source == null)
                    return 0;
                else if (IsGrouped)
                {
                    int result = 0;

                    var gg = itemsViewSource.Source as IOrderedEnumerable<IGrouping<string, AppSectionItemAttribute>>;
                    foreach (var g in gg)
                        result += g.Count();

                    return result;
                }
                else
                    return (itemsViewSource.Source as IEnumerable<AppSectionItemAttribute>).Count();
            }
        }
        #endregion

        #region Events
        public event AppSectionItemEventHandler ItemClicked;
        #endregion

        #region Constructor
        public ucSectionItemsList()
        {
            InitializeComponent();
            Utils.FindFirstVisualChild<Grid>(this).DataContext = this;
        }
        #endregion

        #region Private methods
        private void UpdateItemsSource()
        {
            itemsViewSource.IsSourceGrouped = IsGrouped;

            if (ItemsSource != null)
            {
                if (IsGrouped)
                    itemsViewSource.Source = ItemsSource
                        .OrderBy(item => IsSorted ? item.Title : "")
                        .GroupBy(item => item.Title.Substring(0, 1).ToUpper())
                        .OrderBy(item => item.Key);
                else
                    itemsViewSource.Source = ItemsSource.OrderBy(item => IsSorted ? item.Title : "");
            }
            else
                itemsViewSource.Source = null;

            tbEmptyContent.Visibility = Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            SetSelectedItem();
        }
        private void SetSelectedItem()
        {
            var selectedItem = SelectionMode == ListViewSelectionMode.Single ? (CoreApplication.Properties.ContainsKey("SelectedAppSectionItem") ? CoreApplication.Properties["SelectedAppSectionItem"] as AppSectionItemAttribute : null) : null;
            if (lvItems.SelectedItem != selectedItem)
            {
                lvItems.SelectedItem = selectedItem;
                if (selectedItem != null)
                    lvItems.ScrollIntoView(selectedItem);
            }
        }
        #endregion

        #region Event handlers
        private void UserControl_LayoutUpdated(object sender, object e)
        {
            SetSelectedItem();
        }
        private void lvItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            ItemClicked?.Invoke(sender, new AppSectionItemEventArgs(e.ClickedItem as AppSectionItemAttribute));
        }
        #endregion
    }
}
