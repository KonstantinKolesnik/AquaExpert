using MySensors.Core.Nodes;
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

        public static BatteryLevelDto FromModel(BatteryLevel bl)
        {
            if (bl == null)
                return null;

            return new BatteryLevelDto()
            {
                NodeID = bl.NodeID,
                Time = bl.Time,
                Percent = bl.Percent
            };
        }
        public BatteryLevel ToModel()
        {
            return new BatteryLevel(NodeID, Time, Percent);
        }
    }
}
