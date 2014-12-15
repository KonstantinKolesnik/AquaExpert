using ECM7.Migrator.Framework;
using System.Data;

[assembly: MigrationAssembly("SmartNetwork.Plugins.MySensors")]

namespace SmartNetwork.Plugins.MySensors.Data
{
    [Migration(1)]
    public class Migration01 : Migration
    {
        public override void Apply()
        {
            Database.AddTable("MySensors_Nodes",
                //new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
                new Column("Id", DbType.Byte, ColumnProperty.PrimaryKey),
                new Column("Type", DbType.Byte),
                new Column("ProtocolVersion", DbType.String),
                new Column("SketchName", DbType.String),
                new Column("SketchVersion", DbType.String)
            );


            Database.AddTable("MySensors_Sensors",
                new Column("Id", DbType.Byte, ColumnProperty.NotNull),
                new Column("NodeId", DbType.Byte, ColumnProperty.NotNull),
                new Column("Type", DbType.Byte, ColumnProperty.NotNull),
                new Column("ProtocolVersion", DbType.String)
            );
            Database.AddForeignKey("FK_MySensors_Sensors_NodeId", "MySensors_Sensors", "NodeId", "MySensors_Nodes", "Id", ECM7.Migrator.Framework.ForeignKeyConstraint.Cascade);
            Database.AddPrimaryKey("PK_MySensors_Sensors", "MySensors_Sensors", new string[] { "NodeId", "Id" });


            Database.AddTable("MySensors_BatteryLevels",
                new Column("Id", DbType.Guid, ColumnProperty.NotNull, "newid()"),
                new Column("NodeId", DbType.Byte, ColumnProperty.NotNull),
                new Column("TimeStamp", DbType.DateTime, ColumnProperty.NotNull),
                new Column("Level", DbType.Byte)
            );
            Database.AddForeignKey("FK_MySensors_BatteryLevels_NodeId", "MySensors_BatteryLevels", "NodeId", "MySensors_Nodes", "Id", ECM7.Migrator.Framework.ForeignKeyConstraint.Cascade);
            //Database.AddPrimaryKey("PK_MySensors_BatteryLevels", "MySensors_BatteryLevels", new string[] { "NodeId", "TimeStamp" });
            Database.AddUniqueConstraint("UK_MySensors_BatteryLevels_TimeStamp", "MySensors_BatteryLevels", "NodeId", "Id", "TimeStamp");


            Database.AddTable("MySensors_SensorValues",
                new Column("NodeId", DbType.Byte, ColumnProperty.NotNull),
                new Column("SensorId", DbType.Byte, ColumnProperty.NotNull),
                new Column("TimeStamp", DbType.DateTime, ColumnProperty.NotNull),
                new Column("Type", DbType.Byte, ColumnProperty.NotNull),
                new Column("Value", DbType.Double)
            );
            Database.AddForeignKey("FK_MySensors_SensorValues_NodeId", "MySensors_SensorValues", new string[] { "NodeId", "SensorId" }, "MySensors_Sensors", new string[] { "NodeId", "Id" }, ECM7.Migrator.Framework.ForeignKeyConstraint.Cascade);
            Database.AddPrimaryKey("PK_MySensors_SensorValues", "MySensors_SensorValues", new string[] { "NodeId", "SensorId", "TimeStamp" });
        }

        public override void Revert()
        {
            Database.RemoveTable("MySensors_SensorValues");
            Database.RemoveTable("MySensors_BatteryLevels");
            Database.RemoveTable("MySensors_Sensors");
            Database.RemoveTable("MySensors_Nodes");
        }
    }


    //[Migration(1)]
    //public class Migration01UserScriptTable : Migration
    //{
    //    public override void Apply()
    //    {
    //        Database.AddTable("AlarmClock_AlarmTime",
    //            new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
    //            new Column("Name", DbType.String.WithSize(200), ColumnProperty.NotNull),
    //            new Column("Hours", DbType.Int32, ColumnProperty.NotNull),
    //            new Column("Minutes", DbType.Int32, ColumnProperty.NotNull),
    //            new Column("Enabled", DbType.Boolean, ColumnProperty.NotNull, false)
    //        );
    //    }
    //    public override void Revert()
    //    {
    //        Database.RemoveTable("AlarmClock_AlarmTime");
    //    }
    //}

    //[Migration(2)]
    //public class Migration02PlaySoundAndScriptId : Migration
    //{
    //    public override void Apply()
    //    {
    //        Database.AddColumn("AlarmClock_AlarmTime",
    //            new Column("UserScriptId", DbType.Guid, ColumnProperty.Null)
    //        );

    //        Database.AddForeignKey("AlarmClock_AlarmTime_UserScriptId",
    //            "AlarmClock_AlarmTime", "UserScriptId", "Scripts_UserScript", "Id", ECM7.Migrator.Framework.ForeignKeyConstraint.Cascade);
    //    }
    //    public override void Revert()
    //    {
    //        Database.RemoveConstraint("AlarmClock_AlarmTime", "AlarmClock_AlarmTime_UserScriptId");
    //        Database.RemoveColumn("AlarmClock_AlarmTime", "UserScriptId");
    //    }
    //}
}
