using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Communication.Stream;
using SmartHub.UWP.Plugins.Wemos.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;

namespace SmartHub.UWP.Plugins.Wemos.UI
{
    public sealed partial class ucMonitor : UserControl
    {
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
        }

        private async Task UpdateMonitorValues()
        {
            MonitorValues.Clear();

            if (Monitor != null)
            {
                yAxis.LabelFormat = "{0:N2} " + GetUnits();
                lblDefinition.Format = "{0:N1} " + GetUnits();

                var items = await StreamClient.RequestAsync<IEnumerable<WemosLineValue>>(AppManager.RemoteUrl, AppManager.RemoteServiceName, "/api/wemos/line/values", Monitor.LineID, 10);

                if (items != null)
                    foreach (var item in items)
                        MonitorValues.Add(item);
            }

            LastValue = MonitorValues.Any() ? MonitorValues.LastOrDefault().Value + " " + GetUnits(): "---";
        }

        private string GetUnits()
        {
            if (Monitor == null)
                return "";

            switch (Monitor.LineType)
            {
                case WemosLineType.Switch: return "";
                case WemosLineType.Temperature: return "°C";
                case WemosLineType.Humidity: return "%";
                case WemosLineType.Barometer: return "mm Hg";
                case WemosLineType.Weight: return "kg";
                case WemosLineType.Voltage: return "V";
                case WemosLineType.Current: return "A";
                case WemosLineType.Power: return "Wt";
                case WemosLineType.Rain: return "";
                case WemosLineType.UV: return "";
                case WemosLineType.Distance: return "m";
                case WemosLineType.LightLevel: return "lux";
                case WemosLineType.IR: return "";
                case WemosLineType.AirQuality: return "";
                case WemosLineType.Vibration: return "";
                case WemosLineType.Ph: return "";
                case WemosLineType.ORP: return "";


                default: return "";
            }
        }
    }
}
