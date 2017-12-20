using SQLite.Net.Attributes;
using System;

namespace SmartHub.UWP.Plugins.Wemos.Infrastructure.LineValueSources.Models
{
    public class WemosLineValueSource
    {
        [PrimaryKey, NotNull]
        public string ID
        {
            get; set;
        } = Guid.NewGuid().ToString();
        [NotNull]
        public string LineID
        {
            get; set;
        }
        [NotNull, Default]
        public bool IsEnabled
        {
            get; set;
        }
        [NotNull, Default()]
        public float Value
        {
            get; set;
        }



        public void Process()
        {


        }
    }
}
