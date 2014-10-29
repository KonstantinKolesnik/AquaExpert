using MySensors.Core.Sensors;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace MySensors.Core.Services.Data
{
    [Table("BatteryLevels")]
    class BatteryLevelDto
    {
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
                NodeID = item.NodeID,
                Time = item.Time,
                Percent = item.Percent
            };
        }
        public BatteryLevel ToModel()
        {
            return new BatteryLevel(NodeID, Time, Percent);
        }
    }
}
