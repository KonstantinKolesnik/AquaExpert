using MySensors.Core.Sensors;
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

        public bool IsStarted
        {
            get { return con != null; }
        }

        public void Start()
        {
            if (IsStarted)
                return;

            try
            {
                string dbPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + dbFileName;
                con = new SQLiteConnection(dbFileName);

                con.CreateTable<NodeDto>();
                con.CreateTable<BatteryLevelDto>();
                con.CreateTable<SensorDto>();
                con.CreateTable<SensorValueDto>();
                con.CreateTable<SettingDto>();
            }
            catch (Exception)
            {
                Stop();
            }
        }
        public void Stop()
        {
            con.Close();
            con.Dispose();
            con = null;
        }

        public int Insert(Node item)
        {
            return con.Insert(NodeDto.FromModel(item), "OR REPLACE");
        }
        //public int Insert(List<Node> nodes)
        //{
        //    List<NodeDto> nodes2 = nodes.Select(node => NodeDto.FromModel(node)).ToList();
        //    return con.InsertAll(nodes2);
        //}
        public int Insert(Sensor item)
        {
            return con.Insert(SensorDto.FromModel(item), "OR REPLACE");
        }
        //public int Insert(List<Sensor> sensors)
        //{
        //    List<SensorDto> sensors2 = sensors.Select(sensor => SensorDto.FromModel(sensor)).ToList();
        //    return con.InsertAll(sensors2);
        //}
        public int Insert(BatteryLevel item)
        {
            return con.Insert(BatteryLevelDto.FromModel(item));
        }
        public int Insert(SensorValue item)
        {
            return con.Insert(SensorValueDto.FromModel(item));
        }
        public int Insert(Setting item)
        {
            return con.Insert(SettingDto.FromModel(item), "OR REPLACE");
        }

        public int Update(Node item)
        {
            return con.Update(NodeDto.FromModel(item));
        }
        public int Update(Sensor item)
        {
            return con.Update(SensorDto.FromModel(item));
        }
        public int Update(Setting item)
        {
            return con.Update(SettingDto.FromModel(item));
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
        public List<Setting> GetAllSettings()
        {
            return con.Table<SettingDto>().ToList().Select(item => item.ToModel()).ToList();
        }
    }
}
