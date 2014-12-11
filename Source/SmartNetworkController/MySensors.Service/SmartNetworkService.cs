using MySensors.Core.Infrastructure;
using System.ServiceProcess;

namespace MySensors.Service
{
    public partial class SmartNetworkService : ServiceBase
    {
        private readonly Controller app;

        public SmartNetworkService()
        {
            InitializeComponent();

            ControllerEnvironment.Init();

            app = new Controller();
            app.Init();
        }

        protected override void OnStart(string[] args)
        {
            app.StartServices();
        }

        protected override void OnStop()
        {
            app.StopServices();
        }
    }
}
