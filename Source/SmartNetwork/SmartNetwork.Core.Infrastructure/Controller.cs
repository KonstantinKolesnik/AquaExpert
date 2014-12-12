using ECM7.Migrator.Framework;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using NLog;
using SmartNetwork.Core.Plugins;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data;
using System.IO;
using System.Reflection;

namespace SmartNetwork.Core.Infrastructure
{
    public class Controller
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        [Import(typeof(IServiceContext))]
        private ServiceContext context;

        private static void InitSessionFactory(ServiceContext context)
        {
            var cfg = new Configuration();

            var mapper = new ConventionModelMapper();

            mapper.BeforeMapClass += (inspector, type, map) =>
                {
                    var idProperty = type.GetProperty("Id");
                    map.Id(idProperty, idMapper => { });
                };

            mapper.BeforeMapProperty += (inspector, propertyPath, map) => map.Column(propertyPath.ToColumnName());
            mapper.BeforeMapManyToOne += (inspector, propertyPath, map) => map.Column(propertyPath.ToColumnName() + "Id");

            foreach (var plugin in context.GetAllPlugins())
            {
                plugin.InitDbModel(mapper);
                cfg.AddAssembly(plugin.GetType().Assembly);
            }

            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();


            cfg.DataBaseIntegration(dbConfig =>
            {
                dbConfig.Dialect<MsSqlCe40Dialect>();
                dbConfig.Driver<SqlServerCeDriver>();
                dbConfig.ConnectionStringName = "common";
            });

            cfg.AddDeserializedMapping(mapping, null);	//Loads nhibernate mappings

            var sessionFactory = cfg.BuildSessionFactory();
            context.InitSessionFactory(sessionFactory);
        }

        public void Init()
        {
            try
            {
                ShadowCopyPlugins();
                LoadPlugins();
                InitSessionFactory(context);

                // обновляем структуру БД
                using (var session = context.OpenSession())
                    foreach (var plugin in context.GetAllPlugins())
                        UpdateDatabase(session.Connection, plugin);

                // инициализируем плагины
                foreach (var plugin in context.GetAllPlugins())
                {
                    logger.Info("Init plugin: {0}", plugin.GetType().FullName);
                    plugin.InitPlugin();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error on plugins initialization", ex);
                throw;
            }
        }

        public void StartServices()
        {
            try
            {
                foreach (var plugin in context.GetAllPlugins())
                {
                    logger.Info("Start plugin {0}", plugin.GetType().FullName);
                    plugin.StartPlugin();
                }

                logger.Info("All plugins are started");
            }
            catch (Exception ex)
            {
                logger.Error("Error on start plugins", ex);
                throw;
            }
        }
        public void StopServices()
        {
            try
            {
                foreach (var plugin in context.GetAllPlugins())
                {
                    logger.Info("Stop plugin {0}", plugin.GetType().FullName);
                    plugin.StopPlugin();
                }

                logger.Info("All plugins are stopped");
            }
            catch (Exception ex)
            {
                logger.Error("Error on stop plugins", ex);
            }
        }

        #region Private
        private void LoadPlugins()
        {
            logger.Info("Load plugins");

            var folders = new HashSet<string>();

            var catalog = new AggregateCatalog(new ApplicationCatalog());

            var spDir = new DirectoryInfo(AppSettings.ShadowedPluginsFullPath);

            foreach (var dir in spDir.GetDirectories())
            {
                var subCatalog = new DirectoryCatalog(dir.FullName);
                catalog.Catalogs.Add(subCatalog);

                folders.Add(dir.FullName);
            }

            AppDomain.CurrentDomain.SetupInformation.PrivateBinPath = string.Join(";", folders);

            var container = new CompositionContainer(catalog);
            container.SatisfyImportsOnce(this);
        }

        private void UpdateDatabase(IDbConnection connection, PluginBase plugin)
        {
            //var assembly = plugin.GetType().Assembly;

            //logger.Info("Update database: {0}", assembly.FullName);

            //// todo: sql
            //var provider = ProviderFactory.Create<SqlServerCeTransformationProvider>(connection, null);
            ////var provider = ProviderFactory.Create<SqlServerTransformationProvider>(connection, null);

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
        public void ShadowCopyPlugins()
        {
            logger.Info("shadow copy plugins");

            var shadowedPlugins = new DirectoryInfo(AppSettings.ShadowedPluginsFullPath);

            if (shadowedPlugins.Exists)
                shadowedPlugins.Delete(true);

            shadowedPlugins.Create();

            // Shadow copy plugins (avoid the CLR locking DLLs)
            var plugins = new DirectoryInfo(AppSettings.PluginsFullPath);

            if (!plugins.Exists)
                plugins.Create();

            CopyTo(plugins, shadowedPlugins);
        }
        private static void CopyTo(DirectoryInfo from, DirectoryInfo to)
        {
            foreach (FileInfo file in from.GetFiles())
            {
                string temppath = Path.Combine(to.FullName, file.Name);
                file.CopyTo(temppath, true);
            }

            foreach (DirectoryInfo dir in from.GetDirectories())
            {
                var subdir = to.CreateSubdirectory(dir.Name);
                CopyTo(dir, subdir);
            }
        }
        #endregion
    }
}
