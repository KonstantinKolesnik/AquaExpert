using ECM7.Migrator.Framework;
using System.Data;

[assembly: MigrationAssembly("SmartNetwork.Plugins.WebUI")]

namespace SmartHub.Plugins.WebUI.Data
{
    [Migration(1)]
    public class Migration01 : Migration
    {
        public override void Apply()
        {
            Database.AddTable("WebUI_Tiles",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
                new Column("HandlerKey", DbType.String.WithSize(200), ColumnProperty.NotNull),
                new Column("SortOrder", DbType.Int32, ColumnProperty.NotNull, 0),
                new Column("SerializedParameters", DbType.String.WithSize(int.MaxValue))
            );
        }

        public override void Revert()
        {
            Database.RemoveTable("WebUI_Tiles");
        }
    }
}
