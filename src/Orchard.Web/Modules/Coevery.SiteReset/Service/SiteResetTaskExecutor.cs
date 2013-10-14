using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using Coevery.Core.DynamicTypeGeneration;
using NHibernate.Dialect;
using Orchard.Data;
using Orchard.Data.Migration.Interpreters;
using Orchard.Data.Migration.Schema;
using Orchard.Environment.Configuration;
using Orchard.FileSystems.VirtualPath;
using Orchard.Logging;
using Orchard.Reports.Services;
using Orchard.Services;
using Orchard.Tasks.Scheduling;
using Orchard.Themes.Services;

namespace Coevery.SiteReset.Service
{
    public class SiteResetTaskExecutor : IScheduledTaskHandler
    {
        private readonly IThemeService _themeService;
        private readonly ISiteThemeService _siteThemeService;
        private readonly IVirtualPathProvider _virtualPathProvider;
        private readonly ISessionFactoryHolder _sessionFactoryHolder;
        private readonly SchemaBuilder _schemaBuilder;
        private readonly IDynamicAssemblyBuilder _dynamicAssemblyBuilder;
        private readonly IScheduledTaskManager _scheduledTaskManager;
        private readonly IClock _clock;
        public SiteResetTaskExecutor(
            IThemeService themeService, 
            ISiteThemeService siteThemeService,
            ISessionFactoryHolder sessionFactoryHolder,
            IVirtualPathProvider virtualPathProvider,
            ShellSettings shellSettings,
            IEnumerable<ICommandInterpreter> commandInterpreters,
            IReportsCoordinator reportsCoordinator,
            IDynamicAssemblyBuilder dynamicAssemblyBuilder,
            IScheduledTaskManager scheduledTaskManager,
            IClock clock)
        {
            _themeService = themeService;
            _siteThemeService = siteThemeService;
            _virtualPathProvider = virtualPathProvider;
            _sessionFactoryHolder = sessionFactoryHolder;
            var interpreter = new Coevery.Core.Services.DefaultDataMigrationInterpreter(shellSettings, commandInterpreters, sessionFactoryHolder, reportsCoordinator);
            _schemaBuilder = new SchemaBuilder(interpreter, "", s => s.Replace(".", "_"));
            _dynamicAssemblyBuilder = dynamicAssemblyBuilder;
            _scheduledTaskManager = scheduledTaskManager;
            _clock = clock;
            Logger=NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void Process(ScheduledTaskContext context){
            if (context.Task.TaskType == "SwitchTheme")
            {
                _themeService.EnableThemeFeatures("Offline");
                _siteThemeService.SetSiteTheme("Offline");
                _scheduledTaskManager.CreateTask("ResetSite", _clock.UtcNow.AddSeconds(1), null);
            }
            else if (context.Task.TaskType == "ResetSite") {
                Logger.Information("start reseting site data at {2} utc", context.Task.ScheduledUtc);
                DataTable tables = null;
                var factory = _sessionFactoryHolder.GetSessionFactory();
                using (var session = factory.OpenSession()) {
                    var connection = session.Connection;
                    var dialect = Dialect.GetDialect(_sessionFactoryHolder.GetConfiguration().Properties);
                    var meta = dialect.GetDataBaseSchema((DbConnection) connection);
                    tables = meta.GetTables(null, null, null, null);
                }
                try {
                    foreach (DataRow dr in tables.Rows) {
                        if (dr["TABLE_NAME"].ToString().StartsWith("Coevery_DynamicTypes_"))
                            _schemaBuilder.DropTable(dr["TABLE_NAME"].ToString());
                    }
                    ExecuteOutSql("~/Modules/Coevery.SiteReset/Sql/create.sql");
                    _dynamicAssemblyBuilder.Build();
                }
                catch (Exception ex) {
                    Logger.Warning(ex, "reset site error");
                }
                finally {
                    _themeService.EnableThemeFeatures("Mooncake");
                    _siteThemeService.SetSiteTheme("Mooncake");
                }
            }
        }

        private void ExecuteOutSql(string path) {
            var sqlPath = _virtualPathProvider.MapPath(path);
            var fs = new FileStream(sqlPath, FileMode.Open);
            var sr = new StreamReader(fs);
            var sql = sr.ReadToEnd();
            _schemaBuilder.ExecuteSql(sql);
        }
    }
}