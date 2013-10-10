using Orchard.Data.Migration;

namespace Coevery.Relationship {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            SchemaBuilder.CreateTable("RelationshipRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name")
                    .Column<byte>("Type", column => column.NotNull())
                    .Column<int>("PrimaryEntity_Id", column => column.NotNull())
                    .Column<int>("RelatedEntity_Id", column => column.NotNull())
                );

            SchemaBuilder.CreateTable("OneToManyRelationshipRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("Relationship_Id", column => column.Unique())
                    .Column<int>("LookupField_Id", column => column.Nullable())
                    .Column<int>("RelatedListProjection_Id", column => column.Nullable())
                    .Column<string>("RelatedListLabel", column => column.Nullable())
                    .Column<bool>("ShowRelatedList", column => column.NotNull())
                    .Column<byte>("DeleteOption", column => column.Nullable())
                );

            SchemaBuilder.CreateTable("ManyToManyRelationshipRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("Relationship_Id", column => column.Unique())
                    .Column<int>("RelatedListProjection_Id", column => column.Nullable())
                    .Column<string>("RelatedListLabel", column => column.Nullable())
                    .Column<bool>("ShowRelatedList", column => column.NotNull())
                    .Column<int>("PrimaryListProjection_Id", column => column.Nullable())
                    .Column<string>("PrimaryListLabel", column => column.Nullable())
                    .Column<bool>("ShowPrimaryList", column => column.NotNull())
                );

            return 1;
        }
    }
}