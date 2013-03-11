using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using Orchard.ContentManagement.Records;
using Orchard.Data.Migration.Interpreters;
using Orchard.Data.Migration.Schema;
using Orchard.Data;

namespace Coevery.Dynamic.Services
{
    public class DefaultTableSchemaManager : ITableSchemaManager
    {
        private readonly IDataMigrationInterpreter _interpreter;
        private readonly SchemaBuilder _schemaBuilder;
        private readonly ISessionLocator _sessionLocator;

        public DefaultTableSchemaManager(IDataMigrationInterpreter interpreter, ISessionLocator sessionLocator) {
            _interpreter = interpreter;
            _sessionLocator = sessionLocator;
            _schemaBuilder = new SchemaBuilder(_interpreter);
        }

        public void UpdateSchema(IEnumerable<DynamicTypeDefinition> typeDefinitions, Func<string, string> format)
        {
            format = format ?? (s => s ?? String.Empty);
            var session = _sessionLocator.For(typeof (ContentItemRecord));
            var connection = session.Connection;

            var tableNames = GetDatabaseTables(connection, new SqlPseudoProvider());
            foreach (var definition in typeDefinitions) {
                var tableName = String.Concat(format(definition.Name));
                var exists = tableNames.Any(t => t.Equals(tableName, StringComparison.OrdinalIgnoreCase));
                if (!exists) {
                    _schemaBuilder.CreateTable(tableName, table => {
                        foreach (var field in definition.Fields) {
                            var dbType = SchemaUtils.ToDbType(field.Type);
                            var isPrimaryKey = field.Name.Equals("Id", StringComparison.OrdinalIgnoreCase);
                            if (isPrimaryKey) {
                                table.Column(field.Name, dbType, column => column.PrimaryKey());
                            }
                            else {
                                table.Column(field.Name, dbType);
                            }
                        }
                    });
                }
            }
        }

        private static IEnumerable<string> GetDatabaseTables(IDbConnection connection, IPseudoProvider provider)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = provider.StoreSchemaTablesQuery;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read()) {
                        yield return reader["Name"] as string;
                    }
                }
            }
        }

        private interface IPseudoProvider
        {
            string StoreSchemaTablesQuery { get; }
            bool SupportsSchemas { get; }
        }

        private class SqlPseudoProvider : IPseudoProvider
        {
            public string StoreSchemaTablesQuery
            {
                get { return "SELECT TABLE_SCHEMA SchemaName, TABLE_NAME Name FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'"; }
            }

            public bool SupportsSchemas
            {
                get { return true; }
            }
        }

        private class SqlCePseudoProvider : IPseudoProvider
        {
            public string StoreSchemaTablesQuery
            {
                get { return "SELECT TABLE_SCHEMA SchemaName, TABLE_NAME Name FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'TABLE'"; }
            }

            public bool SupportsSchemas
            {
                get { return false; }
            }
        }
    }
}