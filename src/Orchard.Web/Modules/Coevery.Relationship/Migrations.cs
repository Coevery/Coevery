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
                    .Column<string>("Name")
                    .Column<int>("Type")
                    .Column<int>("PrimaryEntity_Id")
                    .Column<int>("RelatedEntity_Id")
                );
            ContentDefinitionManager.AlterTypeDefinition("Relationship", cfg => cfg
                .WithPart("RelationshipPart")
                .DisplayedAs("Relationship")
                );

            SchemaBuilder.CreateTable("RelationshipColumnRecord",
                table => table
                    .ContentPartRecord()
                    .Column<int>("Relationship_Id")
                    .Column<int>("Column_Id")
                    .Column<int>("IsRelatedList")
                );
            ContentDefinitionManager.AlterTypeDefinition("RelationshipColumn", cfg => cfg
                .WithPart("RelationshipColumnPart")
                .DisplayedAs("RelationshipColumn")
                );

            SchemaBuilder.CreateTable("OneToManyRelationshipRecord",
                table => table
                    .ContentPartRecord()
                    .Column<int>("Relationship_Id")
                    .Column<int>("LookupField_Id")
                    .Column<string>("RelatedListLabel")
                    .Column<int>("ShowRelatedList")
                    .Column<int>("DeleteOption")
                );
            ContentDefinitionManager.AlterTypeDefinition("OneToManyRelationship", cfg => cfg
                .WithPart("OneToManyRelationshipPart")
                .DisplayedAs("OneToManyRelationship")
                );

            SchemaBuilder.CreateTable("ManyToManyRelationshipRecord",
                table => table
                    .ContentPartRecord()
                    .Column<int>("Relationship_Id")
                    .Column<string>("RelatedListLabel")
                    .Column<int>("ShowRelatedList")
                    .Column<string>("PrimaryListLabel")
                    .Column<int>("ShowPrimaryList")
                );
            ContentDefinitionManager.AlterTypeDefinition("ManyToManyRelationship", cfg => cfg
                .WithPart("ManyToManyRelationshipPart")
                .DisplayedAs("ManyToManyRelationship")
                );

            return 1;
        }
    }
}