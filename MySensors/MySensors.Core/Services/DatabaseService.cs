using MySensors.Core.Nodes;
using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MySensors.Core.Services
{
    public class DatabaseService
    {
        private const string dbFileName = "MySensors.dat";
        private SQLiteConnection con;

        public bool Start()
        {
            string dbPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + dbFileName;
            bool exists = File.Exists(dbPath);
            con = new SQLiteConnection(dbFileName);

            if (!exists)
            {
                con.CreateTable<NodeDto>();
                con.CreateTable<SensorDto>();


                //var node = new NodeDto()
                //{
                //    ID = 1,
                //    Type = (byte)SensorType.ArduinoNode
                //};
                //con.Insert(node);   // Insert the object in the database

                //var sensor = new SensorDto()
                //{
                //    ID = 0,
                //    Type = (byte)SensorType.Heater
                //};
                //con.Insert(sensor);   // Insert the object in the database

                // Objects created, let's stablish the relationship
                //node.Sensors = new List<SensorDto> { sensor };

                //con.UpdateWithChildren(node);   // Update the changes into the database
                //if (sensor.Node == node)
                //{
                //    Debug.WriteLine("Inverse relationship already set, yay!");
                //}

                //// Get the object and the relationships
                //var storedSensor = con.GetWithChildren<SensorDto>(sensor.ID);
                //if (node.Type.Equals(storedSensor.Node.Type))
                //{
                //    Debug.WriteLine("Object and relationships loaded correctly!");
                //}
            }

            //NodeDto n = con.Get<NodeDto>(1);



            //con.Get()


            return true;
        }

        public int Insert(Node node)
        {
            return con.Insert(NodeDto.FromModel(node));
        }
        public int Insert(List<Node> nodes)
        {
            List<NodeDto> nodes2 = nodes.Select(node => NodeDto.FromModel(node)).ToList();
            return con.InsertAll(nodes2);
        }
        public int Insert(Sensor sensor)
        {
            return con.Insert(SensorDto.FromModel(sensor));
        }
        public int Insert(List<Sensor> sensors)
        {
            List<SensorDto> sensors2 = sensors.Select(sensor => SensorDto.FromModel(sensor)).ToList();
            return con.InsertAll(sensors2);
        }

        public int Update(Node node)
        {
            return con.Update(NodeDto.FromModel(node));
        }
        public int Update(Sensor sensor)
        {
            return con.Update(SensorDto.FromModel(sensor));
        }
    }
}
