using SmartHub.UWP.Core;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
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
    public sealed partial class ucLines : UserControl
    {
        #region Properties
        public ObservableCollection<WemosLine> Lines
        {
            get;
        } = new ObservableCollection<WemosLine>();
        #endregion

        #region Constructor
        public ucLines()
        {
            InitializeComponent();
            Utils.FindFirstVisualChild<RadDataGrid>(this).DataContext = this;
        }
        #endregion

        #region Private methods
        private async Task UpdateLinesList()
        {
            var items = await Utils.RequestAsync<IEnumerable<WemosLine>>("/api/wemos/lines");

            Lines.Clear();

            if (items != null)
                foreach (var item in items.Where(item => item != null))
                    Lines.Add(item);
        }
        #endregion

        #region Event handlers
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateLinesList();
        }
        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            await UpdateLinesList();
        }
        #endregion
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

            var item = context.CellInfo.Item as WemosLine;
            var res = await Utils.RequestAsync<bool>("/api/wemos/lines/setname", item.NodeID, item.LineID, item.Name);
            if (res)
                Owner.CommandService.ExecuteDefaultCommand(CommandId.CommitEdit, context);
        }
    }
}
