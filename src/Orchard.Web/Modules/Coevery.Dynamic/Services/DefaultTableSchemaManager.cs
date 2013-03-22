using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping;
using NHibernate.Tool.hbm2ddl;
using Orchard.ContentManagement.Records;
using Orchard.Data.Migration.Interpreters;
using Orchard.Data.Migration.Schema;
using Orchard.Data;
using NHibernate.Dialect;
using Orchard.Data.Providers;
using Orchard.Environment.Configuration;
using Orchard.Environment.Descriptor.Models;
using Orchard.Environment.Extensions;
using Orchard.Environment.ShellBuilders;

namespace Coevery.Dynamic.Services
{
    public class DefaultTableSchemaManager : ITableSchemaManager
    {
        private readonly IDataMigrationInterpreter _interpreter;
        private readonly SchemaBuilder _schemaBuilder;
        private readonly ISessionLocator _sessionLocator;
        private readonly ISessionFactoryHolder _sessionFactoryHolder;

        public DefaultTableSchemaManager(IDataMigrationInterpreter interpreter, 
            ISessionLocator sessionLocator,
             ISessionFactoryHolder sessionFactoryHolder
            )
        {
            _interpreter = interpreter;
            _sessionLocator = sessionLocator;
            _schemaBuilder = new SchemaBuilder(_interpreter);
            _sessionFactoryHolder = sessionFactoryHolder;
        }

        public void UpdateSchema(IEnumerable<DynamicTypeDefinition> typeDefinitions, Func<string, string> format)
        {
            //format = format ?? (s => s ?? String.Empty);
            //var session = _sessionLocator.For(typeof (ContentItemRecord));
            //var connection = session.Connection;

            ////var tableNames = GetDatabaseTables(session, new SqlPseudoProvider());
            //var tableNames = GetDatabaseTables();
            //foreach (var definition in typeDefinitions) {
            //    var tableName = String.Concat(format(definition.Name));
            //    var exists = tableNames.Any(t => t.Equals(tableName, StringComparison.OrdinalIgnoreCase));
            //    if (!exists) {
            //        _schemaBuilder.CreateTable(tableName, table => {
            //            foreach (var field in definition.Fields) {
            //                var dbType = SchemaUtils.ToDbType(field.Type);
            //                var isPrimaryKey = field.Name.Equals("Id", StringComparison.OrdinalIgnoreCase);
            //                if (isPrimaryKey) {
            //                    table.Column(field.Name, dbType, column => column.PrimaryKey());
            //                }
            //                else {
            //                    table.Column(field.Name, dbType);
            //                }
            //            }
                       
            //        });
            //    }
            //}
            var configuration = _sessionFactoryHolder.GetConfiguration();
            new SchemaUpdate(configuration).Execute(false, true);

        }

        private IEnumerable<string> GetDatabaseTables()
        {
            var parameters = _sessionFactoryHolder.GetSessionFactoryParameters();
            var tabels = parameters.RecordDescriptors.Select(t => t.TableName).ToList();
            return tabels;

        }

        private static IEnumerable<string> GetDatabaseTables(ISession session, IPseudoProvider provider)
        {
            var query = session.CreateSQLQuery(provider.StoreSchemaTablesQuery);
            var reObjs = query.List();
            List<string> tables = new List<string>();
            foreach (var re in reObjs)
            {
                tables.Add(re.ToString());
            }
            return tables;
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
                get { return "SELECT  TABLE_NAME Name FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'"; }
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
                get { return "SELECT TABLE_NAME Name FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'TABLE'"; }
            }

            public bool SupportsSchemas
            {
                get { return false; }
            }
        }
    }
}