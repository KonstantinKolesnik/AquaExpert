using MySensors.Core.Plugins.Packages;
using NHibernate;
using System.Collections.Generic;

namespace MySensors.Core.Plugins
{
    public interface IServiceContext
    {
        IControllerPackageManager PackageManager { get; }

        IReadOnlyCollection<PluginBase> GetAllPlugins();

        T GetPlugin<T>() where T : PluginBase;

        ISession OpenSession();
    }
}
