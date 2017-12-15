using MySensors.Controllers.Core;
using SQLite;

namespace MySensors.Controllers.Data
{
    [Table("Nodes")]
    class NodeDto
    {
        [PrimaryKey]//, AutoIncrement]
        public byte ID { get; set; }
        public byte Type { get; set; }
        public string ProtocolVersion { get; set; }
        public string SketchName { get; set; }
        public string SketchVersion { get; set; }

        //[OneToMany(CascadeOperations = CascadeOperation.All)]      // One to many relationship with Valuation
        //public List<SensorDto> Sensors { get; set; }


        public static NodeDto FromModel(Node item)
        {
            if (item == null)
                return null;

            return new NodeDto()
            {
                ID = item.ID,
                Type = (byte)item.Type,
                ProtocolVersion = item.ProtocolVersion,
                SketchName = item.SketchName,
                SketchVersion = item.SketchVersion
            };
        }
        public Node ToModel()
        {
            return new Node(ID, (SensorType)Type, ProtocolVersion, SketchName, SketchVersion);
        }
    }
}
