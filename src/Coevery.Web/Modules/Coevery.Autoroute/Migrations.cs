using Coevery.ContentManagement.MetaData;
using Coevery.Core.Contents.Extensions;
using Coevery.Data.Migration;

namespace Coevery.Autoroute {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            SchemaBuilder.CreateTable("AutoroutePartRecord",
                table => table
                    .ContentPartVersionRecord()
                            .Column<string>("CustomPattern", c => c.WithLength(2048))
                            .Column<bool>("UseCustomPattern", c=> c.WithDefault(false))
                            .Column<string>("DisplayAlias", c => c.WithLength(2048)));

            ContentDefinitionManager.AlterPartDefinition("AutoroutePart", part => part
                .Attachable()
                .WithDescription("Adds advanced url configuration options to your content type to completely customize the url pattern for a content item."));

            return 2;
        }

        public int UpdateFrom1() {
            ContentDefinitionManager.AlterPartDefinition("AutoroutePart", part => part
                .WithDescription("Adds advanced url configuration options to your content type to completely customize the url pattern for a content item."));
            return 2;
        }
    }
}