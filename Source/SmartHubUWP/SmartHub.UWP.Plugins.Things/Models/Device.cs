using SQLite.Net.Attributes;

namespace SmartHub.UWP.Plugins.Things.Models
{
    public class Device
    {
        [PrimaryKey, NotNull]
        public string ID
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public string IPAddress
        {
            get; set;
        }
        [NotNull]
        public DeviceType Type
        {
            get; set;
        } = DeviceType.Unknown;
    }
}
