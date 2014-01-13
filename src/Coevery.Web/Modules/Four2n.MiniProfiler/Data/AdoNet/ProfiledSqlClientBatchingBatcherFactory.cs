// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfiledSqlClientBatchingBatcherFactory.cs" company="Daniel Dabrowski - rod.42n.pl">
//   Copyright (c) 2008 Daniel Dabrowski - 42n. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProfiledSqlClientBatchingBatcherFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Four2n.Orchard.MiniProfiler.Data.AdoNet
{
    using NHibernate;
    using NHibernate.AdoNet;
    using NHibernate.Engine;

    public class ProfiledSqlClientBatchingBatcherFactory : SqlClientBatchingBatcherFactory
    {
        public override IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
        {
            return new ProfiledSqlClientBatchingBatcher(connectionManager, interceptor);
        }
    }
}