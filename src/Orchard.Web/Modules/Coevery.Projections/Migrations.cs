using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Coevery.Projections {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            SchemaBuilder.CreateTable("LayoutPropertyRecord",
                table => table
                    .ContentPartRecord()
                    .Column<string>("VisableTo")
                    .Column<int>("PageRowCount")
                    .Column<int>("QueryPartRecord_id")
                );

            ContentDefinitionManager.AlterTypeDefinition("LayoutProperty", cfg => cfg
                 .WithPart("LayoutPropertyPart")
                .DisplayedAs("LayoutProperty")
                );
            return 1;
        }
    }
}