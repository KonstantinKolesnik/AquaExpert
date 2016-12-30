using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Communication.Stream;
using SmartHub.UWP.Core.StringResources;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using SmartHub.UWP.Plugins.Wemos.Monitors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Wemos.UI
{
    public sealed partial class ucMonitors : UserControl
    {
        #region Properties
        public ObservableCollection<WemosLine> Lines
        {
            get;
        } = new ObservableCollection<WemosLine>();
        public ObservableCollection<WemosMonitorDto> Monitors
        {
            get;
        } = new ObservableCollection<WemosMonitorDto>();

        public static readonly DependencyProperty SelectedMonitorProperty = DependencyProperty.Register("SelectedMonitor", typeof(WemosMonitorDto), typeof(ucMonitors), null);
        public WemosMonitorDto SelectedMonitor
        {
            get { return (WemosMonitorDto) GetValue(SelectedMonitorProperty); }
            set { SetValue(SelectedMonitorProperty, value); }
        }

        public ObservableCollection<WemosLineValue> MonitorValues
        {
            get;
        } = new ObservableCollection<WemosLineValue>();
        #endregion

        #region Constructor
        public ucMonitors()
        {
            InitializeComponent();
            Utils.FindFirstVisualChild<Grid>(this).DataContext = this;
        }
        #endregion

        #region Event handlers
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateLinesList();
            await UpdateMonitorsList();
        }
        private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbMonitorName.Text) && cbLines.SelectedIndex != -1)
            {
                var monitor = await StreamClient.RequestAsync<WemosMonitorDto>(AppManager.RemoteUrl, AppManager.RemoteServiceName, "/api/wemos/monitors/add", tbMonitorName.Text.Trim(), (cbLines.SelectedItem as WemosLine).ID);
                if (monitor != null)
                    Monitors.Add(monitor);
            }
        }
        private /*async*/ void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            int id = (int) ((sender as Button).Tag);
            SelectedMonitor = Monitors.FirstOrDefault(m => m.ID == id);

            pnlMonitorsList.Visibility = Visibility.Collapsed;
            pnlMonitorConfiguration.Visibility = Visibility.Visible;

            //await UpdateMonitorValues();
        }
        private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((sender as Button).Tag);

            await Utils.MessageBoxYesNo(Labels.confirmDeleteItem, async (onYes) =>
            {
                bool res = await StreamClient.RequestAsync<bool>(AppManager.RemoteUrl, AppManager.RemoteServiceName, "/api/wemos/monitors/delete", id);
                if (res)
                    Monitors.Remove(Monitors.FirstOrDefault(m => m.ID == id));
            });
        }
        //private void ButtonSave_Click(object sender, RoutedEventArgs e)
        //{

        //    pnlMonitorsList.Visibility = Visibility.Visible;
        //    pnlMonitorConfiguration.Visibility = Visibility.Collapsed;
        //}
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedMonitor = null;

            pnlMonitorsList.Visibility = Visibility.Visible;
            pnlMonitorConfiguration.Visibility = Visibility.Collapsed;
        }
        //private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        //{
        //    await UpdateMonitorValues();
        //}
        #endregion

        #region Private methods
        private async Task UpdateLinesList()
        {
            var items = await StreamClient.RequestAsync<IEnumerable<WemosLine>>(AppManager.RemoteUrl, AppManager.RemoteServiceName, "/api/wemos/lines");

            Lines.Clear();
            if (items != null)
                foreach (var item in items)
                    Lines.Add(item);
        }
        private async Task UpdateMonitorsList()
        {
            var items = await StreamClient.RequestAsync<IEnumerable<WemosMonitorDto>>(AppManager.RemoteUrl, AppManager.RemoteServiceName, "/api/wemos/monitors");

            Monitors.Clear();
            if (items != null)
                foreach (var item in items)
                    Monitors.Add(item);
        }
        private async Task UpdateMonitorValues()
        {
            var items = await StreamClient.RequestAsync<IEnumerable<WemosLineValue>>(AppManager.RemoteUrl, AppManager.RemoteServiceName, "/api/wemos/line/values", SelectedMonitor.LineID, 10);

            MonitorValues.Clear();
            if (items != null)
                foreach (var item in items)
                    MonitorValues.Add(item);
        }
        #endregion
    }

    public class MonitorCommitEditCommand : DataGridCommand
    {
        public MonitorCommitEditCommand()
        {
            Id = CommandId.CommitEdit;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public async override void Execute(object parameter)
        {
            var context = parameter as EditContext;

            var item = context.CellInfo.Item as WemosMonitorDto;
            if (!string.IsNullOrEmpty(item.Name))
            {
                var res = await StreamClient.RequestAsync<bool>(AppManager.RemoteUrl, AppManager.RemoteServiceName, "/api/wemos/monitors/setnames", item.ID, item.Name, item.NameForInformer);
                if (res)
                    Owner.CommandService.ExecuteDefaultCommand(CommandId.CommitEdit, context);
            }
        }
    }
}
