using SmartHub.UWP.Core.Xaml;
using SmartHub.UWP.Plugins.Wemos.Monitors.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Wemos.UI.Controls
{
    public sealed partial class ucMonitor : UserControl
    {
        #region Properties
        public static readonly DependencyProperty MonitorProperty = DependencyProperty.Register("Monitor", typeof(WemosMonitorObservable), typeof(ucMonitor), new PropertyMetadata(null, new PropertyChangedCallback(OnMonitorChanged)));
        public WemosMonitorObservable Monitor
        {
            get { return (WemosMonitorObservable) GetValue(MonitorProperty); }
            set { SetValue(MonitorProperty, value); }
        }
        private async static void OnMonitorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is WemosMonitorObservable oldMonitor)
                oldMonitor.StopListen();

            if (e.NewValue is WemosMonitorObservable newMonitor)
            {
                await newMonitor.UpdateValues();

                //var uc = d as ucMonitor;
                //uc.xAxis.LabelFormat = "{0:G}";
                //uc.xAxis.LabelFormat = "{0:dd.MM.yy\nHH:mm:ss}";
                //uc.yAxis.LabelFormat = "{0:N1} " + (string.IsNullOrEmpty(newMonitor.Units) ? WemosPlugin.LineTypeToUnits(newMonitor.LineType) : newMonitor.Units);
                //uc.lblDefinition0.Format = uc.yAxis.LabelFormat;
                //uc.lblDefinition.Format = uc.yAxis.LabelFormat;

                newMonitor.StartListen();
            }
        }

        public static readonly DependencyProperty XAxisFormatProperty = DependencyProperty.Register("XAxisFormat", typeof(string), typeof(ucMonitor), new PropertyMetadata("{0:dd.MM.yy\nHH:mm:ss}"));
        public string XAxisFormat
        {
            get { return (string)GetValue(XAxisFormatProperty); }
        }

        public static readonly DependencyProperty YAxisFormatProperty = DependencyProperty.Register("YAxisFormat", typeof(string), typeof(ucMonitor), null);/*new PropertyMetadata("{0:N1}")*/
        public string YAxisFormat
        {
            get { return "{0:N1} " + (string.IsNullOrEmpty(Monitor.Units) ? WemosPlugin.LineTypeToUnits(Monitor.LineType) : Monitor.Units); }
        }

        public static readonly DependencyProperty ValueFormatProperty = DependencyProperty.Register("ValueFormat", typeof(string), typeof(ucMonitor), new PropertyMetadata("{0:N1}"));
        public string ValueFormat
        {
            get { return "{}{0,0:N1}" /*"{0:N1}"*/; }
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

        #region Event handlers
        //private void RadExpanderControl_PreviewKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        //{
        //    e.Handled = true;
        //}
        #endregion
    }
}
