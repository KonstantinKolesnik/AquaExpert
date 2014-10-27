using MySensors.Core.Nodes;
using SQLite.Net.Attributes;

namespace MySensors.Core.Services
{
    [Table("Nodes")]
    class NodeDto
    {
        [PrimaryKey]//, AutoIncrement]
        public int ID { get; set; }
        public int Type { get; set; }
        public string ProtocolVersion { get; set; }
        public string SketchName { get; set; }
        public string SketchVersion { get; set; }

        //[OneToMany(CascadeOperations = CascadeOperation.All)]      // One to many relationship with Valuation
        //public List<SensorDto> Sensors { get; set; }


        public static NodeDto FromModel(Node node)
        {
            if (node == null)
                return null;

            return new NodeDto()
            {
                ID = node.ID,
                Type = (byte)node.Type,
                ProtocolVersion = node.ProtocolVersion,
                SketchName = node.SketchName,
                SketchVersion = node.SketchVersion
            };
        }
        public Node ToModel()
        {
            return new Node((byte)ID)
            {
                Type = (SensorType)Type,
                ProtocolVersion = ProtocolVersion,
                SketchName = SketchName,
                SketchVersion = SketchVersion
            };
        }
    }
}
