using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;

namespace Coevery.OptionSet {
    public class Migrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable("OptionSetPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("TermTypeName", column => column.WithLength(255))
                .Column<bool>("IsInternal")
                );

            SchemaBuilder.CreateTable("OptionItemPartRecord", table => table
                .ContentPartRecord()
                .Column<int>("OptionSetId")
                .Column<int>("Weight")
                .Column<bool>("Selectable")
                );

            SchemaBuilder.CreateTable("OptionItemContentItem", table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<string>("Field", column => column.WithLength(250))
                .Column<int>("OptionItemRecord_id")
                .Column<int>("OptionItemContainerPartRecord_id")
                );

            SchemaBuilder.CreateTable("OptionItemContainerPartRecord", table => table
                .ContentPartRecord()
                );

            ContentDefinitionManager.AlterTypeDefinition("OptionSet", cfg => cfg
                .WithPart("OptionSetPart")
                .WithPart("TitlePart")
                .WithPart("CommonPart")
                );

            ContentDefinitionManager.AlterTypeDefinition("OptionItem", cfg => cfg
                .WithPart("OptionItemPart")
                .WithPart("TitlePart")
                .WithPart("CommonPart")
                );

            return 3;
        }
    }
}