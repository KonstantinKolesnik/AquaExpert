using NHibernate;
using SmartHub.Core.Plugins.HubPackages;
using System.Collections.Generic;

namespace SmartHub.Core.Plugins
{
    public interface IServiceContext
    {
        IHubPackageManager PackageManager { get; }

        IReadOnlyCollection<PluginBase> GetAllPlugins();

        T GetPlugin<T>() where T : PluginBase;

        ISession OpenSession();
    }
}
