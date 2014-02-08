using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coevery.Utility;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Coevery.Data
{
    public class SessionConfigurationEvents : ISessionConfigurationEvents
    {
        public void Created(FluentConfiguration cfg, AutoPersistenceModel defaultModel) {}

        public void Prepared(FluentConfiguration cfg) {}

        public void Building(Configuration cfg) {}

        public void Finished(Configuration cfg) {
            SchemaMetadataUpdater.QuoteTableAndColumns(cfg);
        }

        public void ComputingHash(Hash hash) {}
    }
}
