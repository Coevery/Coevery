using System;
using System.Linq;
using Coevery.Dynamic.Services;
using Orchard.ContentManagement.MetaData;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Dynamic
{
    public class ImplicitContentTypeScanner : IOrchardShellEvents
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly ShellSettings _shellSettings;
        private readonly ITableSchemaManager _tableSchemaManager;
        private readonly IDynamicAssemblyBuilder _assemblyBuilder;

        public ImplicitContentTypeScanner(IContentDefinitionManager contentDefinitionManager,
                                          ShellSettings shellSettings,
                                          ITableSchemaManager tableSchemaManager,
                                          IDynamicAssemblyBuilder assemblyBuilder) {
            _contentDefinitionManager = contentDefinitionManager;
            _shellSettings = shellSettings;
            _tableSchemaManager = tableSchemaManager;
            _assemblyBuilder = assemblyBuilder;
        }

        public virtual Feature Feature { get; set; }

        void IOrchardShellEvents.Activated() {

            var typeDefinitions = _contentDefinitionManager.ListTypeDefinitions();

            var typeNames = typeDefinitions.Select(ctd => ctd.Name);

            // user-defined parts
            // except for those parts with the same name as a type (implicit type's part or a mistake)
            var userContentParts = _contentDefinitionManager
                .ListPartDefinitions()
                .Where(cpd => typeNames.Contains(cpd.Name))
                .Select(cpd => new DynamicTypeDefinition {
                    Name = cpd.Name,
                    Fields = cpd.Fields.Select(f => new DynamicFieldDefinition {
                        Name = f.Name,
                        Type = typeof (string)
                    }).Union(new[] {new DynamicFieldDefinition {Name = "Id", Type = typeof (int)}})
                }).ToList();

            if (userContentParts.Any()) {
                try
                {
                    _tableSchemaManager.UpdateSchema(userContentParts, FormatTableName);

                    _assemblyBuilder.Build(userContentParts);
                }
                catch (Exception)
                {
                    throw;
                }
              
            }
        }

        private string FormatTableName(string name) {
            var extensionId = "Coevery_DynamicTypes";
            var extensionName = extensionId.Replace('.', '_');

            var dataTablePrefix = "";
            if (!string.IsNullOrEmpty(_shellSettings.DataTablePrefix))
                dataTablePrefix = _shellSettings.DataTablePrefix + "_";
            return dataTablePrefix + extensionName + '_' + name + "PartRecord";
        }

        void IOrchardShellEvents.Terminating() {}
    }
}
