using ECM7.Migrator.Framework;
using System.Data;

[assembly: MigrationAssembly("SmartHub.Plugins.MeteoStation")]

namespace SmartHub.Plugins.MeteoStation.Data
{
    [Migration(1)]
    public class Migration01 : Migration
    {
        public override void Apply()
        {
            // settings
            Database.AddTable("MeteoStation_Settings",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
                new Column("Name", DbType.String, ColumnProperty.NotNull),
                new Column("SerializedValue", DbType.String.WithSize(int.MaxValue), ColumnProperty.NotNull)
            );
            Database.AddUniqueConstraint("UK_MeteoStation_Settings_Name", "MeteoStation_Settings", "Name");
        }

        public override void Revert()
        {
            // settings
            Database.RemoveConstraint("MeteoStation_Settings", "UK_MeteoStation_Settings_Name");
            Database.RemoveTable("MeteoStation_Settings");
        }
    }
}
