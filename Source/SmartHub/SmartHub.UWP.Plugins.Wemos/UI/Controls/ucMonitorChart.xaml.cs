using SmartHub.UWP.Core;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using SmartHub.UWP.Plugins.Wemos.Monitors.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Wemos.UI.Controls
{
    public sealed partial class ucMonitorChart : UserControl
    {
        #region Fields
        private ThreadPoolTimer timer;
        private double updateIntervalSeconds = 10;
        private double valuesDisplayCount = 10;
        #endregion

        #region Properties
        public static readonly DependencyProperty MonitorProperty = DependencyProperty.Register("Monitor", typeof(WemosMonitorObservable), typeof(ucMonitorChart), new PropertyMetadata(null, new PropertyChangedCallback(OnMonitorChanged)));
        public WemosMonitorObservable Monitor
        {
            get { return (WemosMonitorObservable) GetValue(MonitorProperty); }
            set { SetValue(MonitorProperty, value); }
        }
        private async static void OnMonitorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            await (d as ucMonitorChart).UpdateValues();
        }

        public ObservableCollection<WemosLineValue> Values
        {
            get;
        } = new ObservableCollection<WemosLineValue>();

        public static readonly DependencyProperty LastValueProperty = DependencyProperty.Register("LastValue", typeof(string), typeof(ucMonitorChart), null);
        public string LastValue
        {
            get { return (string) GetValue(LastValueProperty); }
            set { SetValue(LastValueProperty, value); }
        }
        #endregion

        #region Constructor
        public ucMonitorChart()
        {
            InitializeComponent();
            Utils.FindFirstVisualChild<Grid>(this).DataContext = this;

            timer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(async (t) =>
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    if (Visibility == Visibility.Visible)
                        await UpdateValues();
                });
            }), TimeSpan.FromSeconds(updateIntervalSeconds));
        }
        #endregion

        #region Private methods
        private async Task UpdateValues()
        {
            if (Monitor != null)
            {
                yAxis.LabelFormat = "{0:N2} " + GetUnits();
                lblDefinition.Format = "{0:N1} " + GetUnits();

                var items = await Utils.RequestAsync<List<WemosLineValue>>("/api/wemos/line/values", Monitor.LineID, valuesDisplayCount);
                if (items != null)
                {
                    //// remove orphan items:
                    //var deletedItems = Values.Except(items);
                    //foreach (var item in deletedItems)
                    //    Values.Remove(item);

                    //// add new items:
                    //var addedItems = items.Except(Values);
                    //foreach (var item in addedItems)
                    //    Values.Add(item);



                    Values.Clear();
                    foreach (var item in items.OrderBy(i => i.TimeStamp))
                        Values.Add(item);
                }
            }
            else
                Values.Clear();

            LastValue = Values.Any() ? $"{Values.LastOrDefault().Value} {GetUnits()}" : "---";
        }
        private string GetUnits()
        {
            return Monitor != null ? WemosPlugin.LineTypeToUnits(Monitor.LineType) : "";
        }
        #endregion
    }
}
