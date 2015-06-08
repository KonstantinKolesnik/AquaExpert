using ECM7.Migrator.Framework;
using System.Data;
using ForeignKeyConstraint = ECM7.Migrator.Framework.ForeignKeyConstraint;

[assembly: MigrationAssembly("SmartHub.Plugins.Controllers")]

namespace SmartHub.Plugins.Controllers.Data
{
    [Migration(1)]
    public class Migration01 : Migration
    {
        public override void Apply()
        {
            Database.AddTable("Controllers_Controllers",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
                new Column("Name", DbType.String, ColumnProperty.NotNull),
                new Column("Type", DbType.Byte, ColumnProperty.NotNull),
                new Column("Configuration", DbType.String.WithSize(int.MaxValue), ColumnProperty.NotNull)
            );
            Database.AddUniqueConstraint("UK_Controllers_Controllers_Name", "Controllers_Controllers", "Name");
        }

        public override void Revert()
        {
            Database.RemoveConstraint("Controllers_Controllers", "UK_Controllers_Controllers_Name");
            Database.RemoveTable("Controllers_Controllers");
        }
    }
}
