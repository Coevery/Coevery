using NHibernate.Dialect;
using Orchard.ContentManagement.MetaData;
using Orchard.Data;
using Orchard.Data.Migration;
using Orchard.Environment.Configuration;

namespace Coevery.Projections {
    public class Migrations : DataMigrationImpl {
        private readonly ShellSettings _shellSettings;
        private readonly Dialect _dialect;

        public Migrations(ISessionFactoryHolder sessionFactoryHolder, 
            ShellSettings shellSettings) {
            _shellSettings = shellSettings;
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

            SchemaBuilder.ExecuteSql(string.Format(@"INSERT INTO {0}Coevery_Projections_ListViewPartRecord(Id,ItemContentType,VisableTo)
                                       SELECT Id = v.ProjectionPartRecord_id, ItemContentType = t.Name,VisableTo = 'All' 
                                       FROM     {0}Coevery_Core_ViewPartRecord v 
                                                INNER JOIN {0}Settings_ContentTypeDefinitionRecord t 
                                                ON t.Id = v.ContentTypeDefinitionRecord_id", DataTablePrefix()));

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

        private string DataTablePrefix() {
            if (string.IsNullOrEmpty(_shellSettings.DataTablePrefix)) return string.Empty;
            return _shellSettings.DataTablePrefix + "_";
        }

        public int UpdateFrom2() {

            SchemaBuilder.AlterTable("ListViewPartRecord",
                table => table
                    .AddColumn<bool>("IsDefault", column => column.WithDefault(false)));

            SchemaBuilder.ExecuteSql(string.Format(@" UPDATE {0}Coevery_Projections_ListViewPartRecord
                                        SET	IsDefault = 1
                                        FROM	{0}Coevery_Projections_ListViewPartRecord l
                                        WHERE NOT EXISTS(   SELECT * 
                                                            FROM {0}Coevery_Projections_ListViewPartRecord lvp 
                                                            WHERE lvp.ItemContentType = l.ItemContentType AND lvp.IsDefault = 1)", DataTablePrefix()));

            SchemaBuilder.ExecuteSql(string.Format(@" UPDATE {0}Orchard_Framework_ContentItemRecord
                                        SET	ContentType_id	= (SELECT Id FROM {0}Orchard_Framework_ContentTypeRecord WHERE Name = 'LayoutProperty')
                                        FROM {0}Coevery_Projections_ListViewPartRecord lvp INNER JOIN {0}Orchard_Framework_ContentItemRecord i ON	i.Id = lvp.Id

                                        UPDATE	{0}Orchard_Framework_ContentTypeRecord
                                        SET		Name = 'ListViewPage'
                                        WHERE	Name = 'LayoutProperty'", DataTablePrefix()));

            ContentDefinitionManager.AlterTypeDefinition("ListViewPage",
                cfg => cfg
                    .WithPart("TitlePart"));

            return 3;
        }
        
         public int UpdateFrom3() {
            SchemaBuilder.CreateTable("EntityFilterRecord",
                table => table
                    .Column<int>("Id", c => c.PrimaryKey().Identity())
                    .Column<string>("EntityName")
                    .Column<string>("Title")
                    .Column<int>("FilterGroupRecord_id")
                );
            return 4;
        }

    }
}