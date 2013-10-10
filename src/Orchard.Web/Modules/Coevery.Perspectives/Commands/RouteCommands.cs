using Orchard.Commands;
using Orchard.Data.Migration.Interpreters;
using Orchard.Data.Migration.Schema;
using Orchard.Environment.Configuration;

namespace Coevery.Perspectives.Commands {
    public class RouteCommands : DefaultOrchardCommandHandler {
        private readonly SchemaBuilder _schemaBuilder;
        private readonly ShellSettings _shellSettings;

        public RouteCommands(IDataMigrationInterpreter interpreter,
            ShellSettings shellSettings) {
            _shellSettings = shellSettings;
            _schemaBuilder = new SchemaBuilder(interpreter);
        }

        private string DataTablePrefix() {
            if (string.IsNullOrEmpty(_shellSettings.DataTablePrefix)) return string.Empty;
            return _shellSettings.DataTablePrefix + "_";
        }

        [CommandName("set route")]
        [CommandHelp("set route")]
        [OrchardSwitches("Slug,Title,Path,Text,Owner,MenuText,Homepage,MenuName,Publish,UseWelcomeText")]
        public void Create() {
            _schemaBuilder.ExecuteSql(string.Format(@"	UPDATE {0}Orchard_Alias_ActionRecord
	                                                    SET	    Area = 'Coevery.Core',
			                                                    Controller = 'Home',
			                                                    [Action] = 'Index'
	                                                    FROM	{0}Orchard_Autoroute_AutoroutePartRecord ar
			                                                    INNER JOIN {0}Orchard_Alias_AliasRecord a
			                                                    ON	a.Id = ar.UseCustomPattern
			                                                    AND	ar.CustomPattern = '/'
			                                                    INNER JOIN {0}Orchard_Alias_ActionRecord ac
			                                                    ON	ac.Id	= a.Action_id", DataTablePrefix()));
        }
    }
}