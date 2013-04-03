using System;
using System.Linq;
using Coevery.Metadata.DynamicTypeGeneration;
using Orchard.ContentManagement.MetaData;
using Orchard.Environment.Configuration;

namespace Coevery.Metadata.Services {
    public class DefaultTypeDeployService : ITypeDeployService {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly ITableSchemaManager _tableSchemaManager;
        private readonly IDynamicAssemblyBuilder _dynamicAssemblyBuilder;
        private readonly ShellSettings _shellSettings;


        public DefaultTypeDeployService(
            IContentDefinitionManager contentDefinitionManager,
            ITableSchemaManager tableSchemaManager,
            IDynamicAssemblyBuilder dynamicAssemblyBuilder,
            ShellSettings shellSettings) {
            _contentDefinitionManager = contentDefinitionManager;
            _tableSchemaManager = tableSchemaManager;
            _dynamicAssemblyBuilder = dynamicAssemblyBuilder;
            _shellSettings = shellSettings;
        }

        public bool DeployType(string name)
        {
            var typeDefinitions = _contentDefinitionManager.ListTypeDefinitions();

            var typeNames = typeDefinitions.Select(ctd => ctd.Name);

            // user-defined parts
            // except for those parts with the same name as a type (implicit type's part or a mistake)
            var userContentParts = _contentDefinitionManager
                .ListPartDefinitions()
                .Where(cpd => typeNames.Contains(cpd.Name))
                .Select(cpd => new DynamicTypeDefinition
                {
                    Name = cpd.Name,
                    Fields = cpd.Fields.Select(f => new DynamicFieldDefinition
                    {
                        Name = f.Name,
                        Type = typeof(string)
                    }).Union(new[] { new DynamicFieldDefinition { Name = "Id", Type = typeof(int) } })
                }).ToList();

            if (userContentParts.Any())
            {
                try
                {
                    var types = _dynamicAssemblyBuilder.Build(userContentParts);
                    _tableSchemaManager.UpdateSchema(FormatTableName, types);
                }
                catch (Exception)
                {
                    throw;
                }

            }
            return true;
        }

        private string FormatTableName(string name)
        {
            var extensionId = "Coevery_DynamicTypes";
            var extensionName = extensionId.Replace('.', '_');

            var dataTablePrefix = "";
            if (!string.IsNullOrEmpty(_shellSettings.DataTablePrefix))
                dataTablePrefix = _shellSettings.DataTablePrefix + "_";
            return dataTablePrefix + extensionName + '_' + name;
        }


    }
}