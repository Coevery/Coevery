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
using Orchard.Data;
using Orchard.Data.Migration.Interpreters;
using Orchard.Data.Migration.Schema;
using Orchard.Data.Providers;

namespace Coevery.Core.Services
{
    public interface ISchemaUpdateService:IDependency {
        void CreateTable(string tableName);
        void CreateColumn(string tableName, string columnName, string columnType);
        void DropColumn(string tableName, string columnName);
        void DropTable(string tableName);
    }

    public class SchemaUpdateService : ISchemaUpdateService
    {
        private readonly IDataMigrationInterpreter _interpreter;
        private readonly SchemaBuilder _schemaBuilder;
        private readonly IDynamicAssemblyBuilder _dynamicAssemblyBuilder;
        private readonly IDataServicesProviderFactory _dataServiceProviderFactory;
        private readonly ISessionFactoryHolder _sessionFactoryHolder;
        private string tableFormat = "Coevery_DynamicTypes_{0}PartRecord";
        public SchemaUpdateService(IDataMigrationInterpreter interpreter, 
            IDynamicAssemblyBuilder dynamicAssemblyBuilder, 
            IDataServicesProviderFactory dataServiceProviderFactory, 
            ISessionFactoryHolder sessionFactoryHolder)
        {
            _interpreter = interpreter;
            _dynamicAssemblyBuilder = dynamicAssemblyBuilder;
            _dataServiceProviderFactory = dataServiceProviderFactory;
            _sessionFactoryHolder = sessionFactoryHolder;
            _schemaBuilder = new SchemaBuilder(_interpreter, "", s => s.Replace(".", "_"));
        }

        private bool ExistsMetaData(string tableName,string columnName) {
            var factory = _sessionFactoryHolder.GetSessionFactory();
            var session = factory.OpenSession();
            var connection = session.Connection;
            var dialect = Dialect.GetDialect(_sessionFactoryHolder.GetConfiguration().Properties);
            var meta = dialect.GetDataBaseSchema((DbConnection)connection);
            var tables = meta.GetTables(null, null, string.Format(tableFormat, tableName), null);
            bool result = false;
            if (tables.Rows.Count > 0)
            {
                var tableInfo = meta.GetTableMetadata(tables.Rows[0], true);
                if (string.IsNullOrEmpty(columnName)) result = true;
                else {
                    var columnInfo = tableInfo.GetColumnMetadata(columnName);
                    result = columnInfo != null;
                }
            }
            session.Disconnect();
            session.Dispose();
            return result;
        }

        public  void CreateTable(string tableName) {
            bool result = ExistsMetaData(tableName, string.Empty);
            if (result) return;
            _schemaBuilder.CreateTable(string.Format(tableFormat, tableName), table => table.Column<int>("Id", column => column.PrimaryKey()));
            GenerationDynmicAssembly();
        }

        public void DropTable(string tableName) {
            bool result = ExistsMetaData(tableName, string.Empty);
            if (!result) return;
            _schemaBuilder.DropTable(string.Format(tableFormat, tableName));
            GenerationDynmicAssembly();
        }

        public  void CreateColumn(string tableName, string columnName, string columnType)
        {
            bool result = ExistsMetaData(tableName, string.Empty);
            if (!result) {

                _schemaBuilder.CreateTable(string.Format(tableFormat, tableName), 
                    table => 
                        table.Column<int>("Id", column => column.PrimaryKey()));
            }
            else {
                result = ExistsMetaData(tableName, columnName);
            }
            if (result) return;
            var type = _dynamicAssemblyBuilder.GetFieldType(columnType);
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GetGenericArguments()[0];
            }
            var dbType = SchemaUtils.ToDbType(type);
            _schemaBuilder.AlterTable(string.Format(tableFormat, tableName), 
                table => table.AddColumn(columnName, dbType));
            GenerationDynmicAssembly();
        }

        public void DropColumn(string tableName, string columnName) {
            bool result = ExistsMetaData(tableName, columnName);
            if (!result) return;
            _schemaBuilder.AlterTable(string.Format(tableFormat, tableName), table => table.DropColumn(columnName));
            GenerationDynmicAssembly();
        }


        public void GenerationDynmicAssembly()
        {
            try
            {
                bool successful = _dynamicAssemblyBuilder.Build();
            }
            catch (Exception ex) {
                throw ex;
            }
        }


    }
}