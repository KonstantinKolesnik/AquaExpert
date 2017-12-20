using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Xaml;
using SmartHub.UWP.Plugins.Wemos.Infrastructure.Controllers.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Wemos.UI.Controls
{
    public sealed partial class ucControllers : UserControl
    {
        #region Properties
        public static readonly DependencyProperty SelectedControllerProperty = DependencyProperty.Register("SelectedController", typeof(WemosControllerObservable), typeof(ucControllers), null);
        public WemosControllerObservable SelectedController
        {
            get { return (WemosControllerObservable) GetValue(SelectedControllerProperty); }
            set { SetValue(SelectedControllerProperty, value); }
        }
        #endregion

        #region Constructor
        public ucControllers()
        {
            InitializeComponent();
            XamlUtils.FindFirstVisualChild<Grid>(this).DataContext = this;
        }
        #endregion

        #region Event handlers
        private void ucControllersList_ItemClicked(object sender, ObjectEventArgs e)
        {
            SelectedController = e.Data as WemosControllerObservable;
        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedController = null;
        }
        #endregion
    }
}
