using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;

namespace Coevery.Relationship
{
    public class Migrations : DataMigrationImpl
    {
        public int Create() {
            SchemaBuilder.CreateTable("RelationshipRecord", 
                table=>table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name",column=>column.Unique())
                    .Column<byte>("Type", column => column.NotNull())
                    .Column<int>("PrimaryEntity_Id", column => column.NotNull())
                    .Column<int>("RelatedEntity_Id", column => column.NotNull())
                );
            ContentDefinitionManager.AlterTypeDefinition("Relationship", cfg => cfg
                .WithPart("RelationshipPart")
                .DisplayedAs("Relationship")
                );

            SchemaBuilder.CreateTable("RelationshipColumnRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
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
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("Relationship_Id", column => column.Unique())
                    .Column<int>("LookupField_Id", column => column.Nullable())
                    .Column<string>("RelatedListLabel", column => column.Nullable())
                    .Column<bool>("ShowRelatedList", column => column.NotNull())
                    .Column<byte>("DeleteOption", column => column.Nullable())
                );
            ContentDefinitionManager.AlterTypeDefinition("OneToManyRelationship", cfg => cfg
                .WithPart("OneToManyRelationshipPart")
                .DisplayedAs("OneToManyRelationship")
                );

            SchemaBuilder.CreateTable("ManyToManyRelationshipRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("Relationship_Id", column => column.Unique())
                    .Column<string>("RelatedListLabel", column => column.Nullable())
                    .Column<bool>("ShowRelatedList", column => column.NotNull())
                    .Column<string>("PrimaryListLabel", column => column.Nullable())
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