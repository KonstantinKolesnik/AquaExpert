using System.ServiceProcess;

namespace MySensors.Service
{
    static class Program
    {
        static void Main()
        {
            ServiceBase.Run(new SmartNetworkService());
        }
    }
}
