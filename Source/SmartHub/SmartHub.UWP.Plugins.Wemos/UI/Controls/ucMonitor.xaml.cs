using SmartHub.UWP.Core;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using SmartHub.UWP.Plugins.Wemos.Monitors.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Wemos.UI.Controls
{
    public sealed partial class ucMonitor : UserControl
    {
        #region Fields
        private System.Threading.Timer timer;
        private double updateIntervalSeconds = 10;
        private double valuesDisplayCount = 10;
        #endregion

        #region Properties
        public static readonly DependencyProperty MonitorProperty = DependencyProperty.Register("Monitor", typeof(WemosMonitorObservable), typeof(ucMonitor), new PropertyMetadata(null, new PropertyChangedCallback(OnMonitorChanged)));
        public WemosMonitorObservable Monitor
        {
            get { return (WemosMonitorObservable) GetValue(MonitorProperty); }
            set { SetValue(MonitorProperty, value); }
        }
        private async static void OnMonitorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            await (d as ucMonitor).UpdateValues();
        }

        public ObservableCollection<WemosLineValue> Values
        {
            get;
        } = new ObservableCollection<WemosLineValue>();

        public static readonly DependencyProperty LastValueProperty = DependencyProperty.Register("LastValue", typeof(string), typeof(ucMonitor), null);
        public string LastValue
        {
            get { return (string) GetValue(LastValueProperty); }
            set { SetValue(LastValueProperty, value); }
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
            Utils.FindFirstVisualChild<Grid>(this).DataContext = this;

            timer = new System.Threading.Timer(timerCallback, null, 0, (int) TimeSpan.FromSeconds(updateIntervalSeconds).TotalMilliseconds);
        }
        #endregion

        #region Private methods
        private async Task UpdateValues()
        {
            Values.Clear();

            if (Monitor != null)
            {
                yAxis.LabelFormat = "{0:N1} " + GetUnits();
                lblDefinition.Format = "{0:N1} " + GetUnits();

                var items = await Utils.RequestAsync<IEnumerable<WemosLineValue>>("/api/wemos/line/values", Monitor.LineID, valuesDisplayCount);
                if (items != null)
                    foreach (var item in items.OrderBy(i => i.TimeStamp))
                        Values.Add(item);
            }

            LastValue = Values.Any() ? $"{Values.LastOrDefault().Value} {GetUnits()}" : "---";
        }
        private string GetUnits()
        {
            return Monitor != null ? WemosPlugin.LineTypeToUnits(Monitor.LineType) : "";
        }
        #endregion

        #region Event handlers
        private async void timerCallback(object state)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await UpdateValues(); });
        }
        #endregion
    }
}
