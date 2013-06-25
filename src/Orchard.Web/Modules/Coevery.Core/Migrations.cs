using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Coevery.Core {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            SchemaBuilder.CreateTable("ModuleMenuItemPartRecord",
                table => table
                    .ContentPartRecord()
                    .Column<int>("ContentTypeDefinitionRecord_id")
                );

            ContentDefinitionManager.AlterTypeDefinition("ModuleMenuItem", cfg => cfg
                .WithPart("MenuPart")
                .WithPart("CommonPart")
                .WithPart("IdentityPart")
                .WithPart("ModuleMenuItemPart")
                .DisplayedAs("Module Menu Item")
                .WithSetting("Description", "Adds a Module Menu Item to navigation")
                .WithSetting("Stereotype", "MenuItem")
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.CreateTable("ViewPartRecord",
               table => table
                   .Column<int>("Id",column=>column.PrimaryKey().Identity())
                   .Column<int>("ContentTypeDefinitionRecord_id")
                   .Column<int>("ProjectionPartRecord_id")
               );
            return 2;
        }

        public int UpdateFrom2() {
            SchemaBuilder.DropTable("ViewPartRecord");
            SchemaBuilder.CreateTable("ViewPartRecord",
               table => table
                   .Column<int>("Id",column=>column.PrimaryKey().Identity())
                   .Column<int>("ContentTypeDefinitionRecord_id")
                   .Column<int>("ProjectionPartRecord_id")
               );
            return 3;
        }
    
    }
}