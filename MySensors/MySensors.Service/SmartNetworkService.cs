using MySensors.Core.Infrastructure;
using System.ServiceProcess;

namespace MySensors.Service
{
    public partial class SmartNetworkService : ServiceBase
    {
        //private readonly HomeApplication app;

        public SmartNetworkService()
        {
            InitializeComponent();

            ControllerEnvironment.Init();

            //app = new HomeApplication();
            //app.Init();
        }

        protected override void OnStart(string[] args)
        {
            //app.StartServices();
        }

        protected override void OnStop()
        {
            //app.StopServices();
        }
    }
}
