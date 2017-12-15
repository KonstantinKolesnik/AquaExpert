using ECM7.Migrator.Framework;
using System.Data;
using ForeignKeyConstraint = ECM7.Migrator.Framework.ForeignKeyConstraint;

[assembly: MigrationAssembly("SmartHub.Plugins.Zones")]

namespace SmartHub.Plugins.Zones.Data
{
    [Migration(1)]
    public class Migration01 : Migration
    {
        public override void Apply()
        {
            Database.AddTable("Zones_Zones",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
                new Column("Name", DbType.String, ColumnProperty.NotNull),
                new Column("MonitorsList", DbType.String.WithSize(int.MaxValue), ColumnProperty.Null),
                new Column("ControllersList", DbType.String.WithSize(int.MaxValue), ColumnProperty.Null),
                new Column("ScriptsList", DbType.String.WithSize(int.MaxValue), ColumnProperty.Null),
                new Column("GraphsList", DbType.String.WithSize(int.MaxValue), ColumnProperty.Null)
            );
            Database.AddUniqueConstraint("UK_Zones_Zones_Name", "Zones_Zones", "Name");
        }

        public override void Revert()
        {
            Database.RemoveConstraint("Zones_Zones", "UK_Zones_Zones_Name");
            Database.RemoveTable("Zones_Zones");
        }
    }
}
