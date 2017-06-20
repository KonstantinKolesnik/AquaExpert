using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Xaml;
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
        //private DispatcherTimer dispatcherTimer;
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

        public static readonly DependencyProperty LastTimeStampProperty = DependencyProperty.Register("LastTimeStamp", typeof(string), typeof(ucMonitorChart), null);
        public string LastTimeStamp
        {
            get { return (string) GetValue(LastTimeStampProperty); }
            set { SetValue(LastTimeStampProperty, value); }
        }
        #endregion

        #region Constructor
        public ucMonitorChart()
        {
            InitializeComponent();
            XamlUtils.FindFirstVisualChild<Grid>(this).DataContext = this;

            //dispatcherTimer = new DispatcherTimer();
            //dispatcherTimer.Interval = TimeSpan.FromSeconds(updateIntervalSeconds);
            //dispatcherTimer.Tick += (s, e) =>
            //{
            //    if (Visibility == Visibility.Visible)
            //        UpdateValues();
            //};
            //dispatcherTimer.Start();

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
            biRequest.IsActive = true;

            if (Monitor != null)
            {
                //xAxis.LabelFormat = "{0:G}";
                xAxis.LabelFormat = "{0:dd.MM.yy\nH:mm:ss}";
                yAxis.LabelFormat = "{0:N2} " + GetUnits();
                lblDefinition.Format = "{0:N1} " + GetUnits();

                var items = await CoreUtils.RequestAsync<List<WemosLineValue>>("/api/wemos/line/values", Monitor.LineID, valuesDisplayCount);

                Values.Clear();

                if (items != null)
                    foreach (var item in items.Where(item => item != null).OrderBy(i => i.TimeStamp))
                    {
                        //item.TimeStamp = item.TimeStamp.ToLocalTime();
                        Values.Add(item);
                    }
            }
            else
                Values.Clear();

            LastValue = Values.Any() ? $"{Values.LastOrDefault().Value} {GetUnits()}" : "---";
            LastTimeStamp = Values.Any() ? $"{Values.LastOrDefault().TimeStamp.ToString("dd.MM.yy H:mm:ss")}" : "---";

            biRequest.IsActive = false;
        }
        private string GetUnits()
        {
            return Monitor != null ? WemosPlugin.LineTypeToUnits(Monitor.LineType) : "";
        }
        #endregion

        #region Event handlers
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            timer.Cancel();
            //dispatcherTimer.Stop();
        }
        #endregion
    }
}
