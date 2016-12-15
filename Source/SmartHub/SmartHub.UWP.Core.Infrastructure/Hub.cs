using SmartHub.UWP.Core.Plugins;
using SQLite.Net;
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
        public IServiceContext Context
        {
            get; internal set;
        }
        //private readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        [OnImportsSatisfied]
        public void OnImportsSatisfied()
        {
            //int a = 0;
            //int b = a;
        }

        #region Public methods
        public void Init(List<Assembly> assemblies = null)
        {
            //var a = ApplicationData.Current.LocalFolder;
            //StorageFolder assets = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
            //var files = await assets.GetFilesAsync();

            try
            {
                LoadPlugins(assemblies);

                InitSessionFactory(Context);
                // обновляем структуру БД
                //using (var session = Context.OpenSession())
                //    foreach (var plugin in Context.GetAllPlugins())
                //        UpdateDatabase(session.Connection, plugin);

                foreach (var plugin in Context.GetAllPlugins())
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
            if (assemblies == null)
                assemblies = new List<Assembly>();
            //assemblies.Add(typeof(PluginBase).GetTypeInfo().Assembly);
            //assemblies.Add(GetType().GetTypeInfo().Assembly);

            var conventions = new ConventionBuilder();
            conventions
                .ForTypesDerivedFrom<PluginBase>()
                .Export<PluginBase>()
                //.Export()
                //.ExportInterfaces()
                //.Shared()
                ;
            conventions
                .ForTypesDerivedFrom<IServiceContext>()
                .Export<IServiceContext>()
                .Shared();

            //conventions.ForType(GetType()).ImportProperty<IServiceContext>(p => p.context);

            var configuration = new ContainerConfiguration()
                .WithDefaultConventions(conventions)
                .WithAssemblies(assemblies);

            using (var container = configuration.CreateContainer())
                container.SatisfyImports(this);
        }

        private static void InitSessionFactory(IServiceContext context)
        {
            //var mapper = new ConventionModelMapper();
            //mapper.BeforeMapClass += (inspector, type, map) => { var idProperty = type.GetProperty("Id"); map.Id(idProperty, idMapper => { }); };
            //mapper.BeforeMapProperty += (inspector, propertyPath, map) => map.Column(propertyPath.ToColumnName());
            //mapper.BeforeMapManyToOne += (inspector, propertyPath, map) => map.Column(propertyPath.ToColumnName() + "Id");

            //var cfg = new Configuration();
            foreach (var plugin in context.GetAllPlugins())
            {
                plugin.InitDbModel();// mapper);
                //cfg.AddAssembly(plugin.GetType().Assembly);
            }
            //cfg.DataBaseIntegration(dbConfig =>
            //{
            //    dbConfig.Dialect<MsSqlCe40Dialect>();
            //    dbConfig.Driver<SqlServerCeDriver>();
            //    dbConfig.ConnectionStringName = "common";
            //});

            //var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
            //cfg.AddDeserializedMapping(mapping, null);	//Loads nhibernate mappings

            //var sessionFactory = cfg.BuildSessionFactory();
            //context.InitSessionFactory(sessionFactory);
        }
        private void UpdateDatabase(SQLiteConnection connection, PluginBase plugin)
        {
            //var assembly = plugin.GetType().Assembly;

            //logger.Info("Update database: {0}", assembly.FullName);

            //// todo: sql
            //var provider = ProviderFactory.Create<SqlServerCeTransformationProvider>(connection, null);

            //using (var migrator = new Migrator(provider, assembly))
            //{
            //    // запрещаем выполнять миграции, для которых не указано "пространство имен"
            //    if (migrator.AvailableMigrations.Any())
            //    {
            //        var migrationsInfo = assembly.GetCustomAttribute<MigrationAssemblyAttribute>();
            //        if (migrationsInfo == null || string.IsNullOrWhiteSpace(migrationsInfo.Key))
            //            logger.Error("Assembly {0} contains invalid migration info", assembly.FullName);
            //    }

            //    migrator.Migrate();
            //}
        }
        #endregion
    }
}
