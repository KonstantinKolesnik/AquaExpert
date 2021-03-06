﻿using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Xaml;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Wemos.UI.Controls
{
    public sealed partial class ucNodes : UserControl
    {
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
            XamlUtils.FindFirstVisualChild<RadDataGrid>(this).DataContext = this;
        }
        #endregion

        #region Private methods
        private async Task UpdateNodesList()
        {
            biRequest.IsActive = true;

            var items = await CoreUtils.RequestAsync<List<WemosNode>>("/api/wemos/nodes");

            Nodes.Clear();

            if (items != null)
                foreach (var item in items.Where(item => item != null))
                    Nodes.Add(item);

            biRequest.IsActive = false;
        }
        #endregion

        #region Event handlers
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateNodesList();
        }
        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            await UpdateNodesList();
        }
        private async void ButtonPresentation_Click(object sender, RoutedEventArgs e)
        {
            await CoreUtils.RequestAsync<bool>("/api/wemos/presentation");
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
            var item = context.CellInfo.Item as WemosNode;

            var res = await CoreUtils.RequestAsync<bool>("/api/wemos/nodes/update", item);
            if (res)
                Owner.CommandService.ExecuteDefaultCommand(CommandId.CommitEdit, context);
        }
    }
}
