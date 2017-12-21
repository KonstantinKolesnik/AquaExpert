using SmartHub.UWP.Core.Xaml;
using SmartHub.UWP.Plugins.Wemos.Infrastructure.Monitors.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Wemos.UI.Controls
{
    public sealed partial class ucMonitor : UserControl
    {
        #region Properties
        public static readonly DependencyProperty MonitorProperty = DependencyProperty.Register("Monitor", typeof(WemosLineMonitorObservable), typeof(ucMonitor), new PropertyMetadata(null, new PropertyChangedCallback(OnMonitorChanged)));
        public WemosLineMonitorObservable Monitor
        {
            get { return (WemosLineMonitorObservable) GetValue(MonitorProperty); }
            set { SetValue(MonitorProperty, value); }
        }
        private async static void OnMonitorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is WemosLineMonitorObservable oldMonitor)
                oldMonitor.StopListen();

            if (e.NewValue is WemosLineMonitorObservable newMonitor)
            {
                var uc = d as ucMonitor;
                uc.SetChartsFormats();

                newMonitor.PropertyChanged += (s, args) => { uc.SetChartsFormats(); };

                await newMonitor.UpdateValues();
                newMonitor.StartListen();
            }
        }

        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(ucMonitor), new PropertyMetadata(false));
        public bool IsEditable
        {
            get { return (bool) GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }
        #endregion

        #region Constructor
        public ucMonitor()
        {
            InitializeComponent();
            XamlUtils.FindFirstVisualChild<Grid>(this).DataContext = this;
        }
        #endregion

        #region Private methods
        private void SetChartsFormats()
        {
            if (Monitor != null)
            {
                //xAxis.LabelFormat = "{0:G}";
                xAxis.LabelFormat = "{0:dd.MM.yy\nHH:mm:ss}";

                var valueFormat = "{0:N" + Monitor.Precision + "}";
                var units = (string.IsNullOrEmpty(Monitor.Units) ? WemosPlugin.LineTypeToUnits(Monitor.LineType) : Monitor.Units);

                yAxis.LabelFormat = $"{valueFormat} {units}";
                lblDefinition0.Format = lblDefinition.Format = valueFormat;
                nbMin.ValueFormat = nbMax.ValueFormat = nbOffset.ValueFormat = valueFormat;
            }
        }
        #endregion

        #region Event handlers
        //private void RadExpanderControl_PreviewKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        //{
        //    e.Handled = true;
        //}
        #endregion
    }
}
