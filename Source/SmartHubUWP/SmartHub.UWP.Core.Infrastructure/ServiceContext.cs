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
        public const string DBFileName = "SmartHubUWPDB.sqlite";

        private readonly string storagePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DBFileName);
        private SQLiteConnection db = null;

        public string StoragePath => storagePath;

        public SQLiteConnection StorageGet()
        {
            if (db == null)
                db = new SQLiteConnection(new SQLitePlatformWinRT(), storagePath);

            return db;
        }
        public void StorageClose()
        {
            if (db != null)
            {
                db.Dispose();
                db = null;
            }
        }

        public void StorageSave(object item)
        {
            StorageGet().Insert(item);
        }
        public void StorageSaveOrUpdate(object item)
        {
            StorageGet().InsertOrReplace(item);
        }
        public void StorageDelete(object item)
        {
            StorageGet().Delete(item);
        }
        #endregion
    }
}
