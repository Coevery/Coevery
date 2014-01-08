using Coevery.Data.Migration;

namespace Coevery.Query {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            SchemaBuilder.CreateTable("UserProjectionRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("UserId")
                    .Column<int>("ProjectionId")
                );

            return 1;
        }
    }
}