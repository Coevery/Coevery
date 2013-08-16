using Orchard.Data.Migration;

namespace Coevery.Fields {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            //ContentPartFieldDefinitionRecord_Id stands for the primary key of the field
            //OptionItemRecord is used for the multi-item selection table
            SchemaBuilder.CreateTable("OptionItemRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey())
                    .Column<string>("Value")
                    .Column<bool>("IsDefault")
                    .Column<int>("OptionSetRecord_Id")
                );

            SchemaBuilder.CreateTable("OptionSetRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey())
                    .Column<string>("FieldName")
                );

            SchemaBuilder.CreateTable("FieldDependencyRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey())
                    .Column<int>("Entity_Id")
                    .Column<int>("ControlField_Id")
                    .Column<int>("DependentField_Id")
                    .Column<string>("Value")
                );

            SchemaBuilder.CreateTable("SelectedOptionRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey())
                    .Column<int>("OptionItem_Id",column=>column.NotNull())
                    .Column<int>("SelectedOptionSetRecord_Id")
                );
            return 1;
        }
    }
}