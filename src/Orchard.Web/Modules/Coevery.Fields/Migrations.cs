using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Coevery.Fields {
    public class Migrations : DataMigrationImpl {

        public int Create() {

            SchemaBuilder.CreateTable("OptionItemRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("ContentPartFieldDefinitionRecord_Id")
                    .Column<string>("Value")
                    .Column<bool>("IsDefault")
                );

            return 1;
        }
    }
}