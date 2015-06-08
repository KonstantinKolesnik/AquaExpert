using ECM7.Migrator.Framework;
using System.Data;
using ForeignKeyConstraint = ECM7.Migrator.Framework.ForeignKeyConstraint;

[assembly: MigrationAssembly("SmartHub.Plugins.Monitors")]

namespace SmartHub.Plugins.Monitors.Data
{
    //[Migration(1)]
    //public class Migration01 : Migration
    //{
    //    public override void Apply()
    //    {
    //        Database.AddTable("Monitors_Monitors",
    //            new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
    //            new Column("Name", DbType.String, ColumnProperty.NotNull),
    //            new Column("SensorId", DbType.Guid, ColumnProperty.NotNull),
    //            new Column("Type", DbType.Byte, ColumnProperty.NotNull)
    //        );
    //        Database.AddUniqueConstraint("UK_Monitors_Monitors_Name", "Monitors_Monitors", "Name");
    //        Database.AddUniqueConstraint("UK_Monitors_Monitors_Sensor", "Monitors_Monitors", "SensorId");
    //        Database.AddForeignKey("FK_Monitors_Monitors_SensorId", "Monitors_Monitors", "SensorId", "MySensors_Sensors", "Id", ForeignKeyConstraint.Cascade);
    //    }

    //    public override void Revert()
    //    {
    //        Database.RemoveConstraint("Monitors_Monitors", "UK_Monitors_Monitors_Sensor");
    //        Database.RemoveConstraint("Monitors_Monitors", "UK_Monitors_Monitors_Name");
    //        Database.RemoveTable("Monitors_Monitors");
    //    }
    //}
}
