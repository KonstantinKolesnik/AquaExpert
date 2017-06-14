using SmartHub.UWP.Core;
using SmartHub.UWP.Plugins.Wemos.Monitors.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Wemos.UI.Controls
{
    public sealed partial class ucMonitors : UserControl
    {
        #region Properties
        public static readonly DependencyProperty SelectedMonitorProperty = DependencyProperty.Register("SelectedMonitor", typeof(WemosMonitorObservable), typeof(ucMonitors), null);
        public WemosMonitorObservable SelectedMonitor
        {
            get { return (WemosMonitorObservable) GetValue(SelectedMonitorProperty); }
            set { SetValue(SelectedMonitorProperty, value); }
        }
        #endregion

        #region Constructor
        public ucMonitors()
        {
            InitializeComponent();
            Utils.FindFirstVisualChild<Grid>(this).DataContext = this;
        }
        #endregion

        #region Event handlers
        private void ucMonitorsList_ItemClicked(object sender, ObjectEventArgs e)
        {
            SelectedMonitor = e.Data as WemosMonitorObservable;
        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedMonitor = null;
        }
        #endregion
    }
}
