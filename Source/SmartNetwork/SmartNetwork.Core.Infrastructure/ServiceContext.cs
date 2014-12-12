using NHibernate;
using SmartNetwork.Core.Plugins;
using SmartNetwork.Core.Plugins.Packages;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace SmartNetwork.Core.Infrastructure
{
    [Export(typeof(IServiceContext))]
    public class ServiceContext : IServiceContext
    {
        #region Plugins
        // todo: переопределить равенство - сравнивать по типу
        [ImportMany(typeof(PluginBase))]
        protected HashSet<PluginBase> Plugins { get; set; }

        public IReadOnlyCollection<PluginBase> GetAllPlugins()
        {
            return new ReadOnlyCollection<PluginBase>(Plugins.ToList());
        }

        public T GetPlugin<T>() where T : PluginBase
        {
            return Plugins.FirstOrDefault(p => p is T) as T;
        }
        #endregion

        [Import(typeof(IControllerPackageManager))]
        public IControllerPackageManager PackageManager { get; protected set; }

        #region Data
        private ISessionFactory sessionFactory;

        public void InitSessionFactory(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }
        public ISession OpenSession()
        {
            return sessionFactory.OpenSession();
        }
        #endregion
    }
}
