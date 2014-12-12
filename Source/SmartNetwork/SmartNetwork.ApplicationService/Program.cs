using System.ServiceProcess;

namespace SmartNetwork.ApplicationService
{
    static class Program
    {
        static void Main()
        {
            ServiceBase.Run(new SmartNetworkService());
        }
    }
}
