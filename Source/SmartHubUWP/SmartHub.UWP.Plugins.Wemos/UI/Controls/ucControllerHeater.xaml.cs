using Newtonsoft.Json;
using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Xaml;
using SmartHub.UWP.Plugins.Lines.Models;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using SmartHub.UWP.Plugins.Wemos.Infrastructure.Controllers;
using SmartHub.UWP.Plugins.Wemos.Infrastructure.Controllers.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Wemos.UI.Controls
{
    public sealed partial class ucControllerHeater : UserControl
    {
        #region Properties
        public static readonly DependencyProperty ControllerProperty = DependencyProperty.Register("Controller", typeof(WemosControllerObservable), typeof(ucControllerHeater), new PropertyMetadata(null, new PropertyChangedCallback(OnControllerChanged)));
        public WemosControllerObservable Controller
        {
            get { return (WemosControllerObservable) GetValue(ControllerProperty); }
            set { SetValue(ControllerProperty, value); }
        }
        private static void OnControllerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public static readonly DependencyProperty ConfigurationProperty = DependencyProperty.Register("Configuration", typeof(WemosControllerWorkerHeater.ControllerConfiguration), typeof(ucControllerHeater), null);
        public WemosControllerWorkerHeater.ControllerConfiguration Configuration
        {
            get { return (WemosControllerWorkerHeater.ControllerConfiguration) GetValue(ConfigurationProperty); }
            set { SetValue(ConfigurationProperty, value); }
        }

        public ObservableCollection<WemosLine> SwitchLines
        {
            get;
        } = new ObservableCollection<WemosLine>();
        public ObservableCollection<WemosLine> TemperatureLines
        {
            get;
        } = new ObservableCollection<WemosLine>();
        #endregion

        #region Constructor
        public ucControllerHeater(WemosControllerObservable ctrl)
        {
            InitializeComponent();
            XamlUtils.FindFirstVisualChild<ScrollViewer>(this).DataContext = this;

            Controller = ctrl;
            if (Controller != null)
            {
                if (string.IsNullOrEmpty(Controller.Configuration))
                {
                    Configuration = new WemosControllerWorkerHeater.ControllerConfiguration();
                    SaveConfiguration();
                }
                else
                    Configuration = JsonConvert.DeserializeObject<WemosControllerWorkerHeater.ControllerConfiguration>(Controller.Configuration);
            }
        }
        #endregion

        #region Private methods
        private async Task UpdateLinesList()
        {
            var models = await CoreUtils.RequestAsync<List<WemosLine>>("/api/wemos/lines");
            
            foreach (var model in models.Where(m => m.Type == LineType.Switch))
                SwitchLines.Add(model);
            cbSwitchLines.SelectedItem = SwitchLines.FirstOrDefault(l => l.ID == Configuration.LineSwitchID);

            foreach (var model in models.Where(m => m.Type == LineType.Temperature))
                TemperatureLines.Add(model);
            cbTemperatureLines.SelectedItem = TemperatureLines.FirstOrDefault(l => l.ID == Configuration.LineTemperatureID);
        }
        private void SaveConfiguration()
        {
            if (Controller != null && Configuration != null)
                Controller.Configuration = JsonConvert.SerializeObject(Configuration);
        }
        #endregion

        #region Event handlers
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateLinesList();
        }
        private void cbSwitchLines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var line = cbSwitchLines.SelectedItem as WemosLine;
            if (line != null && line.ID != Configuration.LineSwitchID)
            {
                Configuration.LineSwitchID = line.ID;
                SaveConfiguration();
            }
        }
        private void cbTemperatureLines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var line = cbTemperatureLines.SelectedItem as WemosLine;
            if (line != null && line.ID != Configuration.LineTemperatureID)
            {
                Configuration.LineTemperatureID = line.ID;
                SaveConfiguration();
            }
        }
        private void RadNumericBox_ValueChanged(object sender, EventArgs e)
        {
            SaveConfiguration();
        }
        private void tbTAlarm_TextChanged(object sender, TextChangedEventArgs e)
        {
             SaveConfiguration();
        }
        #endregion
    }
}
