using SmartHub.UWP.Core;
using SmartHub.UWP.Plugins.Wemos.Controllers.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Wemos.UI.Controls
{
    public sealed partial class ucControllers : UserControl
    {
        #region Properties
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
        private void ucControllersList_ItemClicked(object sender, ObjectEventArgs e)
        {
            SelectedController = e.Item as WemosController;
        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedController = null;
        }
        #endregion
    }
}
