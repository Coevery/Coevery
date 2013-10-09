using System;
using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Environment.Configuration;

namespace Coevery.Perspectives {
    public class Migrations : DataMigrationImpl {
        private readonly ShellSettings _shellSettings;

        public Migrations(ShellSettings shellSettings) {
            _shellSettings = shellSettings;
        }

        private string DataTablePrefix() {
            if (string.IsNullOrEmpty(_shellSettings.DataTablePrefix)) return string.Empty;
            return _shellSettings.DataTablePrefix + "_";
        }

        public int Create() {
            SchemaBuilder.CreateTable("ModuleMenuItemPartRecord",
                table => table
                    .ContentPartRecord()
                    .Column<int>("ContentTypeDefinitionRecord_id")
                );
            
            ContentDefinitionManager.AlterTypeDefinition("ModuleMenuItem", cfg => cfg
                .WithPart("MenuPart")
                .WithPart("CommonPart")
                .WithPart("IdentityPart")
                .WithPart("ModuleMenuItemPart")
                .DisplayedAs("Module Menu Item")
                .WithSetting("Description", "Adds a Module Menu Item to navigation")
                .WithSetting("Stereotype", "MenuItem")
                );

            return 1;
        }

        public int UpdateFrom1() {
            SchemaBuilder.AlterTable("ModuleMenuItemPartRecord", table => table.AddColumn<string>("IconClass"));
            return 2;
        }

        public int UpdateFrom2() {
            SchemaBuilder.ExecuteSql(string.Format(@"	UPDATE {0}Orchard_Alias_ActionRecord
	                                                    SET	    Area = 'Coevery.Core',
			                                                    Controller = 'Home',
			                                                    [Action] = 'Index'
	                                                    FROM	{0}Orchard_Autoroute_AutoroutePartRecord ar
			                                                    INNER JOIN {0}Orchard_Alias_AliasRecord a
			                                                    ON	a.Id = ar.UseCustomPattern
			                                                    AND	ar.CustomPattern = '/'
			                                                    INNER JOIN {0}Orchard_Alias_ActionRecord ac
			                                                    ON	ac.Id	= a.Action_id", DataTablePrefix()));
            return 3;
        }
    }
}