using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Communication.Stream;
using SmartHub.UWP.Core.StringResources;
using SmartHub.UWP.Plugins.Wemos.Controllers;
using SmartHub.UWP.Plugins.Wemos.Controllers.Models;
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
    public sealed partial class ucControllers : UserControl
    {
        #region Properties
        public ObservableCollection<WemosControllerType> ControllerTypes
        {
            get;
        } = new ObservableCollection<WemosControllerType>();
        public ObservableCollection<WemosController> Controllers
        {
            get;
        } = new ObservableCollection<WemosController>();

        public static readonly DependencyProperty SelectedControllerProperty = DependencyProperty.Register("SelectedController", typeof(WemosController), typeof(ucControllers), null);
        public WemosController SelectedController
        {
            get { return (WemosController) GetValue(SelectedControllerProperty); }
            set { SetValue(SelectedControllerProperty, value); }
        }
        #endregion

        #region Constructor
        public ucControllers()
        {
            InitializeComponent();
            Utils.FindFirstVisualChild<Grid>(this).DataContext = this;
        }
        #endregion

        #region Event handlers
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTypesList();
            await UpdateControllersList();
        }
        private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbControllerName.Text) && cbTypes.SelectedIndex != -1)
            {
                var controller = await StreamClient.RequestAsync<WemosController>(AppManager.RemoteUrl, AppManager.RemoteServiceName, "/api/wemos/controllers/add", tbControllerName.Text.Trim(), (WemosControllerType)cbTypes.SelectedItem);
                if (controller != null)
                    Controllers.Add(controller);
            }
        }
        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            int id = (int) ((sender as Button).Tag);
            SelectedController = Controllers.FirstOrDefault(m => m.ID == id);

            pnlControllersList.Visibility = Visibility.Collapsed;
            pnlControllerConfiguration.Visibility = Visibility.Visible;

            var ctrl = WemosControllerBase.FromController(SelectedController);
            //ctrlPresenter.Content = 
        }
        private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            int id = (int) ((sender as Button).Tag);

            await Utils.MessageBoxYesNo(Labels.confirmDeleteItem, async (onYes) =>
            {
                bool res = await StreamClient.RequestAsync<bool>(AppManager.RemoteUrl, AppManager.RemoteServiceName, "/api/wemos/controllers/delete", id);
                if (res)
                    Controllers.Remove(Controllers.FirstOrDefault(m => m.ID == id));
            });
        }
        //private void ButtonSave_Click(object sender, RoutedEventArgs e)
        //{

        //    pnlControllersList.Visibility = Visibility.Visible;
        //    pnlControllerConfiguration.Visibility = Visibility.Collapsed;
        //}
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedController = null;
            ctrlPresenter.Content = null;

            pnlControllersList.Visibility = Visibility.Visible;
            pnlControllerConfiguration.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region Private methods
        private void UpdateTypesList()
        {
            var items = Enum.GetValues(typeof(WemosControllerType));

            ControllerTypes.Clear();
            if (items != null)
                foreach (var item in items)
                    ControllerTypes.Add((WemosControllerType) item);
        }
        private async Task UpdateControllersList()
        {
            var items = await StreamClient.RequestAsync<IEnumerable<WemosController>>(AppManager.RemoteUrl, AppManager.RemoteServiceName, "/api/wemos/controllers");

            Controllers.Clear();
            if (items != null)
                foreach (var item in items)
                    Controllers.Add(item);
        }
        #endregion
    }

    public class ControllerCommitEditCommand : DataGridCommand
    {
        public ControllerCommitEditCommand()
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

            var item = context.CellInfo.Item as WemosController;
            if (!string.IsNullOrEmpty(item.Name))
            {
                var res = await StreamClient.RequestAsync<bool>(AppManager.RemoteUrl, AppManager.RemoteServiceName, "/api/wemos/controllers/setname", item.ID, item.Name);
                if (res)
                    Owner.CommandService.ExecuteDefaultCommand(CommandId.CommitEdit, context);
            }
        }
    }
}
