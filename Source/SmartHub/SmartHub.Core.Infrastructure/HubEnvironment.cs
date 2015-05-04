using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace SmartHub.Core.Infrastructure
{
    public static class HubEnvironment
    {
        public static void Init()
        {
            InitCurrentDirectory();
            InitApplicationCulture();

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static void InitCurrentDirectory()
        {
            var path = Assembly.GetEntryAssembly().Location;
            var currentDirectory = Path.GetDirectoryName(path);

            if (string.IsNullOrWhiteSpace(currentDirectory))
                throw new Exception("Current directory is empty");

            Directory.SetCurrentDirectory(currentDirectory);
        }
        private static void InitApplicationCulture()
        {
            Thread.CurrentThread.CurrentCulture =
            Thread.CurrentThread.CurrentUICulture =
            CultureInfo.DefaultThreadCurrentCulture =
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().FullName == args.Name);
        }
    }
}
