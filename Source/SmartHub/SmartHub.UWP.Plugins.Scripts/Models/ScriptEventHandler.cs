using SQLite.Net.Attributes;

namespace SmartHub.UWP.Plugins.Scripts.Models
{
    public class ScriptEventHandler
    {
        [PrimaryKey, NotNull, AutoIncrement]
        public int ID
        {
            get; set;
        }
        [NotNull]
        public string EventAlias
        {
            get; set;
        }
        [NotNull]
        //[ForeignKey(typeof(UserScript))]
        //[OneToMany(CascadeOperations = CascadeOperation.All)]
        public int UserScriptID
        {
            get; set;
        }
    }
}
