using System;
using System.Collections.Generic;
using System.Linq;
using Coevery.Commands;
using Coevery.Setup.Services;

namespace Coevery.Setup.Commands {
    public class SetupCommand : DefaultCoeveryCommandHandler {
        private readonly ISetupService _setupService;

        public SetupCommand(ISetupService setupService) {
            _setupService = setupService;
        }

        [CoeverySwitch]
        public string SiteName { get; set; }

        [CoeverySwitch]
        public string AdminUsername { get; set; }

        [CoeverySwitch]
        public string AdminPassword { get; set; }

        [CoeverySwitch]
        public string DatabaseProvider { get; set; }

        [CoeverySwitch]
        public string DatabaseConnectionString { get; set; }

        [CoeverySwitch]
        public string DatabaseTablePrefix { get; set; }

        [CoeverySwitch]
        public string EnabledFeatures { get; set; }

        [CoeverySwitch]
        public string Recipe { get; set; }

        [CommandHelp("setup /SiteName:<siteName> /AdminUsername:<username> /AdminPassword:<password> /DatabaseProvider:<SqlCe|SQLServer|MySql> " + 
            "/DatabaseConnectionString:<connection_string> /DatabaseTablePrefix:<table_prefix> /EnabledFeatures:<feature1,feature2,...> " +
            "/Recipe:<recipe>" + 
            "\r\n\tRun first time setup for the site or for a given tenant")]
        [CommandName("setup")]
        [CoeverySwitches("SiteName,AdminUsername,AdminPassword,DatabaseProvider,DatabaseConnectionString,DatabaseTablePrefix,EnabledFeatures,Recipe")]
        public void Setup() {
            IEnumerable<string> enabledFeatures = null;
            if (!string.IsNullOrEmpty(EnabledFeatures)) {
                enabledFeatures = EnabledFeatures
                    .Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s));
            }
            Recipe = String.IsNullOrEmpty(Recipe) ? "Default" : Recipe;

            var setupContext = new SetupContext {
                SiteName = SiteName,
                AdminUsername = AdminUsername,
                AdminPassword = AdminPassword,
                DatabaseProvider = DatabaseProvider,
                DatabaseConnectionString = DatabaseConnectionString,
                DatabaseTablePrefix = DatabaseTablePrefix,
                EnabledFeatures = enabledFeatures,
                Recipe = Recipe,
            };

            _setupService.Setup(setupContext);

            Context.Output.WriteLine(T("Site \"{0}\" successfully setup to run data provider \"{1}\" (with table prefix \"{2}\") and configured by recipe \"{3}\"",
                setupContext.SiteName,
                setupContext.DatabaseProvider,
                setupContext.DatabaseTablePrefix,
                setupContext.Recipe));
        }
    }
}
