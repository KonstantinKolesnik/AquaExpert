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
        #region Fields
        [Import]
        private IServiceContext context
        {
            get; set;
        }
        //private readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        //[OnImportsSatisfied]
        //public void OnImportsSatisfied()
        //{
        //    var a = 0;
        //    var b = a;
        //}

        #region Public methods
        public void Init(List<Assembly> assemblies = null)
        {
            //var a = ApplicationData.Current.LocalFolder;
            //StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            //StorageFolder assets = await appInstalledFolder.GetFolderAsync("Assets");
            //var files = await assets.GetFilesAsync();

            try
            {
                LoadPlugins(assemblies);

                foreach (var plugin in context.GetAllPlugins())
                    plugin.InitPlugin();
            }
            catch (Exception ex)
            {
                //logger.Error(ex, "Error on plugins initialization");
                throw;
            }
        }
        public void StartServices()
        {
            try
            {
                foreach (var plugin in context.GetAllPlugins())
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
                foreach (var plugin in context.GetAllPlugins())
                {
                    plugin.StopPlugin();
                    //logger.Info("Stop plugin {0}", plugin.GetType().FullName);
                }

                //logger.Info("All plugins are stopped");
            }
            catch (Exception ex)
            {
                //logger.Error(ex, "Error on stop plugins");
            }
        }
        #endregion

        #region Private methods
        private void LoadPlugins(List<Assembly> assemblies = null)
        {
            assemblies = assemblies ?? new List<Assembly>();
            assemblies.Add(typeof(PluginBase).GetTypeInfo().Assembly);
            assemblies.Add(GetType().GetTypeInfo().Assembly);

            //var conventions = new ConventionBuilder();
            //conventions
            //    .ForTypesDerivedFrom<PluginBase>()
            //    .Export<PluginBase>()
            //    //.Export()
            //    //.ExportInterfaces()
            //    //.Shared()
            //    ;
            //conventions.ForTypesDerivedFrom<IServiceContext>().Export<IServiceContext>().Shared();
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
