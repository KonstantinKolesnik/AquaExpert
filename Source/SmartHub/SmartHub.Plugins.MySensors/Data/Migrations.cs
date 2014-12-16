using ECM7.Migrator.Framework;
using System.Data;
using ForeignKeyConstraint = ECM7.Migrator.Framework.ForeignKeyConstraint;

[assembly: MigrationAssembly("SmartNetwork.Plugins.MySensors")]

namespace SmartHub.Plugins.MySensors.Data
{
    [Migration(1)]
    public class Migration01 : Migration
    {
        public override void Apply()
        {
            // settings
            Database.AddTable("MySensors_Settings",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
                new Column("Name", DbType.String, ColumnProperty.NotNull),
                new Column("Value", DbType.String, ColumnProperty.NotNull)
            );
            Database.AddUniqueConstraint("UK_MySensors_Settings_Name", "MySensors_Settings", "Name");

            // nodes
            Database.AddTable("MySensors_Nodes",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
                new Column("NodeNo", DbType.Byte, ColumnProperty.NotNull),
                new Column("Type", DbType.Byte),
                new Column("ProtocolVersion", DbType.String),
                new Column("SketchName", DbType.String),
                new Column("SketchVersion", DbType.String),
                new Column("Name", DbType.String)
            );
            Database.AddUniqueConstraint("UK_MySensors_Nodes_NodeNo", "MySensors_Nodes", "NodeNo");

            // sensors
            Database.AddTable("MySensors_Sensors",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
                new Column("NodeNo", DbType.Byte, ColumnProperty.NotNull),
                new Column("SensorNo", DbType.Byte, ColumnProperty.NotNull),
                new Column("Type", DbType.Byte, ColumnProperty.NotNull),
                new Column("ProtocolVersion", DbType.String),
                new Column("Name", DbType.String)
            );
            Database.AddUniqueConstraint("UK_MySensors_Sensors_NodeNo_SensorNo", "MySensors_Sensors", "NodeNo", "SensorNo");
            Database.AddForeignKey("FK_MySensors_Sensors_NodeId", "MySensors_Sensors", "NodeNo", "MySensors_Nodes", "NodeNo", ForeignKeyConstraint.Cascade);

            // battery levels
            Database.AddTable("MySensors_BatteryLevels",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
                new Column("NodeNo", DbType.Byte, ColumnProperty.NotNull),
                new Column("TimeStamp", DbType.DateTime, ColumnProperty.NotNull),
                new Column("Level", DbType.Byte)
            );
            Database.AddUniqueConstraint("UK_MySensors_BatteryLevels_NodeNo_TimeStamp", "MySensors_BatteryLevels", "NodeNo", "TimeStamp");
            Database.AddForeignKey("FK_MySensors_BatteryLevels_NodeNo", "MySensors_BatteryLevels", "NodeNo", "MySensors_Nodes", "NodeNo", ForeignKeyConstraint.Cascade);

            // sensor values
            Database.AddTable("MySensors_SensorValues",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
                new Column("NodeNo", DbType.Byte, ColumnProperty.NotNull),
                new Column("SensorNo", DbType.Byte, ColumnProperty.NotNull),
                new Column("TimeStamp", DbType.DateTime, ColumnProperty.NotNull),
                new Column("Type", DbType.Byte, ColumnProperty.NotNull),
                new Column("Value", DbType.Double.WithSize(10, 2))
            );
            Database.AddUniqueConstraint("UK_MySensors_SensorValues_NodeNo_SensorNo_TimeStamp", "MySensors_SensorValues", "NodeNo", "SensorNo", "TimeStamp");
            Database.AddForeignKey("FK_MySensors_SensorValues_NodeNo_SensorNo", "MySensors_SensorValues", new string[] { "NodeNo", "SensorNo" }, "MySensors_Sensors", new string[] { "NodeNo", "SensorNo" }, ForeignKeyConstraint.Cascade);
        }

        public override void Revert()
        {
            // sensor values
            Database.RemoveConstraint("MySensors_SensorValues", "UK_MySensors_SensorValues_NodeNo_SensorNo_TimeStamp");
            Database.RemoveConstraint("MySensors_SensorValues", "FK_MySensors_SensorValues_NodeNo_SensorNo");
            Database.RemoveTable("MySensors_SensorValues");

            // battery levels
            Database.RemoveConstraint("MySensors_BatteryLevels", "UK_MySensors_BatteryLevels_NodeNo_TimeStamp");
            Database.RemoveConstraint("MySensors_BatteryLevels", "FK_MySensors_BatteryLevels_NodeNo");
            Database.RemoveTable("MySensors_BatteryLevels");

            // sensors
            Database.RemoveConstraint("MySensors_Sensors", "UK_MySensors_Sensors_NodeNo_SensorNo");
            Database.RemoveConstraint("MySensors_Sensors", "FK_MySensors_Sensors_NodeId");
            Database.RemoveTable("MySensors_Sensors");

            // nodes
            Database.RemoveConstraint("MySensors_Nodes", "UK_MySensors_Nodes_NodeNo");
            Database.RemoveTable("MySensors_Nodes");

            // configuration
            Database.RemoveConstraint("MySensors_Settings", "UK_MySensors_Settings_Name");
            Database.RemoveTable("MySensors_Settings");
        }
    }
}
