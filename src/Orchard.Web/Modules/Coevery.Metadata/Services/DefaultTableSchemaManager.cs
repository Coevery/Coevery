using System;
using System.Linq;
using System.Collections.Generic;
using Coevery.Dynamic;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Orchard.Data.Migration.Interpreters;
using Orchard.Data.Migration.Schema;
using Orchard.Data;
using Orchard.Data.Providers;
using Orchard.Environment.ShellBuilders.Models;

namespace Coevery.Metadata.Services
{
    public class DefaultTableSchemaManager : ITableSchemaManager
    {
        private readonly IDataMigrationInterpreter _interpreter;
        private readonly SchemaBuilder _schemaBuilder;
        private readonly ISessionLocator _sessionLocator;
        private readonly ISessionFactoryHolder _sessionFactoryHolder;
        private readonly IDataServicesProviderFactory _dataServiceProviderFactory;

        public DefaultTableSchemaManager(IDataMigrationInterpreter interpreter,
            ISessionLocator sessionLocator,
             ISessionFactoryHolder sessionFactoryHolder,
            IDataServicesProviderFactory dataServiceProviderFactory)
        {
            _interpreter = interpreter;
            _sessionLocator = sessionLocator;
            _schemaBuilder = new SchemaBuilder(_interpreter);
            _sessionFactoryHolder = sessionFactoryHolder;
            _dataServiceProviderFactory = dataServiceProviderFactory;
        }

        public void UpdateSchema(Func<string, string> format, IEnumerable<Type> types)
        {

            
            var recordBlueprints = types.Select(t => new RecordBlueprint {TableName = format(t.Name), Type = t}).ToList();
            var persistenceModel = AbstractDataServicesProvider.CreatePersistenceModel(recordBlueprints);
            var dataServiceProvider = this._dataServiceProviderFactory.CreateProvider(this._sessionFactoryHolder.GetSessionFactoryParameters());
            var persistenceConfigurer = dataServiceProvider.GetPersistenceConfigurer(true);

            //var persistenceConfigurer = new 
            var configuration = Fluently.Configure()
                    .Database(persistenceConfigurer)
                    .Mappings(m => m.AutoMappings.Add(persistenceModel))
                    .ExposeConfiguration(c =>
                    {
                        // This is to work around what looks to be an issue in the NHibernate driver:
                        // When inserting a row with IDENTITY column, the "SELET @@IDENTITY" statement
                        // is issued as a separate command. By default, it is also issued in a separate
                        // connection, which is not supported (returns NULL).
                        //c.SetProperty("connection.release_mode", "on_close");
                        //new SchemaExport(c).Create(false /*script*/, true /*export*/);
                       // new SchemaUpdate(c).Execute(false, true);
                    })
                    .BuildConfiguration();
            
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
    }
}