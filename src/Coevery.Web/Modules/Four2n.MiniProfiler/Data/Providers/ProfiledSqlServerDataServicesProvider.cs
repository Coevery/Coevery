// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfiledSqlServerDataServicesProvider.cs" company="Daniel Dabrowski - rod.42n.pl">
//   Copyright (c) 2008 Daniel Dabrowski - 42n. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProfiledSqlServerDataServicesProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Four2n.Orchard.MiniProfiler.Data.Providers
{
    using System.Diagnostics;

    using FluentNHibernate.Cfg.Db;

    using global::Coevery.Environment.Extensions;

    [CoeverySuppressDependency("Coevery.Data.Providers.SqlServerDataServicesProvider")]
    public class ProfiledSqlServerDataServicesProvider : global::Coevery.Data.Providers.SqlServerDataServicesProvider
    {
        public ProfiledSqlServerDataServicesProvider(string dataFolder, string connectionString)
            : base(dataFolder, connectionString)
        {
        }

        public static new string ProviderName
        {
            get { return global::Coevery.Data.Providers.SqlServerDataServicesProvider.ProviderName; }
        }

        public override IPersistenceConfigurer GetPersistenceConfigurer(bool createDatabase)
        {
            var persistence = (MsSqlConfiguration)base.GetPersistenceConfigurer(createDatabase);
            Debug.WriteLine("[Four2n.MiniProfiler] - ProfiledSqlServerDataServicesProvider - GetPersistenceConfigurer ");
            return persistence.Driver(typeof(ProfiledSqlClientDriver).AssemblyQualifiedName);
        }
    }
}