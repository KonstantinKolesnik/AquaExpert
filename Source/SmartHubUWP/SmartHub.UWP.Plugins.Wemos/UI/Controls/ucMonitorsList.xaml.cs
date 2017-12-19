using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Xaml;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using SmartHub.UWP.Plugins.Wemos.Monitors;
using SmartHub.UWP.Plugins.Wemos.Monitors.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Wemos.UI.Controls
{
    public sealed partial class UcMonitorsList : UserControl
    {
        #region Properties
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<WemosMonitorObservable>), typeof(UcMonitorsList), new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));
        public ObservableCollection<WemosMonitorObservable> ItemsSource
        {
            get { return (ObservableCollection<WemosMonitorObservable>) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = d as UcMonitorsList;
            uc.UpdateItemsViewSource();

            var items = e.NewValue as ObservableCollection<WemosMonitorObservable>;
            if (items != null)
                items.CollectionChanged += (s, args) =>
                {
                    uc.UpdateItemsViewSource();

                    if (args.Action == NotifyCollectionChangedAction.Add)
                        uc.ItemAdded?.Invoke(uc, new ObjectEventArgs(args.NewItems[0] as WemosMonitorObservable));
                    else if (args.Action == NotifyCollectionChangedAction.Remove)
                        uc.ItemDeleted?.Invoke(uc, new ObjectEventArgs(args.OldItems[0] as WemosMonitorObservable));
                };
        }

        public static readonly DependencyProperty IsSortedProperty = DependencyProperty.Register("IsSorted", typeof(bool), typeof(UcMonitorsList), new PropertyMetadata(false, new PropertyChangedCallback(OnItemsSourceChanged)));
        public bool IsSorted
        {
            get { return (bool) GetValue(IsSortedProperty); }
            set { SetValue(IsSortedProperty, value); }
        }

        public static readonly DependencyProperty IsGroupedProperty = DependencyProperty.Register("IsGrouped", typeof(bool), typeof(UcMonitorsList), new PropertyMetadata(false, new PropertyChangedCallback(OnItemsSourceChanged)));
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

                    var gg = itemsViewSource.Source as IOrderedEnumerable<IGrouping<string, WemosMonitorObservable>>;
                    foreach (var g in gg)
                        result += g.Count();

                    return result;
                }
                else
                    return (itemsViewSource.Source as IEnumerable<WemosMonitorObservable>).Count();
            }
        }
        #endregion

        #region Events
        public event ObjectEventHandler ItemAdded;
        public event ObjectEventHandler ItemDeleted;
        public event ObjectEventHandler ItemClicked;
        #endregion

        #region Constructor
        public UcMonitorsList()
        {
            InitializeComponent();
            XamlUtils.FindFirstVisualChild<Grid>(this).DataContext = this;
        }
        #endregion

        #region Private methods
        private async Task UpdateLinesList()
        {
            var models = await CoreUtils.RequestAsync<List<WemosLine>>("/api/wemos/lines");
            var items = models != null ? new ObservableCollection<WemosLine>(models.Where(m => m != null)) : null;

            cbLines.ItemsSource = items;
            if (items != null && items.Count > 0)
                cbLines.SelectedItem = items[0];
        }
        private async Task UpdateMonitorsList()
        {
            biRequest.IsActive = true;

            var models = await CoreUtils.RequestAsync<List<WemosMonitorDto>>("/api/wemos/monitors");
            ItemsSource = models != null ? new ObservableCollection<WemosMonitorObservable>(models.Where(m => m != null).Select(m => new WemosMonitorObservable(m))) : null;

            biRequest.IsActive = false;
        }
        private void UpdateItemsViewSource()
        {
            itemsViewSource.IsSourceGrouped = IsGrouped;

            if (ItemsSource != null)
            {
                if (IsGrouped)
                    itemsViewSource.Source = ItemsSource
                        .Where(item => item != null)
                        .OrderBy(item => IsSorted ? item.LineName ?? "" : "")
                        .GroupBy(item => string.IsNullOrEmpty(item.LineName) ? "" : item.LineName.Substring(0, 1).ToUpper())
                        .OrderBy(item => item.Key);
                else
                    itemsViewSource.Source = ItemsSource.OrderBy(item => IsSorted ? item.LineName ?? "" : "");
            }
            else
                itemsViewSource.Source = null;

            tbEmptyContent.Visibility = Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        #region Event handlers
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateMonitorsList();
        }
        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            await UpdateMonitorsList();
        }
        private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            nbMin.Value = 0;
            nbMax.Value = 0;

            await UpdateLinesList();

            var result = await dlgAddMonitor.ShowAsync();
            if (result == ContentDialogResult.Primary && cbLines.SelectedItem != null)
            {
                var lineID = (cbLines.SelectedItem as WemosLine).ID;
                var min = nbMin.Value.Value;
                var max = nbMax.Value.Value;

                var model = await CoreUtils.RequestAsync<WemosMonitorDto>("/api/wemos/monitors/add", lineID, min, max);
                if (model != null)
                    ItemsSource.Add(new WemosMonitorObservable(model));
            }
        }
        private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            await CoreUtils.MessageBoxYesNo(XamlUtils.GetLocalizedString(AppManager.AppData.Language, "confirmDeleteItem"), async (onYes) =>
            {
                var id = (string) ((sender as Button).Tag);

                var res = await CoreUtils.RequestAsync<bool>("/api/wemos/monitors/delete", id);
                if (res)
                    ItemsSource.Remove(ItemsSource.FirstOrDefault(m => m.ID == id));
            });
        }
        private void lvMonitors_ItemClick(object sender, ItemClickEventArgs e)
        {
            ItemClicked?.Invoke(sender, new ObjectEventArgs(e.ClickedItem as WemosMonitorObservable));
        }
        #endregion
    }
}
