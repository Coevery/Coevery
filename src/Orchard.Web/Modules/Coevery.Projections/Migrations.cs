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

        public int UpdateFrom1() {
            SchemaBuilder.CreateTable("EntityFilterRecord",
                table => table
                    .Column<int>("Id", c => c.PrimaryKey().Identity())
                    .Column<string>("EntityName")
                    .Column<string>("Title")
                    .Column<int>("FilterGroupRecord_id")
                );
            return 2;
        }
    }
}