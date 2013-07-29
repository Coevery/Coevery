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

        public  void CreateTable(string tableName)
        {
            //var dataServiceProvider = _dataServiceProviderFactory.CreateProvider(_sessionFactoryHolder.GetSessionFactoryParameters());
            //var persistenceConfigurer = dataServiceProvider.GetPersistenceConfigurer(true);
            //var factory = _sessionFactoryHolder.GetSessionFactory();
            //var connection =
            //factory.GetCurrentSession().Connection;
            //var metatTable = factory.GetClassMetadata(string.Format(tableFormat, "Lead"));
            //var d = Dialect.GetDialect(null);
            
            _schemaBuilder.CreateTable(string.Format(tableFormat, tableName), table => table.Column<int>("Id", column => column.PrimaryKey()));
            GenerationDynmicAssembly();
        }

        public void DropTable(string tableName) {
            _schemaBuilder.DropTable(string.Format(tableFormat, tableName));
            GenerationDynmicAssembly();
        }

        public  void CreateColumn(string tableName, string columnName, string columnType)
        {
            var dbType = SchemaUtils.ToDbType(_dynamicAssemblyBuilder.GetFieldType(columnType));
            _schemaBuilder.AlterTable(string.Format(tableFormat, tableName), table => table.AddColumn(columnName, dbType));
            GenerationDynmicAssembly();
        }

        public void DropColumn(string tableName, string columnName) {
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