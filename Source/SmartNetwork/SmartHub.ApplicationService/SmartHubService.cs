using SmartNetwork.Core.Infrastructure;
using System.ServiceProcess;

namespace SmartHub.ApplicationService
{
    public partial class SmartHubService : ServiceBase
    {
        private readonly Controller app;

        public SmartHubService()
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
