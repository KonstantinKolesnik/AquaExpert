using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Communication.Stream;
using SmartHub.UWP.Plugins.Wemos.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid.Commands;
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
        public ObservableCollection<WemosMonitor> Monitors
        {
            get;
        } = new ObservableCollection<WemosMonitor>();
        #endregion

        #region Constructor
        public ucMonitors()
        {
            InitializeComponent();
            Utils.FindFirstVisualChild<Grid>(this).DataContext = this;
        }
        #endregion

        #region Event handlers
        private async void UserControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await UpdateLinesList();
            await UpdateMonitorsList();
        }
        private async void ButtonAdd_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbMonitorName.Text) && cbLines.SelectedIndex != -1)
            {
                var monitor = await StreamClient.RequestAsync<WemosMonitor>(AppManager.RemoteUrl, AppManager.RemoteServiceName, "/api/wemos/monitors/add", tbMonitorName.Text.Trim(), (cbLines.SelectedItem as WemosLine).ID);
                if (monitor != null)
                    Monitors.Add(monitor);
            }
        }
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
            var items = await StreamClient.RequestAsync<IEnumerable<WemosMonitor>>(AppManager.RemoteUrl, AppManager.RemoteServiceName, "/api/wemos/monitors");

            Monitors.Clear();
            if (items != null)
                foreach (var item in items)
                    Monitors.Add(item);
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

            var item = context.CellInfo.Item as WemosMonitor;
            //await StreamClient.RequestAsync(AppManager.RemoteUrl, AppManager.RemoteServiceName, "/api/wemos/nodes/setname", item.NodeID, item.Name);

            Owner.CommandService.ExecuteDefaultCommand(CommandId.CommitEdit, context);
        }
    }
}
