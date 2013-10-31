using Coevery.ContentManagement.MetaData;
using Coevery.Data.Migration;

namespace Coevery.Entities {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            SchemaBuilder.CreateTable("EntityMetadataRecord",
                table => table
                    .ContentPartVersionRecord()
                    .Column<string>("Name")
                    .Column<string>("DisplayName")
                    .Column<string>("Settings", column => column.Unlimited())
                );

            SchemaBuilder.CreateTable("FieldMetadataRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("OriginalId")
                    .Column<string>("Name")
                    .Column<int>("ContentFieldDefinitionRecord_Id")
                    .Column<int>("EntityMetadataRecord_Id")
                    .Column<string>("Settings", column => column.Unlimited())
                );

            ContentDefinitionManager.AlterPartDefinition("EntityMetadataPart", cfg => { });
            ContentDefinitionManager.AlterTypeDefinition("EntityMetadata", cfg => cfg
                .WithPart("EntityMetadataPart")
                );
            return 1;
        }
    }
}