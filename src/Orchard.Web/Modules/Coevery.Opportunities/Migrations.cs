

using Orchard.Data.Migration;

namespace Coevery.Opportunities
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable("OpportunityRecord",
               table => table
                   .Column<int>("Id", column => column.PrimaryKey().Identity())
                   .Column<string>("Name")
                   .Column<string>("Description")
                   .Column<int>("SourceLeadId")
               );

            return 1;
        }


    }
}