using SmartHub.UWP.Core;
using SmartHub.UWP.Plugins.Wemos.Controllers;
using SmartHub.UWP.Plugins.Wemos.Controllers.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Wemos.UI.Controls
{
    public sealed partial class ucControllers : UserControl
    {
        #region Constructor
        public ucControllers()
        {
            InitializeComponent();
        }
        #endregion

        #region Event handlers
        private void ucControllersList_ItemClicked(object sender, ObjectEventArgs e)
        {
            var selectedController = e.Item as WemosController;

            ucControllersList.Visibility = Visibility.Collapsed;
            pnlControllerConfiguration.Visibility = Visibility.Visible;

            var ctrl = WemosControllerBase.FromController(selectedController);
            switch (selectedController.Type)
            {
                case WemosControllerType.ScheduledSwitch:
                    ctrlPresenter.Content = new ucControllerScheduledSwitch();
                    break;


            }
        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            ctrlPresenter.Content = null;

            ucControllersList.Visibility = Visibility.Visible;
            pnlControllerConfiguration.Visibility = Visibility.Collapsed;
        }
        #endregion
    }
}
