using SmartHub.UWP.Core.Plugins;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Composition;
using System.IO;
using System.Linq;
using Windows.Storage;

namespace SmartHub.UWP.Core.Infrastructure
{
    [Export(typeof(IServiceContext))]
    [Shared]
    public class ServiceContext : IServiceContext
    {
        #region Plugins
        [ImportMany]
        public IEnumerable<PluginBase> Plugins
        {
            get; set;
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

        #region Database
        private static string dbPath = string.Empty;
        private static string DbPath
        {
            get
            {
                if (string.IsNullOrEmpty(dbPath))
                    dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "SmartHubUWPDB.sqlite");

                return dbPath;
            }
        }

        public SQLiteConnection OpenConnection()
        {
            return new SQLiteConnection(new SQLitePlatformWinRT(), DbPath);
        }
        #endregion
    }
}
