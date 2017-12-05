using SQLite.Net.Attributes;
using System;

namespace SmartHub.UWP.Plugins.Scripts.Models
{
    public class UserScript
    {
        [PrimaryKey, NotNull, AutoIncrement]
        public int ID
        {
            get; set;
        }
        [NotNull]
        public string Name
        {
            get; set;
        }
        [NotNull]
        public string Body
        {
            get; set;
        }
    }
}
