using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Threading;
using Coevery.Core.DynamicTypeGeneration;
using NHibernate.Dialect;
using Orchard.Data;
using Orchard.Data.Migration.Interpreters;
using Orchard.Data.Migration.Schema;
using Orchard.Environment.Configuration;
using Orchard.FileSystems.VirtualPath;
using Orchard.Logging;
using Orchard.Reports.Services;
using Orchard.Tasks.Scheduling;
using Orchard.Themes.Services;
namespace Coevery.Core.SiteReset
{
    public class SiteResetTaskExecutor : IScheduledTaskHandler
    {
        private readonly IThemeService _themeService;
        private readonly ISiteThemeService _siteThemeService;
        private readonly ITransactionManager _transactionManager;
        private readonly IVirtualPathProvider _virtualPathProvider;
        private readonly ISessionFactoryHolder _sessionFactoryHolder;
        private readonly SchemaBuilder _schemaBuilder;
        private readonly IDynamicAssemblyBuilder _dynamicAssemblyBuilder;
        public SiteResetTaskExecutor(
            IThemeService themeService, 
            ISiteThemeService siteThemeService,
            ISessionFactoryHolder sessionFactoryHolder,
            ITransactionManager transactionManager,
            IVirtualPathProvider virtualPathProvider,
            ShellSettings shellSettings,
            ISessionLocator sessionLocator,
            IEnumerable<ICommandInterpreter> commandInterpreters,
            IReportsCoordinator reportsCoordinator,
            IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            _themeService = themeService;
            _siteThemeService = siteThemeService;
            _transactionManager = transactionManager;
            _virtualPathProvider = virtualPathProvider;
            _sessionFactoryHolder = sessionFactoryHolder;
            var interpreter = new Coevery.Core.Services.DefaultDataMigrationInterpreter(shellSettings, sessionLocator, commandInterpreters, sessionFactoryHolder, reportsCoordinator);
            _schemaBuilder = new SchemaBuilder(interpreter, "", s => s.Replace(".", "_"));
            _dynamicAssemblyBuilder = dynamicAssemblyBuilder;
            Logger=NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void Process(ScheduledTaskContext context)
        {
            if (context.Task.TaskType == "ResetSite")
            {
                Logger.Information("start reseting site data at {2} utc", context.Task.ScheduledUtc);
                var factory = _sessionFactoryHolder.GetSessionFactory();
                using (var session = factory.OpenSession())
                {
                    var connection = session.Connection;
                    var dialect = Dialect.GetDialect(_sessionFactoryHolder.GetConfiguration().Properties);
                    var meta = dialect.GetDataBaseSchema((DbConnection)connection);
                    var tables = meta.GetTables(null, null, null, null);
                    _transactionManager.RequireNew();
                    try
                    {
                        foreach (DataRow dr in tables.Rows)
                        {
                            _schemaBuilder.DropTable(dr["TABLE_NAME"].ToString());
                        }
                        var sqlPath = _virtualPathProvider.MapPath("~/Modules/Coevery.Core/Sql/create.sql");
                        FileStream fs = new FileStream(sqlPath, FileMode.Open);
                        StreamReader sr = new StreamReader(fs);
                        var sql=sr.ReadToEnd();
                        _schemaBuilder.ExecuteSql(sql);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warning(ex, "drop table error");
                        _transactionManager.Cancel();
                    }
                }
                _dynamicAssemblyBuilder.Build();
                _themeService.EnableThemeFeatures("Mooncake");
                _siteThemeService.SetSiteTheme("Mooncake");
            }
        }
    }
}