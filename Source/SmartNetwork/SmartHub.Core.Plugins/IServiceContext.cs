using NHibernate;
using SmartHub.Core.Plugins.Packages;
using System.Collections.Generic;

namespace SmartHub.Core.Plugins
{
    public interface IServiceContext
    {
        IControllerPackageManager PackageManager { get; }

        IReadOnlyCollection<PluginBase> GetAllPlugins();

        T GetPlugin<T>() where T : PluginBase;

        ISession OpenSession();
    }
}
