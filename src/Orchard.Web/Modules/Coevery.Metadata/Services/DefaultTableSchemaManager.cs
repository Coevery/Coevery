using System;
using System.Linq;
using System.Collections.Generic;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Helpers;
using FluentNHibernate.Conventions.Instances;
using NHibernate.Tool.hbm2ddl;
using Orchard.Data.Conventions;
using Orchard.Data;
using Orchard.Data.Providers;
using Orchard.Environment.ShellBuilders.Models;

namespace Coevery.Metadata.Services
{
    public class DefaultTableSchemaManager : ITableSchemaManager {
        private readonly ISessionFactoryHolder _sessionFactoryHolder;
        private readonly IDataServicesProviderFactory _dataServiceProviderFactory;

        public DefaultTableSchemaManager(
            ISessionFactoryHolder sessionFactoryHolder,
            IDataServicesProviderFactory dataServiceProviderFactory) {
            _sessionFactoryHolder = sessionFactoryHolder;
            _dataServiceProviderFactory = dataServiceProviderFactory;
        }

        public void UpdateSchema(IEnumerable<RecordBlueprint> recordBlueprints) {
            var persistenceModel = CreatePersistenceModel(recordBlueprints.ToList());
            var dataServiceProvider = this._dataServiceProviderFactory.CreateProvider(this._sessionFactoryHolder.GetSessionFactoryParameters());
            var persistenceConfigurer = dataServiceProvider.GetPersistenceConfigurer(true);


            var configuration = Fluently.Configure()
                                        .Database(persistenceConfigurer)
                                        .Mappings(m => m.AutoMappings.Add(persistenceModel))
                                        .ExposeConfiguration(c => { })
                                        .BuildConfiguration();

            new SchemaUpdate(configuration).Execute(false, true);

        }

        private AutoPersistenceModel CreatePersistenceModel(ICollection<RecordBlueprint> recordDescriptors) {
            if (recordDescriptors == null) {
                throw new ArgumentNullException("recordDescriptors");
            }

            return AutoMap.Source(new AbstractDataServicesProvider.TypeSource(recordDescriptors))
                          .Conventions.Setup(x => x.Add(AutoImport.Never()))
                          .Conventions.Add(new RecordTableNameConvention(recordDescriptors))
                          .Conventions.Add(new CacheConvention(recordDescriptors))
                          .Conventions.Add(new PrimaryKeyConvention())
                          .Alterations(alt => alt.Add(new IgnoreInfosetAlteration()));

        }
    }

    internal class IgnoreInfosetAlteration : IAutoMappingAlteration {
        public void Alter(AutoPersistenceModel model) {
            model.OverrideAll(alt => alt.IgnoreProperty("ContentItemRecord"));
            model.OverrideAll(alt => alt.IgnoreProperty("ContentItemVersionRecord"));
        }
    }

    internal class PrimaryKeyConvention : IIdConvention {
        public void Apply(IIdentityInstance instance) {
            instance.Column("Id");
            instance.GeneratedBy.Assigned();
        }
    }
}