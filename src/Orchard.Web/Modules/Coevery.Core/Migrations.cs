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

            SchemaBuilder.CreateTable("OpportunityRecord",
              table => table
                  .ContentPartRecord()
                  .Column<string>("Name")
                  .Column<string>("Description")
                  .Column<int>("SourceLeadId")
              );

            ContentDefinitionManager.AlterPartDefinition("Opportunity",
                cfg => cfg
                    .WithField("Name", builder => builder
                        .WithDisplayName("Name")
                        .OfType("TextField"))
                    .WithField("Description", builder => builder
                        .WithDisplayName("Description")
                        .OfType("TextField"))
                    .WithField("SourceLeadId", builder => builder
                        .WithDisplayName("SourceLeadId")
                        .OfType("TextField"))
                    .Attachable()
                );

            ContentDefinitionManager.AlterTypeDefinition("Opportunity",
               cfg => cfg
                   .WithPart("Opportunity")
                   .WithPart("LocalizationPart")
                   .Creatable()
               );

            return 1;
            return 1;
        }

    
    }
}