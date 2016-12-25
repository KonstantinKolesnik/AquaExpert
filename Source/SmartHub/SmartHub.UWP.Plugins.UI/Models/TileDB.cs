using SQLite.Net.Attributes;

namespace SmartHub.UWP.Plugins.UI.Models
{
    public class TileDB
    {
        [PrimaryKey, NotNull, AutoIncrement]
        public int ID { get; set; }

        [NotNull]
        public string TypeFullName { get; set; }
        [NotNull]
        public int SortOrder { get; set; }
        [NotNull]
        public string SerializedParameters { get; set; }

        //public virtual dynamic GetParameters()
        //{
        //    var json = string.IsNullOrWhiteSpace(SerializedParameters) ? "{}" : SerializedParameters;
        //    return Extensions.FromJson(json);
        //}
        //public virtual void SetParameters(object parameters)
        //{
        //    SerializedParameters = parameters.ToJson();
        //}
    }
}
