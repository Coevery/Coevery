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

            var dropViewPartRecordTable = _dialect.GetDropTableString("Coevery_Core_ViewPartRecord");
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

            SchemaBuilder.AlterTable("ListViewPartRecord",
                table => table
                    .AddColumn<bool>("IsDefault", column => column.WithDefault(false)));

            SchemaBuilder.ExecuteSql(@" UPDATE Coevery_Projections_ListViewPartRecord
                                        SET	IsDefault = 1
                                        FROM	Coevery_Projections_ListViewPartRecord l
                                        WHERE NOT EXISTS(   SELECT * 
                                                            FROM Coevery_Projections_ListViewPartRecord lvp 
                                                            WHERE lvp.ItemContentType = l.ItemContentType AND lvp.IsDefault = 1)");

            SchemaBuilder.ExecuteSql(@" UPDATE Orchard_Framework_ContentItemRecord
                                        SET	ContentType_id	= (SELECT Id FROM Orchard_Framework_ContentTypeRecord WHERE Name = 'LayoutProperty')
                                        FROM Coevery_Projections_ListViewPartRecord lvp INNER JOIN Orchard_Framework_ContentItemRecord i ON	i.Id = lvp.Id

                                        UPDATE	Orchard_Framework_ContentTypeRecord
                                        SET		Name = 'ListViewPage'
                                        WHERE	Name = 'LayoutPropert'");

            ContentDefinitionManager.AlterTypeDefinition("ListViewPage",
                cfg => cfg
                    .WithPart("TitlePart"));

            return 3;
        }
    }
}