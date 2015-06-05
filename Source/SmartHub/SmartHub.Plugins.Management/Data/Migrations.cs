using ECM7.Migrator.Framework;
using System.Data;

[assembly: MigrationAssembly("SmartHub.Plugins.Management")]

namespace SmartHub.Plugins.Management.Data
{
    [Migration(1)]
    public class Migration01 : Migration
    {
        public override void Apply()
        {
            Database.AddTable("Management_Zones",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
                new Column("Name", DbType.String, ColumnProperty.NotNull),
                new Column("MonitorsList", DbType.String.WithSize(int.MaxValue), ColumnProperty.Null),
                new Column("ControllersList", DbType.String.WithSize(int.MaxValue), ColumnProperty.Null)
            );
            Database.AddUniqueConstraint("UK_Management_Zones_Name", "Management_Zones", "Name");
        }

        public override void Revert()
        {
            Database.RemoveConstraint("Management_Zones", "UK_Management_Zones_Name");
            Database.RemoveTable("Management_Zones");
        }
    }

    [Migration(2)]
    public class Migration02 : Migration
    {
        public override void Apply()
        {
            Database.AddTable("Management_Controllers",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
                new Column("Name", DbType.String, ColumnProperty.NotNull),
                new Column("Type", DbType.Byte, ColumnProperty.NotNull),
                new Column("Configuration", DbType.String.WithSize(int.MaxValue), ColumnProperty.NotNull)
            );
            Database.AddUniqueConstraint("UK_Management_Controllers_Name", "Management_Controllers", "Name");
        }

        public override void Revert()
        {
            Database.RemoveConstraint("Management_Controllers", "UK_Management_Controllers_Name");
            Database.RemoveTable("Management_Controllers");
        }
    }

    //[Migration(3)]
    //public class Migration03 : Migration
    //{
    //    public override void Apply()
    //    {
    //        Database.AddTable("Management_Monitors",
    //            new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
    //            new Column("Name", DbType.String, ColumnProperty.NotNull),
    //            new Column("SensorId", DbType.Guid, ColumnProperty.NotNull),
    //            new Column("IsVisible", DbType.Boolean, ColumnProperty.NotNull, true)
    //        );
    //        Database.AddUniqueConstraint("UK_Management_Monitors_Name", "Management_Monitors", "Name");
    //        Database.AddUniqueConstraint("UK_Management_Monitors_Sensor", "Management_Monitors", "SensorId");
    //    }

    //    public override void Revert()
    //    {
    //        Database.RemoveConstraint("Management_Monitors", "UK_Management_Monitors_Sensor");
    //        Database.RemoveConstraint("Management_Monitors", "UK_Management_Monitors_Name");
    //        Database.RemoveTable("Management_Monitors");
    //    }
    //}
}
