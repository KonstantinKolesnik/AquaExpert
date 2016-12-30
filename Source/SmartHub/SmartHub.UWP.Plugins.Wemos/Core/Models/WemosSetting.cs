using SQLite.Net.Attributes;
using System;

namespace SmartHub.UWP.Plugins.Wemos.Core.Models
{
    public class WemosSetting
    {
        [PrimaryKey, NotNull]
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
