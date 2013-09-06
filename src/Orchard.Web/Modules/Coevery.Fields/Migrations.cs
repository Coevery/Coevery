using Orchard.Data.Migration;

namespace Coevery.Fields {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            SchemaBuilder.CreateTable("FieldDependencyRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("Entity_Id")
                    .Column<int>("ControlField_Id")
                    .Column<int>("DependentField_Id")
                    .Column<string>("Value")
                );
            return 5;
        }
    }
}