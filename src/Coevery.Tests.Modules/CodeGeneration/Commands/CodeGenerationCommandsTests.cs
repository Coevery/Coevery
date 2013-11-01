using System.Collections.Generic;
using System.IO;
using Autofac;
using Autofac.Features.Metadata;
using NUnit.Framework;
using Coevery.Caching;
using Coevery.CodeGeneration.Commands;
using Coevery.Commands;
using Coevery.Data;
using Coevery.Data.Migration.Generator;
using Coevery.Data.Providers;
using Coevery.Environment;
using Coevery.Environment.Configuration;
using Coevery.Environment.Extensions;
using Coevery.Environment.ShellBuilders;
using Coevery.Environment.ShellBuilders.Models;
using Coevery.FileSystems.AppData;
using Coevery.Tests.Environment;
using Coevery.Tests.FileSystems.AppData;
using Coevery.Tests.Stubs;

namespace Coevery.Tests.Modules.CodeGeneration.Commands {
    [TestFixture]
    public class CodeGenerationCommandsTests {

        private IContainer _container;
        private IExtensionManager _extensionManager;
        private ISchemaCommandGenerator _schemaCommandGenerator;

        [SetUp]
        public void Init() {
            string databaseFileName = Path.GetTempFileName();
            IDataServicesProviderFactory dataServicesProviderFactory = new DataServicesProviderFactory(new[] {
                new Meta<CreateDataServicesProvider>(
                    (dataFolder, connectionString) => new SqlCeDataServicesProvider(dataFolder, connectionString),
                    new Dictionary<string, object> {{"ProviderName", "SqlCe"}})
            });

            var builder = new ContainerBuilder();

            builder.RegisterInstance(new ShellBlueprint());
            builder.RegisterInstance(new ShellSettings { Name = ShellSettings.DefaultName, DataTablePrefix = "Test", DataProvider = "SqlCe" });
            builder.RegisterInstance(dataServicesProviderFactory).As<IDataServicesProviderFactory>();
            builder.RegisterInstance(AppDataFolderTests.CreateAppDataFolder(Path.GetDirectoryName(databaseFileName))).As<IAppDataFolder>();

            builder.RegisterType<SqlCeDataServicesProvider>().As<IDataServicesProvider>();
            builder.RegisterType<SessionConfigurationCache>().As<ISessionConfigurationCache>();
            builder.RegisterType<SessionFactoryHolder>().As<ISessionFactoryHolder>();
            builder.RegisterType<DefaultDatabaseCacheConfiguration>().As<IDatabaseCacheConfiguration>();
            builder.RegisterType<CompositionStrategy>().As<ICompositionStrategy>();
            builder.RegisterType<ExtensionManager>().As<IExtensionManager>();
            builder.RegisterType<SchemaCommandGenerator>().As<ISchemaCommandGenerator>();
            builder.RegisterType<StubCacheManager>().As<ICacheManager>();
            builder.RegisterType<StubParallelCacheContext>().As<IParallelCacheContext>();
            builder.RegisterType<StubAsyncTokenProvider>().As<IAsyncTokenProvider>();
            builder.RegisterType<StubHostEnvironment>().As<IHostEnvironment>();

            _container = builder.Build();
            _extensionManager = _container.Resolve<IExtensionManager>();
            _schemaCommandGenerator = _container.Resolve<ISchemaCommandGenerator>();
        }

        [Test]
        public void CreateDataMigrationTestNonExistentFeature() {
            CodeGenerationCommands codeGenerationCommands = new CodeGenerationCommands(_extensionManager,
                _schemaCommandGenerator);

            TextWriter textWriterOutput = new StringWriter();
            codeGenerationCommands.Context = new CommandContext { Output = textWriterOutput };
            codeGenerationCommands.CreateDataMigration("feature");

            Assert.That(textWriterOutput.ToString(), Is.StringContaining("Creating data migration failed"));
        }
    }
}