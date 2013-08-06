using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;

namespace Coevery.Relationship
{
    public class Migrations : DataMigrationImpl
    {
        public int Create() {
            SchemaBuilder.CreateTable("RelationshipRecord", 
                table=>table
                    .ContentPartRecord()
                    .Column<string>("Name",column=>column.NotNull())
                    .Column<bool>("Type", column => column.NotNull())
                    .Column<int>("PrimaryEntity_Id", column => column.NotNull())
                    .Column<int>("RelatedEntity_Id", column => column.NotNull())
                );
            ContentDefinitionManager.AlterTypeDefinition("Relationship", cfg => cfg
                .WithPart("RelationshipPart")
                .DisplayedAs("Relationship")
                );

            SchemaBuilder.CreateTable("RelationshipColumnRecord",
                table => table
                    .ContentPartRecord()
                    .Column<int>("Relationship_Id", column => column.NotNull())
                    .Column<int>("Column_Id", column => column.NotNull())
                    .Column<bool>("IsRelatedList", column => column.NotNull())
                );
            ContentDefinitionManager.AlterTypeDefinition("RelationshipColumn", cfg => cfg
                .WithPart("RelationshipColumnPart")
                .DisplayedAs("RelationshipColumn")
                );

            SchemaBuilder.CreateTable("OneToManyRelationshipRecord",
                table => table
                    .ContentPartRecord()
                    .Column<int>("Relationship_Id", column => column.NotNull())
                    .Column<int>("LookupField_Id", column => column.NotNull())
                    .Column<string>("RelatedListLabel", column => column.NotNull())
                    .Column<bool>("ShowRelatedList", column => column.NotNull())
                    .Column<int>("DeleteOption", column => column.NotNull())
                );
            ContentDefinitionManager.AlterTypeDefinition("OneToManyRelationship", cfg => cfg
                .WithPart("OneToManyRelationshipPart")
                .DisplayedAs("OneToManyRelationship")
                );

            SchemaBuilder.CreateTable("ManyToManyRelationshipRecord",
                table => table
                    .ContentPartRecord()
                    .Column<int>("Relationship_Id", column => column.NotNull())
                    .Column<string>("RelatedListLabel", column => column.NotNull())
                    .Column<bool>("ShowRelatedList", column => column.NotNull())
                    .Column<string>("PrimaryListLabel", column => column.NotNull())
                    .Column<bool>("ShowPrimaryList", column => column.NotNull())
                );
            ContentDefinitionManager.AlterTypeDefinition("ManyToManyRelationship", cfg => cfg
                .WithPart("ManyToManyRelationshipPart")
                .DisplayedAs("ManyToManyRelationship")
                );

            return 1;
        }
    }
}