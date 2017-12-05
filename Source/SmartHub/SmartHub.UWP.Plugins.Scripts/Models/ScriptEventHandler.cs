using SQLite.Net.Attributes;
using System;

namespace SmartHub.UWP.Plugins.Scripts.Models
{
    public class ScriptEventHandler
    {
        [PrimaryKey, NotNull]//, AutoIncrement]
        public string ID
        {
            get; set;
        } = Guid.NewGuid().ToString();
        [NotNull]
        public string EventAlias
        {
            get; set;
        }
        [NotNull]
        //[ForeignKey(typeof(UserScript))]
        //[OneToMany(CascadeOperations = CascadeOperation.All)]
        public string UserScriptID
        {
            get; set;
        }
    }
}
