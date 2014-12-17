using SmartHub.Core.Infrastructure;
using System.ServiceProcess;

namespace SmartHub.ApplicationService
{
    public partial class SmartHubService : ServiceBase
    {
        private readonly Hub app;

        public SmartHubService()
        {
            InitializeComponent();

            HubEnvironment.Init();

            app = new Hub();
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
