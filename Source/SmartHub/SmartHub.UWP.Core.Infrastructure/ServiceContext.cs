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
        public const string StorageFileName = "SmartHubUWPDB.sqlite";

        private readonly string storagePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, StorageFileName);

        public string StoragePath => storagePath;

        public SQLiteConnection StorageOpen()
        {
            return new SQLiteConnection(new SQLitePlatformWinRT(), storagePath);
        }

        public void StorageSave(object item)
        {
            using (var db = StorageOpen())
                db.Insert(item);
        }
        public void StorageSaveOrUpdate(object item)
        {
            using (var db = StorageOpen())
                db.InsertOrReplace(item);
        }
        public void StorageDelete(object item)
        {
            using (var db = StorageOpen())
                db.Delete(item);
        }
        #endregion
    }
}
