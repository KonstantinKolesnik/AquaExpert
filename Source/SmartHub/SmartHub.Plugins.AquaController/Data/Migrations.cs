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

    [Migration(2)]
    public class Migration02 : Migration
    {
        public override void Apply()
        {
            Database.AddTable("AquaController_Monitors",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
                new Column("Name", DbType.String, ColumnProperty.NotNull),
                new Column("SensorId", DbType.Guid, ColumnProperty.NotNull)
            );
            Database.AddUniqueConstraint("UK_AquaController_Monitors_Name", "AquaController_Monitors", "Name");
            Database.AddUniqueConstraint("UK_AquaController_Monitors_Sensor", "AquaController_Monitors", "SensorId");
        }

        public override void Revert()
        {
            Database.RemoveConstraint("AquaController_Monitors", "UK_AquaController_Monitors_Sensor");
            Database.RemoveConstraint("AquaController_Monitors", "UK_AquaController_Monitors_Name");
            Database.RemoveTable("AquaController_Monitors");
        }
    }

    [Migration(3)]
    public class Migration03 : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("AquaController_Monitors", new Column("IsVisible", DbType.Boolean, ColumnProperty.NotNull, true));
        }

        public override void Revert()
        {
            Database.RemoveColumn("AquaController_Monitors", "IsVisible");
        }
    }

    [Migration(4)]
    public class Migration04 : Migration
    {
        public override void Apply()
        {
            Database.AddTable("AquaController_Controllers",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
                new Column("Name", DbType.String, ColumnProperty.NotNull),
                new Column("Type", DbType.Byte, ColumnProperty.NotNull),
                new Column("IsVisible", DbType.Boolean, ColumnProperty.NotNull, true),
                new Column("Configuration", DbType.String.WithSize(int.MaxValue), ColumnProperty.NotNull)
            );
            Database.AddUniqueConstraint("UK_AquaController_Controllers_Name", "AquaController_Controllers", "Name");
        }

        public override void Revert()
        {
            Database.RemoveConstraint("AquaController_Controllers", "UK_AquaController_Controllers_Name");
            Database.RemoveTable("AquaController_Controllers");
        }
    }
}
