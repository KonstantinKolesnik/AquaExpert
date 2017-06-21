using System;

namespace SmartHub.UWP.Plugins.Storage.Models
{
    public class StorageInfo
    {
        public ulong Size
        {
            get; set;
        }
        public DateTimeOffset DateModified
        {
            get; set;
        }
        public DateTimeOffset ItemDate
        {
            get; set;
        }
    }
}
