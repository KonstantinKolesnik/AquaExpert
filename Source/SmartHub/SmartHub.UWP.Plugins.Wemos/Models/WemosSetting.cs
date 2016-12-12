using System;

namespace SmartHub.UWP.Plugins.Wemos.Models
{
    public class WemosSetting
    {
        public Guid Id { get; set; } = new Guid();
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
