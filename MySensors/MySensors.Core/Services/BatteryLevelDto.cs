using MySensors.Core.Nodes;
using SQLiteNetExtensions.Attributes;
using System;

namespace MySensors.Core.Services
{
    class BatteryLevelDto
    {
        [ForeignKey(typeof(NodeDto))]     // Specify the foreign key
        public int NodeID { get; set; }
        public DateTime Time { get; set; }
        public int Percent { get; set; }

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
            return new BatteryLevel((byte)NodeID, Time, (byte)Percent);
        }
    }
}
