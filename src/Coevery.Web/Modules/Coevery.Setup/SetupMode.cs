using System;
using System.Collections.Generic;
using System.Web.Routing;
using Autofac;
using JetBrains.Annotations;
using Coevery.Caching;
using Coevery.Commands;
using Coevery.Commands.Builtin;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Handlers;
using Coevery.ContentManagement.MetaData.Builders;
using Coevery.Core.Settings.Models;
using Coevery.Data.Migration.Interpreters;
using Coevery.Data.Providers;
using Coevery.Data.Migration;
using Coevery.DisplayManagement;
using Coevery.DisplayManagement.Descriptors;
using Coevery.DisplayManagement.Descriptors.ShapeAttributeStrategy;
using Coevery.DisplayManagement.Descriptors.ShapeTemplateStrategy;
using Coevery.DisplayManagement.Implementation;
using Coevery.Environment;
using Coevery.Environment.Extensions.Models;
using Coevery.Localization;
using Coevery.Mvc;
using Coevery.Mvc.ModelBinders;
using Coevery.Mvc.Routes;
using Coevery.Mvc.ViewEngines;
using Coevery.Mvc.ViewEngines.Razor;
using Coevery.Mvc.ViewEngines.ThemeAwareness;
using Coevery.Recipes.Services;
using Coevery.Settings;
using Coevery.Tasks;
using Coevery.Themes;
using Coevery.UI.Notify;
using Coevery.UI.PageClass;
using Coevery.UI.PageTitle;
using Coevery.UI.Resources;
using Coevery.UI.Zones;
using IFilterProvider = Coevery.Mvc.Filters.IFilterProvider;

namespace Coevery.Setup {
    public class SetupMode : Module {
        public Feature Feature { get; set; }

        protected override void Load(ContainerBuilder builder) {

            // standard services needed in setup mode
            builder.RegisterModule(new MvcModule());
            builder.RegisterModule(new CommandModule());
            builder.RegisterModule(new WorkContextModule());
            builder.RegisterModule(new CacheModule());

            builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().InstancePerLifetimeScope();
            builder.RegisterType<ModelBinderPublisher>().As<IModelBinderPublisher>().InstancePerLifetimeScope();
            builder.RegisterType<RazorViewEngineProvider>().As<IViewEngineProvider>().As<IShapeTemplateViewEngine>().SingleInstance();
            builder.RegisterType<ThemedViewResultFilter>().As<IFilterProvider>().InstancePerLifetimeScope();
            builder.RegisterType<ThemeFilter>().As<IFilterProvider>().InstancePerLifetimeScope();
            builder.RegisterType<PageTitleBuilder>().As<IPageTitleBuilder>().InstancePerLifetimeScope();
            builder.RegisterType<PageClassBuilder>().As<IPageClassBuilder>().InstancePerLifetimeScope();
            builder.RegisterType<Notifier>().As<INotifier>().InstancePerLifetimeScope();
            builder.RegisterType<NotifyFilter>().As<IFilterProvider>().InstancePerLifetimeScope();
            builder.RegisterType<DataServicesProviderFactory>().As<IDataServicesProviderFactory>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultCommandManager>().As<ICommandManager>().InstancePerLifetimeScope();
            builder.RegisterType<HelpCommand>().As<ICommandHandler>().InstancePerLifetimeScope();
            //builder.RegisterType<WorkContextAccessor>().As<IWorkContextAccessor>().InstancePerMatchingLifetimeScope("shell");
            builder.RegisterType<ResourceManager>().As<IResourceManager>().InstancePerLifetimeScope();
            builder.RegisterType<ResourceFilter>().As<IFilterProvider>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultCoeveryShell>().As<ICoeveryShell>().InstancePerMatchingLifetimeScope("shell");
            builder.RegisterType<SweepGenerator>().As<ISweepGenerator>().SingleInstance();

            // setup mode specific implementations of needed service interfaces
            builder.RegisterType<SafeModeThemeService>().As<IThemeManager>().InstancePerLifetimeScope();
            builder.RegisterType<SafeModeText>().As<IText>().InstancePerLifetimeScope();
            builder.RegisterType<SafeModeSiteService>().As<ISiteService>().InstancePerLifetimeScope();

            builder.RegisterType<DefaultDataMigrationInterpreter>().As<IDataMigrationInterpreter>().InstancePerLifetimeScope();
            builder.RegisterType<DataMigrationManager>().As<IDataMigrationManager>().InstancePerLifetimeScope();

            builder.RegisterType<RecipeHarvester>().As<IRecipeHarvester>().InstancePerLifetimeScope();
            builder.RegisterType<RecipeParser>().As<IRecipeParser>().InstancePerLifetimeScope();

            builder.RegisterType<DefaultCacheHolder>().As<ICacheHolder>().SingleInstance();

            // in progress - adding services for display/shape support in setup
            builder.RegisterType<DisplayHelperFactory>().As<IDisplayHelperFactory>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultDisplayManager>().As<IDisplayManager>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultShapeFactory>().As<IShapeFactory>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultShapeTableManager>().As<IShapeTableManager>().InstancePerLifetimeScope();
            builder.RegisterType<ShapeTableLocator>().As<IShapeTableLocator>().InstancePerMatchingLifetimeScope("work");

            builder.RegisterType<ThemeAwareViewEngine>().As<IThemeAwareViewEngine>().InstancePerLifetimeScope();
            builder.RegisterType<LayoutAwareViewEngine>().As<ILayoutAwareViewEngine>().InstancePerLifetimeScope();
            builder.RegisterType<ConfiguredEnginesCache>().As<IConfiguredEnginesCache>().SingleInstance();
            builder.RegisterType<LayoutWorkContext>().As<IWorkContextStateProvider>().InstancePerLifetimeScope();
            builder.RegisterType<SafeModeSiteWorkContextProvider>().As<IWorkContextStateProvider>().InstancePerLifetimeScope();

            builder.RegisterType<ShapeTemplateBindingStrategy>().As<IShapeTableProvider>().InstancePerLifetimeScope();
            builder.RegisterType<BasicShapeTemplateHarvester>().As<IShapeTemplateHarvester>().InstancePerLifetimeScope();
            builder.RegisterType<ShapeAttributeBindingStrategy>().As<IShapeTableProvider>().InstancePerMatchingLifetimeScope("shell");
            builder.RegisterModule(new ShapeAttributeBindingModule());
        }


        [UsedImplicitly]
        class SafeModeText : IText {
            public LocalizedString Get(string textHint, params object[] args) {
                if (args == null || args.Length == 0) {
                    return new LocalizedString(textHint);
                }
                return new LocalizedString(string.Format(textHint, args));
            }
        }

        [UsedImplicitly]
        class SafeModeThemeService : IThemeManager {
            private readonly ExtensionDescriptor _theme = new ExtensionDescriptor {
                Id = "SafeMode",
                Name = "SafeMode",
                Location = "~/Themes",
            };

            public ExtensionDescriptor GetRequestTheme(RequestContext requestContext) { return _theme; }
        }

        [UsedImplicitly]
        class SafeModeSiteWorkContextProvider : IWorkContextStateProvider {
            public Func<WorkContext, T> Get<T>(string name) {
                if (name == "CurrentSite") {
                    ISite safeModeSite = new SafeModeSite();
                    return ctx => (T)safeModeSite;
                }
                return null;
            }
        }

        [UsedImplicitly]
        class SafeModeSiteService : ISiteService {
            public ISite GetSiteSettings() {
                var siteType = new ContentTypeDefinitionBuilder().Named("Site").Build();
                var site = new ContentItemBuilder(siteType)
                    .Weld<SafeModeSite>()
                    .Build();

                return site.As<ISite>();
            }
        }

        class SafeModeSite : ContentPart, ISite {
            public string PageTitleSeparator {
                get { return " - "; }
            }

            public string SiteName {
                get { return "Coevery Setup"; }
            }

            public string SiteSalt {
                get { return "42"; }
            }

            public string SiteUrl {
                get { return "/"; }
            }

            public string SuperUser {
                get { return ""; }
            }

            public string HomePage {
                get { return ""; }
                set { throw new NotImplementedException(); }
            }

            public string SiteCulture {
                get { return ""; }
                set { throw new NotImplementedException(); }
            }

            public ResourceDebugMode ResourceDebugMode {
                get { return ResourceDebugMode.FromAppSetting; }
                set { throw new NotImplementedException(); }
            }

            public int PageSize {
                get { return SiteSettingsPartRecord.DefaultPageSize; }
                set { throw new NotImplementedException(); }
            }

            public string BaseUrl {
                get { return ""; }
            }

            public string SiteTimeZone {
                get { return TimeZoneInfo.Local.Id; }
             }        
        }
    }
}
