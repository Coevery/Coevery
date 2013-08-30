using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Linq;
using System.Web;
using Coevery.Core.DynamicTypeGeneration;
using FluentNHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Dialect.Schema;
using Orchard;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.Records;
using Orchard.Data;
using Orchard.Data.Migration.Interpreters;
using Orchard.Data.Migration.Schema;
using Orchard.Data.Providers;

namespace Coevery.Core.Services {
    public class CreateTableContext {
        private readonly IDynamicAssemblyBuilder _dynamicAssemblyBuilder;
        private readonly CreateTableCommand _tableCommand;

        public CreateTableContext(IDynamicAssemblyBuilder dynamicAssemblyBuilder,
            CreateTableCommand tableCommand) {
            _dynamicAssemblyBuilder = dynamicAssemblyBuilder;
            _tableCommand = tableCommand;
        }

        public void FieldColumn(string fieldName, string fieldTypeName, Action<CreateColumnCommand> column = null) {
            var type = _dynamicAssemblyBuilder.GetFieldType(fieldTypeName);
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>)) {
                type = Nullable.GetUnderlyingType(type);
            }
            var dbType = SchemaUtils.ToDbType(type);
            var command = new CreateColumnCommand(_tableCommand.Name, fieldName);
            command.WithType(dbType);

            if (column != null) {
                column(command);
            }
            _tableCommand.TableCommands.Add(command);
        }
    }

    public static class CreateTableCommandExteisons {}

    public interface ISchemaUpdateService : IDependency {
        void CreateTable(string tableName, Action<CreateTableContext> action = null);
        void CreateTable(string formatString, string tableName);
        void CreateColumn(string tableName, string columnName, string columnType);
        void DropColumn(string tableName, string columnName);
        void DropTable(string tableName);
        void DropTable(string formatString, string tableName);
    }

    public class SchemaUpdateService : ISchemaUpdateService {
        private readonly IDataMigrationInterpreter _interpreter;
        private readonly SchemaBuilder _schemaBuilder;
        private readonly IDynamicAssemblyBuilder _dynamicAssemblyBuilder;
        private readonly ISessionFactoryHolder _sessionFactoryHolder;
        private readonly string _tableFormat = "Coevery_DynamicTypes_{0}PartRecord";
        private readonly ISessionLocator _sessionLocator;

        public SchemaUpdateService(IDataMigrationInterpreter interpreter,
            IDynamicAssemblyBuilder dynamicAssemblyBuilder,
            ISessionFactoryHolder sessionFactoryHolder,
            ISessionLocator sessionLocator) {
            _interpreter = interpreter;
            _dynamicAssemblyBuilder = dynamicAssemblyBuilder;
            _sessionFactoryHolder = sessionFactoryHolder;
            _sessionLocator = sessionLocator;
            _schemaBuilder = new SchemaBuilder(_interpreter, "", s => s.Replace(".", "_"));
        }

        private bool CheckTableExists(string tableName) {
            var factory = _sessionFactoryHolder.GetSessionFactory();
            using (var session = factory.OpenSession()) {
                var connection = session.Connection;
                var dialect = Dialect.GetDialect(_sessionFactoryHolder.GetConfiguration().Properties);
                var meta = dialect.GetDataBaseSchema((DbConnection) connection);
                var tables = meta.GetTables(null, null, tableName, null);
                return tables.Rows.Count > 0;
            }
        }

        private bool CheckTableColumnExists(string tableName, string columnName) {
            var factory = _sessionFactoryHolder.GetSessionFactory();
            bool result = false;
            using (var session = factory.OpenSession()) {
                var connection = session.Connection;
                var dialect = Dialect.GetDialect(_sessionFactoryHolder.GetConfiguration().Properties);
                var meta = dialect.GetDataBaseSchema((DbConnection) connection);
                var tables = meta.GetTables(null, null, string.Format(_tableFormat, tableName), null);
                if (tables.Rows.Count > 0) {
                    var tableInfo = meta.GetTableMetadata(tables.Rows[0], true);
                    var columnInfo = tableInfo.GetColumnMetadata(columnName);
                    result = columnInfo != null;
                }
            }
            return result;
        }

        public void CreateTable(string tableName, Action<CreateTableContext> action = null) {
            Func<string, string> format = x => string.Format(_tableFormat, x);
            string formatedTableName = format(tableName);
            bool result = CheckTableExists(formatedTableName);
            if (result) {
                return;
            }
            _schemaBuilder.CreateTable(formatedTableName,
                table => {
                    table.Column<int>("Id", column => column.PrimaryKey())
                        .Column<int>("ContentItemRecord_id");
                    if (action != null) {
                        var context = new CreateTableContext(_dynamicAssemblyBuilder, table);
                        action(context);
                    }
                });
            GenerationDynmicAssembly();
        }

        public void CreateTable(string tableName, string formatString) {
            string formatedTableName = string.Format(formatString, tableName);
            bool result = CheckTableExists(formatedTableName);
            if (result) {
                return;
            }
            _schemaBuilder.CreateTable(formatedTableName,
                table => table.Column<int>("Id", column => column.PrimaryKey())
                    .Column<int>("PrimaryEntry_Id", column => column.NotNull())
                    .Column<int>("RelatedEntry_Id", column => column.NotNull()));
        }

        public void DropTable(string tableName) {
            string formatedTableName = string.Format(_tableFormat, tableName);
            bool result = CheckTableExists(formatedTableName);
            if (!result) {
                return;
            }
            _schemaBuilder.DropTable(formatedTableName);
            GenerationDynmicAssembly();
        }

        public void DropTable(string formatString, string tableName) {
            string formatedTableName = string.Format(formatString, tableName);
            var result = CheckTableExists(formatedTableName);
            if (!result) {
                return;
            }
            _schemaBuilder.DropTable(formatedTableName);
        }

        public void CreateColumn(string tableName, string columnName, string columnType) {
            string formatedTableName = string.Format(_tableFormat, tableName);
            bool result = CheckTableExists(formatedTableName);
            if (!result) {
                _schemaBuilder.CreateTable(formatedTableName,
                    table =>
                        table.Column<int>("Id", column => column.PrimaryKey())
                            .Column<int>("ContentItemRecord_id"));
            }
            else {
                result = CheckTableColumnExists(tableName, columnName);
            }
            if (result) {
                return;
            }
            var type = _dynamicAssemblyBuilder.GetFieldType(columnType);
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>)) {
                type = Nullable.GetUnderlyingType(type);
            }
            var dbType = SchemaUtils.ToDbType(type);
            _schemaBuilder.AlterTable(string.Format(_tableFormat, tableName),
                table => table.AddColumn(columnName, dbType));
            GenerationDynmicAssembly();
        }

        public void DropColumn(string tableName, string columnName) {
            bool result = CheckTableColumnExists(tableName, columnName);
            if (!result) {
                return;
            }
            _schemaBuilder.AlterTable(string.Format(_tableFormat, tableName), table => table.DropColumn(columnName));
            GenerationDynmicAssembly();
        }


        public void GenerationDynmicAssembly() {
            try {
                bool successful = _dynamicAssemblyBuilder.Build();
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}