using MySensors.Core;
using SQLite;

namespace MySensors.Controllers.Data
{
    [Table("Settings")]
    class SettingDto
    {
        [PrimaryKey]
        public string Name { get; set; }
        public string Value { get; set; }

        public static SettingDto FromModel(Setting item)
        {
            if (item == null)
                return null;

            return new SettingDto()
            {
                Name = item.Name,
                Value = item.Value
            };
        }
        public Setting ToModel()
        {
            return new Setting(Name, Value);
        }
    }
}
