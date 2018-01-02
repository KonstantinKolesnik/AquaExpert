using SmartHub.UWP.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Reflection;

namespace SmartHub.UWP.Core.Infrastructure
{
    public class Hub
    {
        #region Properties
        //private readonly Logger logger = LogManager.GetCurrentClassLogger();
        [Import]
        public IServiceContext Context
        {
            get; internal set;
        }
        #endregion

        //[OnImportsSatisfied]
        //public void OnImportsSatisfied()
        //{
        //    //int a = 0;
        //    //int b = a;
        //}

        #region Public methods
        public void Init(List<Assembly> assemblies = null)
        {
            //var a = ApplicationData.Current.LocalFolder;
            //StorageFolder assets = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
            //var files = await assets.GetFilesAsync();

            //try
            {
                //SqliteEngine.UseWinSqlite3();

                LoadPlugins(assemblies);

                foreach (var plugin in Context.GetAllPlugins())
                    plugin.InitDbModel();

                foreach (var plugin in Context.GetAllPlugins())
                    plugin.InitPlugin();
            }
            //catch (Exception ex)
            //{
            //    //logger.Error(ex, "Error on plugins initialization");
            //    throw;
            //}
        }
        public void StartServices()
        {
            try
            {
                foreach (var plugin in Context.GetAllPlugins())
                {
                    plugin.StartPlugin();
                    //logger.Info("Start plugin {0}", plugin.GetType().FullName);
                }

                //logger.Info("All plugins are started");
            }
            catch (Exception ex)
            {
                //logger.Error(ex, "Error on start plugins");
                throw;
            }
        }
        public void StopServices()
        {
            try
            {
                foreach (var plugin in Context.GetAllPlugins())
                {
                    plugin.StopPlugin();
                    //logger.Info("Stop plugin {0}", plugin.GetType().FullName);
                }

                Context.StorageClose();

                //logger.Info("All plugins are stopped");
            }
            catch (Exception ex)
            {
                //logger.Error(ex, "Error on stop plugins");
                throw;
            }
        }
        #endregion

        #region Private methods
        private void LoadPlugins(List<Assembly> assemblies)
        {
            if (assemblies == null)
                assemblies = new List<Assembly>();
            //assemblies.Add(typeof(PluginBase).GetTypeInfo().Assembly);
            //assemblies.Add(GetType().GetTypeInfo().Assembly);

            //var conventions = new ConventionBuilder();
            //conventions
            //    .ForTypesDerivedFrom<PluginBase>()
            //    .Export<PluginBase>()
            //    //.Export()
            //    //.ExportInterfaces()
            //    //.Shared()
            //    ;
            //conventions
            //    .ForTypesDerivedFrom<IServiceContext>()
            //    .Export<IServiceContext>()
            //    .Shared();

            //conventions.ForType(GetType()).ImportProperty<IServiceContext>(p => p.context);

            var configuration = new ContainerConfiguration()
                //.WithDefaultConventions(conventions)
                .WithAssemblies(assemblies);

            using (var container = configuration.CreateContainer())
                container.SatisfyImports(this);
        }
        #endregion
    }
}
