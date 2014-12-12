using NHibernate;
using SmartNetwork.Core.Plugins.Packages;
using System.Collections.Generic;

namespace SmartNetwork.Core.Plugins
{
    public interface IServiceContext
    {
        IControllerPackageManager PackageManager { get; }

        IReadOnlyCollection<PluginBase> GetAllPlugins();

        T GetPlugin<T>() where T : PluginBase;

        ISession OpenSession();
    }
}
