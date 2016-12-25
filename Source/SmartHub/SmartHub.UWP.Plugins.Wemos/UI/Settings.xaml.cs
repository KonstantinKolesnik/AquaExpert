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
    public sealed partial class Settings : UserControl
    {
        #region Fields
        private StreamClient apiClient = new StreamClient();
        #endregion

        #region Properties
        public ObservableCollection<WemosNode> Nodes
        {
            get;
        } = new ObservableCollection<WemosNode>();
        public ObservableCollection<WemosLine> Lines
        {
            get;
        } = new ObservableCollection<WemosLine>();
        #endregion

        #region Constructor
        public Settings()
        {
            InitializeComponent();
            Utils.FindFirstVisualChild<Grid>(this).DataContext = this;
        }
        #endregion

        #region Event handlers
        private async void UserControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await apiClient.StartAsync(AppManager.RemoteUrl, AppManager.RemoteServiceName);

            await UpdateNodesList();
            await UpdateLinesList();

            //GridLocalizationManager.
        }
        #endregion

        #region Private methods
        private async Task UpdateNodesList()
        {
            var items = await apiClient.RequestAsync<IEnumerable<WemosNode>>("/api/wemos/nodes");

            Nodes.Clear();
            foreach (var item in items)
                Nodes.Add(item);
        }
        private async Task UpdateLinesList()
        {
            var items = await apiClient.RequestAsync<IEnumerable<WemosLine>>("/api/wemos/lines");

            Lines.Clear();
            foreach (var item in items)
                Lines.Add(item);
        }
        #endregion
    }

    public class NodeCommitEditCommand : DataGridCommand
    {
        public NodeCommitEditCommand()
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

            var node = context.CellInfo.Item as WemosNode;

            var apiClient = new StreamClient();
            await apiClient.StartAsync(AppManager.RemoteUrl, AppManager.RemoteServiceName);
            await apiClient.RequestAsync("/api/wemos/nodes/setname", node.NodeID, node.Name);

            Owner.CommandService.ExecuteDefaultCommand(CommandId.CommitEdit, context);
        }
    }
    public class LineCommitEditCommand : DataGridCommand
    {
        public LineCommitEditCommand()
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

            var line = context.CellInfo.Item as WemosLine;

            var apiClient = new StreamClient();
            await apiClient.StartAsync(AppManager.RemoteUrl, AppManager.RemoteServiceName);
            await apiClient.RequestAsync("/api/wemos/lines/setname", line.NodeID, line.LineID, line.Name);

            Owner.CommandService.ExecuteDefaultCommand(CommandId.CommitEdit, context);
        }
    }
}
