using SQLite.Net.Attributes;

namespace SmartHub.UWP.Plugins.Wemos.Core.Models
{
    public class WemosSetting
    {
        [PrimaryKey, NotNull]
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
