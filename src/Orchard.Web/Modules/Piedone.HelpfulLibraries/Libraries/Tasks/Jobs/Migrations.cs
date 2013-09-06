using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using Piedone.HelpfulLibraries.Models;

namespace Piedone.HelpfulLibraries.Tasks.Jobs
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.Jobs")]
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(JobRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Industry", column => column.NotNull())
                    .Column<string>("ContextDefinion", column => column.Unlimited())
                    .Column<int>("Priority")
                )
                .AlterTable(typeof(JobRecord).Name,
                table => table
                    .CreateIndex("Industry", new string[] { "Industry" })
                );


            return 1;
        }
    }
}
