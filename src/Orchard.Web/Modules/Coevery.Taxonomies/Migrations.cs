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
                .Column<int>("TaxonomyId")
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
                );

            SchemaBuilder.CreateTable("TermsPartRecord", table => table
                .ContentPartRecord()
                );

            return 3;
        }

        public int UpdateFrom3() {
            ContentDefinitionManager.AlterTypeDefinition("Term", cfg => cfg
                .WithPart("TermPart")
                .WithPart("TitlePart")
                .WithPart("CommonPart")
                );
            return 4;
        }
    }
}