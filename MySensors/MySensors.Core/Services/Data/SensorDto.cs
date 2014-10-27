using MySensors.Core.Nodes;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace MySensors.Core.Services.Data
{
    [Table("Sensors")]
    class SensorDto
    {
        [ForeignKey(typeof(NodeDto))]     // Specify the foreign key
        public int NodeID { get; set; }
        public int ID { get; set; }
        public int Type { get; set; }
        public string ProtocolVersion { get; set; }

        //[ManyToOne]      // Many to one relationship with Stock
        //public NodeDto Node { get; set; }

        public static SensorDto FromModel(Sensor sensor)
        {
            if (sensor == null)
                return null;

            return new SensorDto()
            {
                NodeID = sensor.NodeID,
                ID = sensor.ID,
                Type = (byte)sensor.Type,
                ProtocolVersion = sensor.ProtocolVersion
            };
        }
        public Sensor ToModel()
        {
            return new Sensor((byte)NodeID, (byte)ID)
            {
                Type = (SensorType)Type,
                ProtocolVersion = ProtocolVersion
            };
        }
    }
}
