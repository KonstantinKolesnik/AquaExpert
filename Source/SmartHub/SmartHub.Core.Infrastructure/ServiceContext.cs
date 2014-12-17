﻿using NHibernate;
using SmartHub.Core.Plugins;
using SmartHub.Core.Plugins.HubPackages;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace SmartHub.Core.Infrastructure
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

        [Import(typeof(IHubPackageManager))]
        public IHubPackageManager PackageManager { get; protected set; }

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
