using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SmartHub.UWP.Applications.Server
{
    public sealed partial class DashboardPage : Page
    {
        //private readonly Mpu6050 mpu6050 = new Mpu6050();
        //private int interruptCount = 0;
        //private readonly DateTime startTime;

        #region Constructor
        public DashboardPage()
        {
            InitializeComponent();

            //mpu6050.InitHardware();
            //mpu6050.SensorInterruptEvent += mpu6050_SensorInterruptEvent;
            //startTime = DateTime.Now;
        }
        #endregion

        #region Navigation
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            AppShell.Current.SetNavigationInfo("Dashboard", "menuDashboard");
        }
        #endregion


        //private void mpu6050_SensorInterruptEvent(object sender, MpuSensorEventArgs e)
        //{
        //    var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //    {
        //        interruptCount += e.Values.Length;
        //        float samplesPerSecond = (float)interruptCount / (float)((DateTime.Now - startTime).Seconds);

        //        //textBoxStatus.Text = string.Format("{0} {1} {2}", e.Status, e.SamplePeriod, samplesPerSecond);
        //        //textBoxAccel.Text = string.Format("{0}, {1}, {2}", e.Values[0].AccelerationX, e.Values[0].AccelerationY, e.Values[0].AccelerationZ);
        //        //textBoxGyro.Text = string.Format("{0}, {1}, {2}", e.Values[0].GyroX, e.Values[0].GyroY, e.Values[0].GyroZ);
        //    });
        //}
        //private void Page_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    //mpu6050.Dispose();
        //}

    }
}
