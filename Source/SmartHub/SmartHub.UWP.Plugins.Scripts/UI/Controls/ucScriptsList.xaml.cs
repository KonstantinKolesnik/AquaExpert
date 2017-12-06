using SmartHub.UWP.Core;
using SmartHub.UWP.Core.StringResources;
using SmartHub.UWP.Core.Xaml;
using SmartHub.UWP.Plugins.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Scripts.UI.Controls
{
    public sealed partial class ucScriptsList : UserControl
    {
        #region Properties
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<UserScriptObservable>), typeof(ucScriptsList), new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));
        public ObservableCollection<UserScriptObservable> ItemsSource
        {
            get { return (ObservableCollection<UserScriptObservable>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = d as ucScriptsList;
            uc.UpdateItemsViewSource();

            var items = e.NewValue as ObservableCollection<UserScriptObservable>;
            if (items != null)
                items.CollectionChanged += (s, args) =>
                {
                    uc.UpdateItemsViewSource();

                    if (args.Action == NotifyCollectionChangedAction.Add)
                        uc.ItemAdded?.Invoke(uc, new ObjectEventArgs(args.NewItems[0] as UserScriptObservable));
                    else if (args.Action == NotifyCollectionChangedAction.Remove)
                        uc.ItemDeleted?.Invoke(uc, new ObjectEventArgs(args.OldItems[0] as UserScriptObservable));
                };
        }

        public static readonly DependencyProperty IsSortedProperty = DependencyProperty.Register("IsSorted", typeof(bool), typeof(ucScriptsList), new PropertyMetadata(false, new PropertyChangedCallback(OnItemsSourceChanged)));
        public bool IsSorted
        {
            get { return (bool)GetValue(IsSortedProperty); }
            set { SetValue(IsSortedProperty, value); }
        }

        public static readonly DependencyProperty IsGroupedProperty = DependencyProperty.Register("IsGrouped", typeof(bool), typeof(ucScriptsList), new PropertyMetadata(false, new PropertyChangedCallback(OnItemsSourceChanged)));
        public bool IsGrouped
        {
            get { return (bool)GetValue(IsGroupedProperty); }
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

                    var gg = itemsViewSource.Source as IOrderedEnumerable<IGrouping<string, UserScriptObservable>>;
                    foreach (var g in gg)
                        result += g.Count();

                    return result;
                }
                else
                    return (itemsViewSource.Source as IEnumerable<UserScriptObservable>).Count();
            }
        }
        #endregion

        #region Events
        public event ObjectEventHandler ItemAdded;
        public event ObjectEventHandler ItemDeleted;
        public event ObjectEventHandler ItemClicked;
        #endregion

        #region Constructor
        public ucScriptsList()
        {
            InitializeComponent();
            XamlUtils.FindFirstVisualChild<Grid>(this).DataContext = this;
        }
        #endregion

        #region Private methods
        private async Task UpdateScriptsList()
        {
            biRequest.IsActive = true;

            var models = await CoreUtils.RequestAsync<List<UserScript>>("/api/scripts");
            ItemsSource = models != null ? new ObservableCollection<UserScriptObservable>(models.Where(m => m != null).Select(m => new UserScriptObservable(m))) : null;

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
                        .OrderBy(item => IsSorted ? item.Name ?? "" : "")
                        .GroupBy(item => string.IsNullOrEmpty(item.Name) ? "" : item.Name.Substring(0, 1).ToUpper())
                        .OrderBy(item => item.Key);
                else
                    itemsViewSource.Source = ItemsSource.OrderBy(item => IsSorted ? item.Name ?? "" : "");
            }
            else
                itemsViewSource.Source = null;

            tbEmptyContent.Visibility = Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        #region Event handlers
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateScriptsList();
        }
        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            await UpdateScriptsList();
        }
        private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            tbScriptName.Text = "";

            var result = await dlgAddScript.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var name = tbScriptName.Text.Trim();

                var model = await CoreUtils.RequestAsync<UserScript>("/api/scripts/add", name);
                if (model != null)
                    ItemsSource.Add(new UserScriptObservable(model));
            }
        }
        private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            await CoreUtils.MessageBoxYesNo(Labels.confirmDeleteItem, async (onYes) =>
            {
                var id = (string)((sender as Button).Tag);

                bool res = await CoreUtils.RequestAsync<bool>("/api/scripts/delete", id);
                if (res)
                    ItemsSource.Remove(ItemsSource.FirstOrDefault(m => m.ID == id));
            });
        }
        private void lvMonitors_ItemClick(object sender, ItemClickEventArgs e)
        {
            ItemClicked?.Invoke(sender, new ObjectEventArgs(e.ClickedItem as UserScriptObservable));
        }
        #endregion
    }
}
