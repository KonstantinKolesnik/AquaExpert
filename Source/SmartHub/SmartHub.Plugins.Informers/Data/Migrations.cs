using ECM7.Migrator.Framework;
using System.Data;
using ForeignKeyConstraint = ECM7.Migrator.Framework.ForeignKeyConstraint;

[assembly: MigrationAssembly("SmartHub.Plugins.Informers")]

namespace SmartHub.Plugins.Informers.Data
{
    [Migration(1)]
    public class Migration01 : Migration
    {
        public override void Apply()
        {
            Database.AddTable("Informers_Informers",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
                new Column("Name", DbType.String, ColumnProperty.NotNull),
                new Column("SensorDisplayId", DbType.Guid, ColumnProperty.NotNull),
                new Column("MonitorsIds", DbType.String.WithSize(int.MaxValue), ColumnProperty.Null)
            );
            Database.AddUniqueConstraint("UK_Informers_Informers_Name", "Informers_Informers", "Name");
            Database.AddUniqueConstraint("UK_Informers_Informers_SensorDisplay", "Informers_Informers", "SensorDisplayId");
            Database.AddForeignKey("FK_Informers_Informers_SensorDisplayId", "Informers_Informers", "SensorDisplayId", "MySensors_Sensors", "Id", ForeignKeyConstraint.Cascade);
        }

        public override void Revert()
        {
            Database.RemoveConstraint("Informers_Informers", "UK_Informers_Informers_SensorDisplay");
            Database.RemoveConstraint("Informers_Informers", "UK_Informers_Informers_Name");
            Database.RemoveTable("Informers_Informers");
        }
    }
}
