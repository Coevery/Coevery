using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Orchard.ContentManagement.Records;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Environment.ShellBuilders.Models;

namespace Coevery.Metadata.Services
{
    public class DynamicTypeAutomaticMigrations : IOrchardShellEvents {
        private readonly ITableSchemaManager _tableSchemaManager;
        private readonly ShellSettings _shellSettings;

        public DynamicTypeAutomaticMigrations(
            ITableSchemaManager tableSchemaManager,
            ShellSettings shellSettings) {
            _tableSchemaManager = tableSchemaManager;
            _shellSettings = shellSettings;
        }

        public void Activated() {
            const string featureName = "Coevery.DynamicTypes";
            var types = Assembly.Load(featureName).GetTypes();
            var recordTypes = types.Where(c => typeof (ContentPartRecord).IsAssignableFrom(c)).ToList();
            var recordBlueprints = recordTypes.Select(t => new RecordBlueprint {TableName = FormatTableName(featureName, t.Name), Type = t}).ToList();
            _tableSchemaManager.UpdateSchema(recordBlueprints);
        }

        public void Terminating() {}

        private string FormatTableName(string featureName, string name)
        {
            var extensionName = featureName.Replace('.', '_');

            var dataTablePrefix = "";
            if (!string.IsNullOrEmpty(_shellSettings.DataTablePrefix))
                dataTablePrefix = _shellSettings.DataTablePrefix + "_";
            return dataTablePrefix + extensionName + '_' + name;
        }
    }
}