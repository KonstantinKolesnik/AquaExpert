using System.ServiceProcess;

namespace SmartHub.ApplicationService
{
    static class Program
    {
        static void Main()
        {
            ServiceBase.Run(new SmartHubService());
        }
    }
}
