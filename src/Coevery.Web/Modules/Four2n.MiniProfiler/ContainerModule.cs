// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerModule.cs" company="Daniel Dabrowski - rod.42n.pl">
//   Copyright (c) 2008 Daniel Dabrowski - 42n. All rights reserved.
// </copyright>
// <summary>
//   Defines the ContainerModule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Four2n.Orchard.MiniProfiler
{
    using System;

    using Autofac;

    using Four2n.Orchard.MiniProfiler.Data;

    using global::Coevery.Environment;

    using StackExchange.Profiling;
    using StackExchange.Profiling.Storage;

    using Module = Autofac.Module;

    public class ContainerModule : Module
    {
        private readonly ICoeveryHost orchardHost;

        public ContainerModule(ICoeveryHost orchardHost)
        {
            this.orchardHost = orchardHost;
        }

        protected override void Load(ContainerBuilder moduleBuilder)
        {
            InitProfilerSettings();
            var currentLogger = ((DefaultCoeveryHost)this.orchardHost).Logger;
            if (currentLogger is OrchardHostProxyLogger)
            {
                return;
            }

            ((DefaultCoeveryHost)this.orchardHost).Logger = new OrchardHostProxyLogger(currentLogger);
        }

        private static void InitProfilerSettings()
        {
            MiniProfiler.Settings.SqlFormatter = new PoorMansTSqlFormatter();
            MiniProfiler.Settings.Storage = new HttpRuntimeCacheStorage(TimeSpan.FromHours(1));
            MiniProfiler.Settings.StackMaxLength = 500;
            MiniProfiler.Settings.ExcludeAssembly("MiniProfiler");
            MiniProfiler.Settings.ExcludeAssembly("NHibernate");
            WebRequestProfilerProvider.Settings.UserProvider = new IpAddressIdentity();
        }
    }
}