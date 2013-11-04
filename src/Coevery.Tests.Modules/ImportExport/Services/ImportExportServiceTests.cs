using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Autofac;
using Moq;
using NHibernate;
using NUnit.Framework;
using Coevery.Caching;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.ContentManagement.MetaData.Services;
using Coevery.ContentManagement.Records;
using Coevery.Core.Settings.Metadata;
using Coevery.Data;
using Coevery.Environment.Descriptor;
using Coevery.Environment.Descriptor.Models;
using Coevery.Environment.Extensions;
using Coevery.Environment.Extensions.Loaders;
using Coevery.FileSystems.AppData;
using Coevery.FileSystems.WebSite;
using Coevery.ImportExport.Services;
using Coevery.Recipes.Events;
using Coevery.Recipes.Services;
using Coevery.Services;
using Coevery.Tests.ContentManagement;
using Coevery.Tests.Environment.Extensions;
using Coevery.Tests.Modules.Recipes.Services;
using Coevery.Tests.Stubs;
using Coevery.Tests.UI.Navigation;

namespace Coevery.Tests.Modules.ImportExport.Services {
    [TestFixture]
    public class ImportExportManagerTests {
        private IContainer _container;
        private IImportExportService _importExportService;
        private ISessionFactory _sessionFactory;
        private ISession _session;

        [TestFixtureSetUp]
        public void InitFixture() {
            var databaseFileName = System.IO.Path.GetTempFileName();
            _sessionFactory = DataUtility.CreateSessionFactory(
                databaseFileName,
                typeof(ContentTypeRecord),
                typeof(ContentItemRecord),
                typeof(ContentItemVersionRecord));
        }

        [SetUp]
        public void Init() {
            var builder = new ContainerBuilder();
            builder.RegisterType<ImportExportService>().As<IImportExportService>();
            builder.RegisterType<StubShellDescriptorManager>().As<IShellDescriptorManager>();
            builder.RegisterType<RecipeManager>().As<IRecipeManager>();
            builder.RegisterType<RecipeHarvester>().As<IRecipeHarvester>();
            builder.RegisterType<RecipeStepExecutor>().As<IRecipeStepExecutor>();
            builder.RegisterType<StubStepQueue>().As<IRecipeStepQueue>().InstancePerLifetimeScope();
            builder.RegisterType<StubRecipeJournal>().As<IRecipeJournal>();
            builder.RegisterType<StubRecipeScheduler>().As<IRecipeScheduler>();
            builder.RegisterType<ExtensionManager>().As<IExtensionManager>();
            builder.RegisterType<StubAppDataFolder>().As<IAppDataFolder>();
            builder.RegisterType<StubClock>().As<IClock>();
            builder.RegisterType<StubCacheManager>().As<ICacheManager>();
            builder.RegisterType<ExtensionManagerTests.StubLoaders>().As<IExtensionLoader>();
            builder.RegisterType<RecipeParser>().As<IRecipeParser>();
            builder.RegisterType<StubWebSiteFolder>().As<IWebSiteFolder>();
            builder.RegisterType<CustomRecipeHandler>().As<IRecipeHandler>();
            builder.RegisterType<DefaultContentManager>().As<IContentManager>();
            builder.RegisterType<ContentDefinitionManager>().As<IContentDefinitionManager>();
            builder.RegisterType<ContentDefinitionWriter>().As<IContentDefinitionWriter>();
            builder.RegisterType<StubCoeveryServices>().As<ICoeveryServices>();
            builder.RegisterType<StubAppDataFolder>().As<IAppDataFolder>();
            builder.RegisterType<Signals>().As<ISignals>();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
            builder.RegisterInstance(new Mock<ISettingsFormatter>().Object);
            builder.RegisterInstance(new Mock<IRecipeExecuteEventHandler>().Object);
            _session = _sessionFactory.OpenSession();
            builder.RegisterInstance(new DefaultContentManagerTests.TestSessionLocator(_session)).As<ISessionLocator>();

            _container = builder.Build();
            _importExportService = _container.Resolve<IImportExportService>();
        }

        [Test]
        public void ImportSucceedsWhenRecipeContainsImportSteps() {
            Assert.DoesNotThrow(() => _importExportService.Import(
                                                                    @"<Coevery>
                                                                        <Recipe>
                                                                        <Name>MyModuleInstaller</Name>
                                                                        </Recipe>
                                                                        <Settings />
                                                                    </Coevery>"));
        }

        [Test]
        public void ImportDoesntFailsWhenRecipeContainsNonImportSteps() {
            Assert.DoesNotThrow(() => _importExportService.Import(
                                                                    @"<Coevery>
                                                                        <Recipe>
                                                                        <Name>MyModuleInstaller</Name>
                                                                        </Recipe>
                                                                        <Module name=""MyModule"" />
                                                                    </Coevery>"));
        }
    }

    public class StubShellDescriptorManager : IShellDescriptorManager {
        public ShellDescriptor GetShellDescriptor() {
            return new ShellDescriptor();
        }

        public void UpdateShellDescriptor(int priorSerialNumber, IEnumerable<ShellFeature> enabledFeatures, IEnumerable<ShellParameter> parameters) {
        }
    }
}