using MySensors.Core.Nodes;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace MySensors.Core.Services.Data
{
    [Table("Sensors")]
    class SensorDto
    {
        [PrimaryKey]
        public int PK { get; set; }
        [ForeignKey(typeof(NodeDto))]     // Specify the foreign key
        public byte NodeID { get; set; }
        public byte ID { get; set; }
        public byte Type { get; set; }
        public string ProtocolVersion { get; set; }

        //[ManyToOne]      // Many to one relationship with Stock
        //public NodeDto Node { get; set; }

        public static SensorDto FromModel(Sensor sensor)
        {
            if (sensor == null)
                return null;

            return new SensorDto()
            {
                PK = sensor.NodeID << 8 + sensor.ID,
                NodeID = sensor.NodeID,
                ID = sensor.ID,
                Type = (byte)sensor.Type,
                ProtocolVersion = sensor.ProtocolVersion
            };
        }
        public Sensor ToModel()
        {
            return new Sensor(NodeID, ID)
            {
                Type = (SensorType)Type,
                ProtocolVersion = ProtocolVersion
            };
        }
    }
}
