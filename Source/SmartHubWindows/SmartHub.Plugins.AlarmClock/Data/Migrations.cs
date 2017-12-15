using ECM7.Migrator.Framework;
using System.Data;
using ForeignKeyConstraint = ECM7.Migrator.Framework.ForeignKeyConstraint;

[assembly: MigrationAssembly("SmartHub.Plugins.AlarmClock")]

namespace SmartHub.Plugins.AlarmClock.Data
{
    [Migration(1)]
    public class Migration01 : Migration
    {
        public override void Apply()
        {
            Database.AddTable("AlarmClock_AlarmTime",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
                new Column("Name", DbType.String.WithSize(200), ColumnProperty.NotNull),
                new Column("Hours", DbType.Int32, ColumnProperty.NotNull),
                new Column("Minutes", DbType.Int32, ColumnProperty.NotNull),
                new Column("Enabled", DbType.Boolean, ColumnProperty.NotNull, false),
                new Column("UserScriptId", DbType.Guid, ColumnProperty.Null)
            );

            Database.AddForeignKey("AlarmClock_AlarmTime_UserScriptId",
                "AlarmClock_AlarmTime", "UserScriptId",
                "Scripts_UserScript", "Id", ForeignKeyConstraint.Cascade);
        }

        public override void Revert()
        {
            Database.RemoveConstraint("AlarmClock_AlarmTime", "AlarmClock_AlarmTime_UserScriptId");
            Database.RemoveTable("AlarmClock_AlarmTime");
        }
    }
}
