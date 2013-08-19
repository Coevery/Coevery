using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;

namespace Coevery.Taxonomies {
    public class Migrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable("TaxonomyPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("TermTypeName", column => column.WithLength(255))
                .Column<bool>("IsInternal")
            );

            SchemaBuilder.CreateTable("TermPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("Path", column => column.WithLength(255))
                .Column<int>("TaxonomyId")
                .Column<int>("Count")
                .Column<int>("Weight")
                .Column<bool>("Selectable")
            );

            SchemaBuilder.CreateTable("TermContentItem", table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<string>("Field", column => column.WithLength(50))
                .Column<int>("TermRecord_id")
                .Column<int>("TermsPartRecord_id")
            );

            ContentDefinitionManager.AlterTypeDefinition("Taxonomy", cfg => cfg
                .WithPart("TaxonomyPart")
                .WithPart("TitlePart")
                .WithPart("CommonPart")
                .WithPart("AutoroutePart", builder => builder
                .WithSetting("AutorouteSettings.AllowCustomPattern", "true")
                .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Title', Pattern: '{Content.Slug}', Description: 'my-taxonomy'}]")
                .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
            );

            SchemaBuilder.CreateTable("TermsPartRecord", table => table
                .ContentPartRecord()
            );

            return 3;
        }
    }
}