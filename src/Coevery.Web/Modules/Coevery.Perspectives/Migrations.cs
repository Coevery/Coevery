using System;
using Coevery.ContentManagement.MetaData;
using Coevery.Core.Contents.Extensions;
using Coevery.Data.Migration;
using Coevery.Environment.Configuration;

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

        public int Create()
        {
            SchemaBuilder.CreateTable("PerspectivePartRecord",
                table => table
                    .ContentPartRecord()
                    .Column<string>("Title", column => column.WithLength(255))
                    .Column<string>("Description",column=>column.Unlimited())
                    .Column<int>("Position")
                );

            ContentDefinitionManager.AlterPartDefinition("PerspectivePart", builder => builder
                .Attachable()
                .WithDescription("Provides a perspective for your content item."));

            return 1;
        }
    }
}