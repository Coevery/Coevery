using NHibernate.Dialect;
using Orchard.ContentManagement.MetaData;
using Orchard.Data;
using Orchard.Data.Migration;

namespace Coevery.Projections {
    public class Migrations : DataMigrationImpl {

        private readonly Dialect _dialect;

        public Migrations(ISessionFactoryHolder sessionFactoryHolder) {
            var configuration = sessionFactoryHolder.GetConfiguration();
            _dialect = Dialect.GetDialect(configuration.Properties);
        }

        public int Create() {
            SchemaBuilder.CreateTable("LayoutPropertyRecord",
                table => table
                    .ContentPartRecord()
                    .Column<string>("VisableTo")
                    .Column<int>("PageRowCount")
                    .Column<int>("QueryPartRecord_id")
                );

            ContentDefinitionManager.AlterTypeDefinition("LayoutProperty", cfg => cfg
                .WithPart("LayoutPropertyPart")
                .DisplayedAs("LayoutProperty")
                );
            return 1;
        }

        public int UpdateFrom1() {

            SchemaBuilder.CreateTable("ListViewPartRecord",
                table => table
                    .ContentPartRecord()
                    .Column<string>("ItemContentType")
                    .Column<string>("VisableTo")
                );

            ContentDefinitionManager.DeleteTypeDefinition("LayoutProperty");
            ContentDefinitionManager.DeletePartDefinition("LayoutPropertyPart");

            SchemaBuilder.ExecuteSql(@"INSERT INTO Coevery_Projections_ListViewPartRecord(Id,ItemContentType,VisableTo)
                                       SELECT Id = v.ProjectionPartRecord_id, ItemContentType = t.Name,VisableTo = 'All' 
                                       FROM     Coevery_Core_ViewPartRecord v 
                                                INNER JOIN Settings_ContentTypeDefinitionRecord t 
                                                ON t.Id = v.ContentTypeDefinitionRecord_id");

            var dropViewPartRecordTable =_dialect.GetDropTableString("Coevery_Core_ViewPartRecord");
            SchemaBuilder.ExecuteSql(dropViewPartRecordTable);
            SchemaBuilder.DropTable("LayoutPropertyRecord");

            ContentDefinitionManager.AlterTypeDefinition("ListViewPage",
                cfg => cfg
                    .WithPart("ListViewPart")
                    .WithPart("ProjectionPart")
                    .DisplayedAs("List View"));

            return 2;
        }
        
         public int UpdateFrom2() {
            SchemaBuilder.CreateTable("EntityFilterRecord",
                table => table
                    .Column<int>("Id", c => c.PrimaryKey().Identity())
                    .Column<string>("EntityName")
                    .Column<string>("Title")
                    .Column<int>("FilterGroupRecord_id")
                );
            return 3;
        }

    }
}