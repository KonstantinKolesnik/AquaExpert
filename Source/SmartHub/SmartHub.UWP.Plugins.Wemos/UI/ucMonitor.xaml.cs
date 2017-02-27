using SmartHub.UWP.Core;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using SmartHub.UWP.Plugins.Wemos.Monitors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Wemos.UI
{
    public sealed partial class ucMonitor : UserControl
    {
        private System.Threading.Timer timer;

        public static readonly DependencyProperty MonitorProperty = DependencyProperty.Register("Monitor", typeof(WemosMonitorDto), typeof(ucMonitor), new PropertyMetadata(null, new PropertyChangedCallback(OnMonitorChanged)));
        public WemosMonitorDto Monitor
        {
            get { return (WemosMonitorDto) GetValue(MonitorProperty); }
            set { SetValue(MonitorProperty, value); }
        }
        private async static void OnMonitorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = d as ucMonitor;
            await uc.UpdateMonitorValues();
        }

        public ObservableCollection<WemosLineValue> MonitorValues
        {
            get;
        } = new ObservableCollection<WemosLineValue>();

        public static readonly DependencyProperty LastValueProperty = DependencyProperty.Register("LastValue", typeof(string), typeof(ucMonitor), null);
        public string LastValue
        {
            get { return (string) GetValue(LastValueProperty); }
            set { SetValue(LastValueProperty, value); }
        }

        public ucMonitor()
        {
            InitializeComponent();
            Utils.FindFirstVisualChild<Grid>(this).DataContext = this;

            timer = new System.Threading.Timer(timerCallback, null, 0, (int) TimeSpan.FromSeconds(10).TotalMilliseconds);
        }

        private async Task UpdateMonitorValues()
        {
            MonitorValues.Clear();

            if (Monitor != null)
            {
                yAxis.LabelFormat = "{0:N2} " + GetUnits();
                lblDefinition.Format = "{0:N1} " + GetUnits();

                var items = await Utils.RequestAsync<IEnumerable<WemosLineValue>>("/api/wemos/line/values", Monitor.LineID, 10);

                if (items != null)
                    foreach (var item in items.OrderBy(i => i.TimeStamp))
                        MonitorValues.Add(item);
            }

            LastValue = MonitorValues.Any() ? MonitorValues.LastOrDefault().Value + " " + GetUnits(): "---";
        }
        private string GetUnits()
        {
            return Monitor != null ? WemosPlugin.LineTypeToUnits(Monitor.LineType) : "";
        }

        private async void timerCallback(object state)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await UpdateMonitorValues(); });
        }
    }
}
