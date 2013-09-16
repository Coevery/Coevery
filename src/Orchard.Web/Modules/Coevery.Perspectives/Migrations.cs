using System;
using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;

namespace Coevery.Perspectives {
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

        public int UpdateFrom1() {
            SchemaBuilder.AlterTable("ModuleMenuItemPartRecord", table => table.AddColumn<string>("IconClass"));
            return 2;
        }
    }
}