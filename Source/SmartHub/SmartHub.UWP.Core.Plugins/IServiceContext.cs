using System.Collections.Generic;

namespace SmartHub.UWP.Core.Plugins
{
    public interface IServiceContext
    {
        #region Plugins
        IReadOnlyCollection<PluginBase> GetAllPlugins();
        T GetPlugin<T>() where T : PluginBase;
        #endregion

        bool IsServer { get; set; }

        #region Database
        //ISession OpenSession();
        //IStatelessSession OpenStatelessSession();
        #endregion
    }
}
