// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfiledSqlClientDriver.cs" company="Daniel Dabrowski - rod.42n.pl">
//   Copyright (c) 2008 Daniel Dabrowski - 42n. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProfiledSqlClientDriver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Four2n.Orchard.MiniProfiler.Data.Providers
{
    using System;
    using System.Data;
    using System.Data.Common;
    using AdoNet;
    using NHibernate.AdoNet;
    using NHibernate.Driver;

    public class ProfiledSqlClientDriver : SqlClientDriver, IEmbeddedBatcherFactoryProvider
    {
        Type IEmbeddedBatcherFactoryProvider.BatcherFactoryClass
        {
            get { return typeof(ProfiledSqlClientBatchingBatcherFactory); }
        }

        public override IDbCommand CreateCommand()
        {
            var command = base.CreateCommand();
            if (StackExchange.Profiling.MiniProfiler.Current != null)
            {
                command = new ProfiledSqlCommand(
                    (DbCommand)command,
                    StackExchange.Profiling.MiniProfiler.Current);
            }

            return command;
        }
    }
}