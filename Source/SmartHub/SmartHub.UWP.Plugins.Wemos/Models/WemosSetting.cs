using SQLite.Net.Attributes;
using System;

namespace SmartHub.UWP.Plugins.Wemos.Models
{
    public class WemosSetting
    {
        //public Guid Id { get; set; } = new Guid();
        [PrimaryKey, NotNull]
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
