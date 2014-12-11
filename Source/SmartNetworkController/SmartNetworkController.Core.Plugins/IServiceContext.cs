using SmartNetworkController.Core.Plugins.Packages;
using NHibernate;
using System.Collections.Generic;

namespace SmartNetworkController.Core.Plugins
{
    public interface IServiceContext
    {
        IControllerPackageManager PackageManager { get; }

        IReadOnlyCollection<PluginBase> GetAllPlugins();

        T GetPlugin<T>() where T : PluginBase;

        ISession OpenSession();
    }
}
