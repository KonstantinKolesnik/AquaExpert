using MySensors.Core.Sensors;
using SQLite;
using System;

namespace MySensors.Core.Services.Data
{
    [Table("BatteryLevels")]
    class BatteryLevelDto
    {
        [PrimaryKey]
        public string PK { get; set; }
        //[ForeignKey(typeof(NodeDto))]
        public byte NodeID { get; set; }
        public DateTime Time { get; set; }
        public byte Percent { get; set; }

        public static BatteryLevelDto FromModel(BatteryLevel item)
        {
            if (item == null)
                return null;

            return new BatteryLevelDto()
            {
                PK = GetPK(item.Time),
                NodeID = item.NodeID,
                Time = item.Time,
                Percent = item.Percent
            };
        }
        public BatteryLevel ToModel()
        {
            return new BatteryLevel(NodeID, Time, Percent);
        }

        private static string GetPK(DateTime dt)
        {
            long ticks = dt.Ticks;
            byte[] bytes = BitConverter.GetBytes(ticks);
            return Convert.ToBase64String(bytes)
                                    .Replace('+', '_')
                                    .Replace('/', '-')
                                    .TrimEnd('=');
        }
    }
}
