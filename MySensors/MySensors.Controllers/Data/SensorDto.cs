using MySensors.Controllers.Core;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace MySensors.Controllers.Data
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

        public static SensorDto FromModel(Sensor item)
        {
            if (item == null)
                return null;

            return new SensorDto()
            {
                PK = item.NodeID << 8 + item.ID,
                NodeID = item.NodeID,
                ID = item.ID,
                Type = (byte)item.Type,
                ProtocolVersion = item.ProtocolVersion
            };
        }
        public Sensor ToModel()
        {
            return new Sensor(NodeID, ID, (SensorType)Type, ProtocolVersion);
        }
    }
}
