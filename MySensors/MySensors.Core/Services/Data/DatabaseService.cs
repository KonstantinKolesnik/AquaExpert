using MySensors.Core.Nodes;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MySensors.Core.Services.Data
{
    public class DatabaseService
    {
        private const string dbFileName = "MySensors.dat";
        private SQLiteConnection con = null;

        public bool Start()
        {
            if (con != null)
                return true;

            try
            {
                string dbPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + dbFileName;
                bool exists = File.Exists(dbPath);

                con = new SQLiteConnection(dbFileName);

                if (!exists)
                {
                    con.CreateTable<NodeDto>();
                    con.CreateTable<BatteryLevelDto>();
                    con.CreateTable<SensorDto>();
                    con.CreateTable<SensorValueDto>();
                }
                
                return true;
            }
            catch (Exception) { }

            return false;
        }
        public void Stop()
        {
            con.Close();
            con.Dispose();
            con = null;
        }

        public int Insert(Node node)
        {
            return con.Insert(NodeDto.FromModel(node), "OR REPLACE");
        }
        //public int Insert(List<Node> nodes)
        //{
        //    List<NodeDto> nodes2 = nodes.Select(node => NodeDto.FromModel(node)).ToList();
        //    return con.InsertAll(nodes2);
        //}
        public int Insert(Sensor sensor)
        {
            return con.Insert(SensorDto.FromModel(sensor), "OR REPLACE");
        }
        //public int Insert(List<Sensor> sensors)
        //{
        //    List<SensorDto> sensors2 = sensors.Select(sensor => SensorDto.FromModel(sensor)).ToList();
        //    return con.InsertAll(sensors2);
        //}
        public int Insert(BatteryLevel bl)
        {
            return con.Insert(BatteryLevelDto.FromModel(bl));
        }
        public int Insert(SensorValue sv)
        {
            return con.Insert(SensorValueDto.FromModel(sv));
        }

        public int Update(Node node)
        {
            return con.Update(NodeDto.FromModel(node));
        }
        public int Update(Sensor sensor)
        {
            return con.Update(SensorDto.FromModel(sensor));
        }

        public List<Node> GetAllNodes()
        {
            return con.Table<NodeDto>().ToList().Select(item => item.ToModel()).ToList();
        }
        public List<Sensor> GetAllSensors()
        {
            return con.Table<SensorDto>().ToList().Select(item => item.ToModel()).ToList();
        }
        public List<BatteryLevel> GetAllBatteryLevels()
        {
            return con.Table<BatteryLevelDto>().ToList().Select(item => item.ToModel()).ToList();
        }
        public List<SensorValue> GetAllSensorValues()
        {
            return con.Table<SensorValueDto>().ToList().Select(item => item.ToModel()).ToList();
        }
    }
}
