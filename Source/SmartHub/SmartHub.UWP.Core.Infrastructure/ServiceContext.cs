using SmartHub.UWP.Core.Plugins;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;

namespace SmartHub.UWP.Core.Infrastructure
{
    [Export(typeof(IServiceContext))]
    public class ServiceContext : IServiceContext
    {
        #region Plugins
        [ImportMany]
        protected IEnumerable<PluginBase> Plugins
        {
            get; set;
        }

        [OnImportsSatisfied]
        public void OnImportsSatisfied()
        {
            var a = 0;
            var b = a;
        }

        public ServiceContext()
        {
            var configuration = new ContainerConfiguration()
                .WithPart<PluginBase>();

            using (CompositionHost host = configuration.CreateContainer())
                host.SatisfyImports(this);
        }

        public IReadOnlyCollection<PluginBase> GetAllPlugins()
        {
            return new ReadOnlyCollection<PluginBase>(Plugins.ToList());
        }
        public T GetPlugin<T>() where T : PluginBase
        {
            return Plugins.FirstOrDefault(p => p is T) as T;
        }
        #endregion





        //[Import(typeof(IHubPackageManager))]
        //public IHubPackageManager PackageManager { get; protected set; }

        #region Data
        //private ISessionFactory sessionFactory;

        //public void InitSessionFactory(ISessionFactory sessionFactory)
        //{
        //    this.sessionFactory = sessionFactory;
        //}
        //public ISession OpenSession()
        //{
        //    return sessionFactory.OpenSession();
        //}
        //public IStatelessSession OpenStatelessSession()
        //{
        //    return sessionFactory.OpenStatelessSession();
        //}
        #endregion
    }
}
