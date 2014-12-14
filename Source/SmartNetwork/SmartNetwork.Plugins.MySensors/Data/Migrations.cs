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
            Database.AddTable("MySensors_Node",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
                new Column("NodeID", DbType.Byte, ColumnProperty.NotNull),
                new Column("Type", DbType.Byte, ColumnProperty.NotNull),
                new Column("ProtocolVersion", DbType.String, ColumnProperty.NotNull),
                new Column("SketchName", DbType.String),
                new Column("SketchVersion", DbType.String)
            );
        }

        public override void Revert()
        {
            Database.RemoveTable("MySensors_Node");
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
