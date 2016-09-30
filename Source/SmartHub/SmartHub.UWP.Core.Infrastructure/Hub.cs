using SmartHub.UWP.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Reflection;

namespace SmartHub.UWP.Core.Infrastructure
{
    public class Hub
    {
        #region Fields
        [Import]
        public IServiceContext context
        {
            get; set;
        }
        //private readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        [OnImportsSatisfied]
        public void OnImportsSatisfied()
        {
            var a = 0;
            var b = a;
        }


        #region Public methods
        public void Init(List<Assembly> assemblies)
        {
            //var a = ApplicationData.Current.LocalFolder;
            //StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            //StorageFolder assets = await appInstalledFolder.GetFolderAsync("Assets");
            //var files = await assets.GetFilesAsync();


            //GetType().GetTypeInfo().Assembly.GetReferencedAssemblies()

            //List<Assembly> assemblies = new List<Assembly>()
            //{
            //    //GetType().GetTypeInfo().Assembly,
            //    typeof(IPlugin).GetTypeInfo().Assembly,

            //    Assembly.Load(new AssemblyName("Plugin1")),
            //    Assembly.Load(new AssemblyName("Plugin2")),
            //};

            assemblies.Add(typeof(PluginBase).GetTypeInfo().Assembly);
            assemblies.Add(GetType().GetTypeInfo().Assembly);
            //assemblies.Add(Assembly.Load(new AssemblyName("Plugin1")));

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
        private void LoadPlugins(List<Assembly> assemblies)
        {
            var conventions = new ConventionBuilder();
            conventions
                .ForTypesDerivedFrom<PluginBase>()
                .Export<PluginBase>()
                //.Export()
                //.ExportInterfaces()
                //.Shared()
                ;
            //conventions.ForTypesDerivedFrom<IServiceContext>().Export<IServiceContext>().Shared();
            //conventions.ForType(GetType()).ImportProperty<IServiceContext>(p => p.context);

            //typeof(ServiceContext).GetTypeInfo().Assembly
            var configuration = new ContainerConfiguration()
                //.WithDefaultConventions(conventions)
                .WithAssemblies(assemblies);

            using (var container = configuration.CreateContainer())
            {
                //context = container.GetExport<IServiceContext>();
                //var a = container.GetExports<PluginBase>();
                //var b = a;

                container.SatisfyImports(this);
            }
        }
        #endregion
    }
}
