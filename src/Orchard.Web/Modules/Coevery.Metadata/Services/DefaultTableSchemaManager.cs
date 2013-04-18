using System;
using System.Linq;
using System.Collections.Generic;
using Coevery.Dynamic;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Helpers;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;
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

        public void UpdateSchema(IEnumerable<RecordBlueprint> recordBlueprints)
        {
            var persistenceModel = CreatePersistenceModel(recordBlueprints.ToList());
            var dataServiceProvider = this._dataServiceProviderFactory.CreateProvider(this._sessionFactoryHolder.GetSessionFactoryParameters());
            var persistenceConfigurer = dataServiceProvider.GetPersistenceConfigurer(true);
            
            
            var configuration = Fluently.Configure()
                    .Database(persistenceConfigurer)
                    .Mappings(m => m.AutoMappings.Add(persistenceModel))
                    .ExposeConfiguration(c =>{})
                    .BuildConfiguration();
            
            new SchemaUpdate(configuration).Execute(false, true);

        }

        private  AutoPersistenceModel CreatePersistenceModel(ICollection<RecordBlueprint> recordDescriptors)
        {
            if (recordDescriptors == null)
            {
                throw new ArgumentNullException("recordDescriptors");
            }

            return AutoMap.Source(new AbstractDataServicesProvider.TypeSource(recordDescriptors))
                          .Conventions.Setup(x => x.Add(AutoImport.Never()))
                          .Conventions.Add(new RecordTableNameConvention(recordDescriptors))
                          .Conventions.Add(new CacheConvention(recordDescriptors))
                          .Conventions.Add(new PrimaryKeyConvention())
                          .Alterations(alt =>
                          {
                              alt.Add(new IgnoreInfosetAlteration(recordDescriptors));
                          });
         
        }
    }

     class IgnoreInfosetAlteration : IAutoMappingAlteration
    {
        private readonly IEnumerable<RecordBlueprint> _recordDescriptors;

        public IgnoreInfosetAlteration()
        {
            _recordDescriptors = Enumerable.Empty<RecordBlueprint>();
        }

        public IgnoreInfosetAlteration(IEnumerable<RecordBlueprint> recordDescriptors)
        {
            _recordDescriptors = recordDescriptors;
        }

        public void Alter(AutoPersistenceModel model)
        {
            model.OverrideAll(alt => alt.IgnoreProperty("ContentItemRecord"));
            model.OverrideAll(alt => alt.IgnoreProperty("ContentItemVersionRecord"));
        }
    }

     class PrimaryKeyConvention : IIdConvention
     {
         public void Apply(IIdentityInstance instance)
         {
             instance.Column("Id");
             instance.GeneratedBy.Assigned();
         }
     }
}