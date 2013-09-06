using System;
using System.Collections.Generic;
using System.Data.Common;
using Coevery.Core.DynamicTypeGeneration;
using NHibernate.Dialect;
using Orchard;
using Orchard.Data;
using Orchard.Data.Migration.Interpreters;
using Orchard.Data.Migration.Schema;
using Orchard.Environment.Configuration;
using Orchard.Reports.Services;

namespace Coevery.Core.Services {
    public class CreateTableContext {
        private readonly IDynamicAssemblyBuilder _dynamicAssemblyBuilder;
        private readonly CreateTableCommand _tableCommand;

        public CreateTableContext(
            IDynamicAssemblyBuilder dynamicAssemblyBuilder,
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
        void CreateColumn(string tableName, string columnName, string columnType);
        void DropColumn(string tableName, string columnName);
        void DropTable(string tableName);
        void CreateCustomTable(string tableName, Action<CreateTableCommand> table);
        void DropCustomTable(string tableName);
    }

    public class SchemaUpdateService : ISchemaUpdateService {
        private readonly SchemaBuilder _schemaBuilder;
        private readonly IDynamicAssemblyBuilder _dynamicAssemblyBuilder;
        private readonly ISessionFactoryHolder _sessionFactoryHolder;

        private const string TableFormat = "Coevery_DynamicTypes_{0}PartRecord";

        public SchemaUpdateService(
            IDynamicAssemblyBuilder dynamicAssemblyBuilder,
            ISessionFactoryHolder sessionFactoryHolder,
            ShellSettings shellSettings,
            ISessionLocator sessionLocator,
            IEnumerable<ICommandInterpreter> commandInterpreters,
            IReportsCoordinator reportsCoordinator) {
            _dynamicAssemblyBuilder = dynamicAssemblyBuilder;
            _sessionFactoryHolder = sessionFactoryHolder;
            var interpreter = new DefaultDataMigrationInterpreter(shellSettings, sessionLocator, commandInterpreters, sessionFactoryHolder, reportsCoordinator);
            _schemaBuilder = new SchemaBuilder(interpreter, "", s => s.Replace(".", "_"));
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
                var tables = meta.GetTables(null, null, tableName, null);
                if (tables.Rows.Count > 0) {
                    var tableInfo = meta.GetTableMetadata(tables.Rows[0], true);
                    var columnInfo = tableInfo.GetColumnMetadata(columnName);
                    result = columnInfo != null;
                }
            }
            return result;
        }

        public void CreateTable(string tableName, Action<CreateTableContext> action = null) {
            Func<string, string> format = x => string.Format(TableFormat, x);
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

        public void DropTable(string tableName) {
            string formatedTableName = string.Format(TableFormat, tableName);
            bool result = CheckTableExists(formatedTableName);
            if (!result) {
                return;
            }
            _schemaBuilder.DropTable(formatedTableName);
            GenerationDynmicAssembly();
        }

        public void CreateCustomTable(string tableName, Action<CreateTableCommand> table) {
            bool result = CheckTableExists(tableName);
            if (result) {
                return;
            }
            _schemaBuilder.CreateTable(tableName, table);
        }

        public void DropCustomTable(string tableName) {
            var result = CheckTableExists(tableName);
            if (!result) {
                return;
            }
            _schemaBuilder.DropTable(tableName);
        }

        public void CreateColumn(string tableName, string columnName, string columnType) {
            string formatedTableName = string.Format(TableFormat, tableName);
            bool result = CheckTableColumnExists(formatedTableName, columnName);
            if (result) {
                return;
            }
            var type = _dynamicAssemblyBuilder.GetFieldType(columnType);
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>)) {
                type = Nullable.GetUnderlyingType(type);
            }
            var dbType = SchemaUtils.ToDbType(type);
            _schemaBuilder.AlterTable(string.Format(TableFormat, tableName),
                table => table.AddColumn(columnName, dbType));
            GenerationDynmicAssembly();
        }

        public void DropColumn(string tableName, string columnName) {
            string formatedTableName = string.Format(TableFormat, tableName);
            bool result = CheckTableColumnExists(formatedTableName, columnName);
            if (!result) {
                return;
            }
            _schemaBuilder.AlterTable(formatedTableName, table => table.DropColumn(columnName));
            GenerationDynmicAssembly();
        }

        public void GenerationDynmicAssembly() {
            try {
                _dynamicAssemblyBuilder.Build();
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}