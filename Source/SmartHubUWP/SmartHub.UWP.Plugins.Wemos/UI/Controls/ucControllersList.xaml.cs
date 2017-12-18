using SmartHub.UWP.Core;
using SmartHub.UWP.Core.StringResources;
using SmartHub.UWP.Core.Xaml;
using SmartHub.UWP.Plugins.Wemos.Controllers.Models;
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
    public sealed partial class ucControllersList : UserControl
    {
        #region Properties
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<WemosControllerObservable>), typeof(ucControllersList), new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));
        public ObservableCollection<WemosControllerObservable> ItemsSource
        {
            get { return (ObservableCollection<WemosControllerObservable>) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = d as ucControllersList;
            uc.UpdateItemsViewSource();

            var items = e.NewValue as ObservableCollection<WemosControllerObservable>;
            if (items != null)
                items.CollectionChanged += (s, args) =>
                {
                    uc.UpdateItemsViewSource();

                    if (args.Action == NotifyCollectionChangedAction.Add)
                        uc.ItemAdded?.Invoke(uc, new ObjectEventArgs(args.NewItems[0] as WemosControllerObservable));
                    else if (args.Action == NotifyCollectionChangedAction.Remove)
                        uc.ItemDeleted?.Invoke(uc, new ObjectEventArgs(args.OldItems[0] as WemosControllerObservable));
                };
        }

        public static readonly DependencyProperty IsSortedProperty = DependencyProperty.Register("IsSorted", typeof(bool), typeof(ucControllersList), new PropertyMetadata(false, new PropertyChangedCallback(OnItemsSourceChanged)));
        public bool IsSorted
        {
            get { return (bool) GetValue(IsSortedProperty); }
            set { SetValue(IsSortedProperty, value); }
        }

        public static readonly DependencyProperty IsGroupedProperty = DependencyProperty.Register("IsGrouped", typeof(bool), typeof(ucControllersList), new PropertyMetadata(false, new PropertyChangedCallback(OnItemsSourceChanged)));
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

                    var gg = itemsViewSource.Source as IOrderedEnumerable<IGrouping<string, WemosControllerObservable>>;
                    foreach (var g in gg)
                        result += g.Count();

                    return result;
                }
                else
                    return (itemsViewSource.Source as IEnumerable<WemosControllerObservable>).Count();
            }
        }
        #endregion

        #region Events
        public event ObjectEventHandler ItemAdded;
        public event ObjectEventHandler ItemDeleted;
        public event ObjectEventHandler ItemClicked;
        #endregion

        #region Constructor
        public ucControllersList()
        {
            InitializeComponent();
            XamlUtils.FindFirstVisualChild<Grid>(this).DataContext = this;
        }
        #endregion

        #region Private methods
        private void UpdateTypesList()
        {
            var items = new ObservableCollection<WemosControllerType>();
            foreach (var item in Enum.GetValues(typeof(WemosControllerType)))
                items.Add((WemosControllerType) item);

            cbTypes.ItemsSource = items;
            if (items.Count > 0)
                cbTypes.SelectedItem = items[0];
        }
        private async Task UpdateControllersList()
        {
            var models = await CoreUtils.RequestAsync<List<WemosController>>("/api/wemos/controllers");
            ItemsSource = new ObservableCollection<WemosControllerObservable>(models.Select(m => new WemosControllerObservable(m)));
        }
        private void UpdateItemsViewSource()
        {
            itemsViewSource.IsSourceGrouped = IsGrouped;

            if (ItemsSource != null)
            {
                if (IsGrouped)
                    itemsViewSource.Source = ItemsSource
                        .OrderBy(item => IsSorted ? item.Name : "")
                        .GroupBy(item => item.Name.Substring(0, 1).ToUpper())
                        .OrderBy(item => item.Key);
                else
                    itemsViewSource.Source = ItemsSource.OrderBy(item => IsSorted ? item.Name : "");
            }
            else
                itemsViewSource.Source = null;

            tbEmptyContent.Visibility = Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        #region Event handlers
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTypesList();
            await UpdateControllersList();
        }
        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            await UpdateControllersList();
        }
        private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            tbControllerName.Text = "";

            var result = await dlgAddController.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var name = tbControllerName.Text.Trim();
                var type = (WemosControllerType) cbTypes.SelectedItem;

                var model = await CoreUtils.RequestAsync<WemosController>("/api/wemos/controllers/add", name, type);
                if (model != null)
                    ItemsSource.Add(new WemosControllerObservable(model));
            }
        }
        private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            await CoreUtils.MessageBoxYesNo(Labels.confirmDeleteItem, async (onYes) =>
            {
                var id = (string) ((sender as Button).Tag);

                bool res = await CoreUtils.RequestAsync<bool>("/api/wemos/controllers/delete", id);
                if (res)
                    ItemsSource.Remove(ItemsSource.FirstOrDefault(m => m.ID == id));
            });
        }
        private void lvControllers_ItemClick(object sender, ItemClickEventArgs e)
        {
            ItemClicked?.Invoke(sender, new ObjectEventArgs(e.ClickedItem as WemosControllerObservable));
        }
        #endregion
    }
}
