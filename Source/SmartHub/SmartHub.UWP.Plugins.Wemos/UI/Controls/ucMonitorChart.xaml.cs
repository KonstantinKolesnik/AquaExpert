using SmartHub.UWP.Core;
using SmartHub.UWP.Core.StringResources;
using SmartHub.UWP.Core.Xaml;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using SmartHub.UWP.Plugins.Wemos.Monitors.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
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
        private double updateIntervalSeconds = 1;
        private double valuesDisplayCount = 10;

        private Task taskListen;
        private CancellationTokenSource ctsListen;
        private bool isListenActive = false;
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
            //await (d as ucMonitorChart).UpdateValues();
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await (d as ucMonitorChart).UpdateValues(); });
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

            //timer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(async (t) =>
            //{
            //    //await UpdateValues();
            //    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await UpdateValues(); });
            //}), TimeSpan.FromSeconds(updateIntervalSeconds));
        }
        #endregion

        #region Private methods
        private async Task UpdateValues()
        {
            if (Visibility == Visibility.Visible)
            {
                //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                //{
                biRequest.IsActive = true;

                if (Monitor != null)
                {
                    //xAxis.LabelFormat = "{0:G}";
                    xAxis.LabelFormat = "{0:dd.MM.yy\nHH:mm:ss}";
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

                LastValue = Values.Any() ? $"{Values.LastOrDefault().Value} {GetUnits()}" : Labels.NoData;
                LastTimeStamp = Values.Any() ? $"{Values.LastOrDefault().TimeStamp.ToString("dd.MM.yy HH:mm:ss")}" : "";

                biRequest.IsActive = false;
                //});
            }
        }
        private string GetUnits()
        {
            return Monitor != null ? WemosPlugin.LineTypeToUnits(Monitor.LineType) : "";
        }

        private void StartListen()
        {
            if (!isListenActive)
            {
                ctsListen = new CancellationTokenSource();

                taskListen = Task.Factory.StartNew(async () =>
                {
                    while (!ctsListen.IsCancellationRequested)
                    {
                        if (isListenActive)
                            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await UpdateValues(); });

                        await Task.Delay((int)updateIntervalSeconds * 1000);
                    }
                }, ctsListen.Token);

                isListenActive = true;
            }
        }
        private void StopListen()
        {
            if (isListenActive)
            {
                ctsListen?.Cancel();
                isListenActive = false;
            }
        }
        #endregion

        #region Event handlers
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            StartListen();
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (timer != null)
                timer.Cancel();

            StopListen();
        }
        #endregion
    }
}
