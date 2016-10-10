using System.Collections.Generic;

namespace SmartHub.UWP.Core.Plugins
{
    public interface IServiceContext
    {
        IReadOnlyCollection<PluginBase> GetAllPlugins();
        T GetPlugin<T>() where T : PluginBase;

        //ISession OpenSession();
        //IStatelessSession OpenStatelessSession();
    }
}
