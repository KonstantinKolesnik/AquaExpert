using MySensors.Core.Sensors;
using SQLite;
using System;

namespace MySensors.Core.Services.Data
{
    [Table("SensorValues")]
    class SensorValueDto
    {
        //[ForeignKey(typeof(NodeDto))]
        public byte NodeID { get; set; }
        //[ForeignKey(typeof(SensorDto))]
        public byte ID { get; set; }
        [PrimaryKey]
        public DateTime Time { get; set; }
        public byte Type { get; set; }
        public float Value { get; set; }

        public static SensorValueDto FromModel(SensorValue item)
        {
            if (item == null)
                return null;

            return new SensorValueDto()
            {
                NodeID = item.NodeID,
                ID = item.ID,
                Time = item.Time,
                Type = (byte)item.Type,
                Value = item.Value
            };
        }
        public SensorValue ToModel()
        {
            return new SensorValue(NodeID, ID, Time, (SensorValueType)Type, Value);
        }
    }
}
