using Newtonsoft.Json;
using SmartHub.UWP.Core;
using SmartHub.UWP.Plugins.Wemos.Controllers;
using SmartHub.UWP.Plugins.Wemos.Controllers.Models;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
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
        #region Fields
        private WemosHeaterController.ControllerConfiguration configuration = null;
        #endregion

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
            Utils.FindFirstVisualChild<Grid>(this).DataContext = this;

            Controller = ctrl;
            if (Controller != null)
            {
                if (string.IsNullOrEmpty(Controller.Configuration))
                {
                    configuration = new WemosHeaterController.ControllerConfiguration();
                    SaveConfiguration();
                }
                else
                    configuration = JsonConvert.DeserializeObject<WemosHeaterController.ControllerConfiguration>(Controller.Configuration);
            }
        }
        #endregion

        #region Private methods
        private async Task UpdateLinesList()
        {
            var models = await Utils.RequestAsync<List<WemosLine>>("/api/wemos/lines");
            
            foreach (var model in models.Where(m => m.Type == WemosLineType.Switch))
                SwitchLines.Add(model);
            cbSwitchLines.SelectedItem = SwitchLines.FirstOrDefault(l => l.ID == configuration.LineSwitchID);

            foreach (var model in models.Where(m => m.Type == WemosLineType.Temperature))
                TemperatureLines.Add(model);
            cbTemperatureLines.SelectedItem = TemperatureLines.FirstOrDefault(l => l.ID == configuration.LineTemperatureID);
        }
        private void SaveConfiguration()
        {
            Controller.Configuration = JsonConvert.SerializeObject(configuration);
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
            if (line != null && line.ID != configuration.LineSwitchID)
            {
                configuration.LineSwitchID = line.ID;
                SaveConfiguration();
            }
        }
        private void cbTemperatureLines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var line = cbTemperatureLines.SelectedItem as WemosLine;
            if (line != null && line.ID != configuration.LineTemperatureID)
            {
                configuration.LineTemperatureID = line.ID;
                SaveConfiguration();
            }
        }

        //private void btnAddPeriod_Click(object sender, RoutedEventArgs e)
        //{
        //    configuration.ActivePeriods.Add(new Period
        //    {
        //        From = new TimeSpan(0, 0, 0),
        //        To = new TimeSpan(1, 0, 0),
        //        IsEnabled = false
        //    });
        //}
        //private void TimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        //{
        //    SaveConfiguration();
        //}
        //private void tsIsActive_Toggled(object sender, RoutedEventArgs e)
        //{
        //    SaveConfiguration();
        //}
        //private async void btnDeletePeriod_Click(object sender, RoutedEventArgs e)
        //{
        //    var period = (Period) ((sender as Button).Tag);

        //    await Utils.MessageBoxYesNo(Labels.confirmDeleteItem, (onYes) =>
        //    {
        //        configuration.ActivePeriods.Remove(period);
        //    });
        //}
        #endregion
    }
}
