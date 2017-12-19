using SQLite.Net.Attributes;
using System;

namespace SmartHub.UWP.Plugins.Scripts.Models
{
    public class UserScript
    {
        //blob, integer, numeric, real, text
        [PrimaryKey, NotNull]
        public string ID
        {
            get; set;
        } = Guid.NewGuid().ToString();
        [NotNull]
        public string Name
        {
            get; set;
        }
        [NotNull]
        public string Body
        {
            get; set;
        } = "alert('Not implemented!');";
    }
}
