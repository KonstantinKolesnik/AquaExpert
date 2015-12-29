using SmartHub.Core.Infrastructure;
using System.ServiceProcess;

namespace SmartHub.ApplicationService
{
    public partial class SmartHubService : ServiceBase
    {
        private readonly Hub hub;

        public SmartHubService()
        {
            InitializeComponent();

            HubEnvironment.Init();

            hub = new Hub();
            hub.Init();
        }

        protected override void OnStart(string[] args)
        {
            hub.StartServices();
        }

        protected override void OnStop()
        {
            hub.StopServices();
        }
    }
}
