using Orchard.Data.Migration;

namespace Coevery.Fields {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            //ContentPartFieldDefinitionRecord_Id stands for the primary key of the field
            //OptionItemRecord is used for the multi-item selection table
            SchemaBuilder.CreateTable("OptionItemRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("ContentPartFieldDefinitionRecord_Id")
                    .Column<string>("Value")
                    .Column<bool>("IsDefault")
                );

            SchemaBuilder.CreateTable("FieldDependencyRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("Entity_Id")
                    .Column<int>("ControlField_Id")
                    .Column<int>("DependentField_Id")
                    .Column<string>("Value")
                );

            return 1;
        }
    }
}