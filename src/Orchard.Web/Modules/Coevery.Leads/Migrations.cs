

using Orchard.Data.Migration;

namespace Coevery.Leads
{
    public class Migrations : DataMigrationImpl
    {

        public int Create()
        {
            SchemaBuilder.CreateTable("TodoListRecord",
                table => table
                    .ContentPartRecord()
                    .Column<string>("Title")
                    .Column<string>("UserId")
                );

            SchemaBuilder.CreateTable("TodoItemRecord",
                table => table
                    .ContentPartRecord()
                    .Column<string>("Title")
                    .Column<bool>("IsDone")
                    .Column<int>("ToDoList_id")
                );

            SchemaBuilder.CreateTable("LeadRecord",
               table => table
                   .Column<int>("Id", column => column.PrimaryKey().Identity())
                   .Column<string>("Topic")
                   .Column<string>("FirstName")
                   .Column<string>("LastName")
                   .Column<int>("StatusCode")
               );

            return 5;
        }


    }
}