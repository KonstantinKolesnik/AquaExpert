using ECM7.Migrator.Framework;
using System.Data;

[assembly: MigrationAssembly("SmartHub.Plugins.AquaController")]

namespace SmartHub.Plugins.AquaController.Data
{
    [Migration(1)]
    public class Migration01 : Migration
    {
        public override void Apply()
        {
            // settings
            Database.AddTable("AquaController_Settings",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
                new Column("Name", DbType.String, ColumnProperty.NotNull),
                new Column("SerializedValue", DbType.String.WithSize(int.MaxValue), ColumnProperty.NotNull)
            );
            Database.AddUniqueConstraint("UK_AquaController_Settings_Name", "AquaController_Settings", "Name");
        }

        public override void Revert()
        {
            // settings
            Database.RemoveConstraint("AquaController_Settings", "UK_AquaController_Settings_Name");
            Database.RemoveTable("AquaController_Settings");
        }
    }
}
