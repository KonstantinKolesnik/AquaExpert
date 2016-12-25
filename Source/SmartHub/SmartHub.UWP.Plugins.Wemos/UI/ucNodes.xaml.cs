using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Communication.Stream;
using SmartHub.UWP.Plugins.Wemos.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Wemos.UI
{
    public sealed partial class ucNodes : UserControl
    {
        #region Fields
        private StreamClient apiClient = new StreamClient();
        #endregion

        #region Properties
        public ObservableCollection<WemosNode> Nodes
        {
            get;
        } = new ObservableCollection<WemosNode>();
        #endregion

        #region Constructor
        public ucNodes()
        {
            InitializeComponent();
            Utils.FindFirstVisualChild<RadDataGrid>(this).DataContext = this;
        }
        #endregion

        #region Event handlers
        private async void UserControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await apiClient.StartAsync(AppManager.RemoteUrl, AppManager.RemoteServiceName);
            await UpdateNodesList();
        }
        #endregion

        #region Private methods
        private async Task UpdateNodesList()
        {
            var items = await apiClient.RequestAsync<IEnumerable<WemosNode>>("/api/wemos/nodes");

            Nodes.Clear();
            if (items != null)
                foreach (var item in items)
                    Nodes.Add(item);
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
}
