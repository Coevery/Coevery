using Coevery.ContentManagement.MetaData;
using Coevery.Data.Migration;

namespace Coevery.Themes {
    public class ThemesDataMigration : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable("ThemeSiteSettingsPartRecord", 
                table => table
                    .ContentPartRecord()
                    .Column<string>("CurrentThemeName")
                );

            return 1;
        }
    }
}