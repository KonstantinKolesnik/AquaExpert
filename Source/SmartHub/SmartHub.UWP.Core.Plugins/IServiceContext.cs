using SQLite.Net;
using System.Collections.Generic;

namespace SmartHub.UWP.Core.Plugins
{
    public interface IServiceContext
    {
        #region Plugins
        IReadOnlyCollection<PluginBase> GetAllPlugins();
        T GetPlugin<T>() where T : PluginBase;
        #endregion

        #region Database
        string StoragePath
        {
            get;
        }

        SQLiteConnection StorageOpen();
        void StorageClose();

        void StorageSave(object item);
        void StorageSaveOrUpdate(object item);
        void StorageDelete(object item);
        #endregion
    }
}
