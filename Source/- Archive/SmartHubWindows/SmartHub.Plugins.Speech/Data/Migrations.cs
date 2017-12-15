using ECM7.Migrator.Framework;
using System.Data;
using ForeignKeyConstraint = ECM7.Migrator.Framework.ForeignKeyConstraint;

[assembly: MigrationAssembly("SmartHub.Plugins.Speech")]

namespace SmartHub.Plugins.Speech.Data
{
    [Migration(1)]
    public class Migration01 : Migration
    {
        public override void Apply()
        {
            Database.AddTable("Speech_VoiceCommand",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "newid()"),
                new Column("CommandText", DbType.String.WithSize(200), ColumnProperty.NotNull),
                new Column("UserScriptId", DbType.Guid, ColumnProperty.NotNull)
            );

            Database.AddForeignKey("FK_Speech_VoiceCommand_UserScriptId", "Speech_VoiceCommand", "UserScriptId", "Scripts_UserScript", "Id", ForeignKeyConstraint.Cascade);
        }

        public override void Revert()
        {
            Database.RemoveTable("Speech_VoiceCommand");
        }
    }
}
