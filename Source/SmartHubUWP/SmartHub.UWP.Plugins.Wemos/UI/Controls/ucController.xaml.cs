using SmartHub.UWP.Core.Xaml;
using SmartHub.UWP.Plugins.Wemos.Controllers.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Wemos.UI.Controls
{
    public sealed partial class ucController : UserControl
    {
        #region Properties
        public static readonly DependencyProperty ControllerProperty = DependencyProperty.Register("Controller", typeof(WemosControllerObservable), typeof(ucController), new PropertyMetadata(null, new PropertyChangedCallback(OnControllerChanged)));
        public WemosControllerObservable Controller
        {
            get { return (WemosControllerObservable) GetValue(ControllerProperty); }
            set { SetValue(ControllerProperty, value); }
        }
        private static void OnControllerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ucController).LoadEditor();
        }
        #endregion

        #region Constructor
        public ucController()
        {
            InitializeComponent();
            XamlUtils.FindFirstVisualChild<Grid>(this).DataContext = this;
        }
        #endregion

        #region Private methods
        private void LoadEditor()
        {
            if (Controller == null)
                ctrlPresenter.Content = null;
            else
                switch (Controller.Type)
                {
                    case WemosControllerType.ScheduledSwitch: ctrlPresenter.Content = new ucControllerScheduledSwitch(Controller); break;
                    case WemosControllerType.Heater: ctrlPresenter.Content = new ucControllerHeater(Controller); break;

                }
        }
        #endregion
    }
}
