using SmartHub.UWP.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        #region Public methods
        public void Init()
        {
            //var a = ApplicationData.Current.LocalFolder;
            //StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            //StorageFolder assets = await appInstalledFolder.GetFolderAsync("Assets");
            //var files = await assets.GetFilesAsync();

            try
            {
                LoadPlugins();

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
        private void LoadPlugins()
        {
            //typeof(ServiceContext).GetTypeInfo().Assembly
            var configuration = new ContainerConfiguration()
                //.WithParts(typeof(IServiceContext), typeof(PluginBase))
                .WithPart<IServiceContext>()
                //.WithPart<PluginBase>()
                ;
            using (var container = configuration.CreateContainer())
            {
                //context = container.GetExport<IServiceContext>();
                //var a = container.GetExports<PluginBase>();
                //var aa = container.GetExport<PluginBase>();
                //var b = a;

                container.SatisfyImports(this);
            }

            //--------------------------------------------------------------------

            //var conventions = new ConventionBuilder();
            //conventions.ForTypesDerivedFrom<IServiceContext>().Export<IServiceContext>().Shared();
            //conventions.ForType(GetType()).ImportProperty<IServiceContext>(p => p.context);

            //var configuration = new ContainerConfiguration()
            //    .WithDefaultConventions(conventions)
            //    .WithAssemblies(GetAssemblies("c:/addins"));

            //using (CompositionHost host = configuration.CreateContainer())
            //{
            //    host.SatisfyImports(this, conventions);
            //}
        }
        #endregion
    }
}
