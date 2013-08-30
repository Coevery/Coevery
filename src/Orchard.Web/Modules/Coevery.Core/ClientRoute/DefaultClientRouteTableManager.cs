using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using Autofac.Features.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orchard.Caching;
using Orchard.Environment.Extensions.Models;
using Orchard.Logging;
using Orchard.Utility.Extensions;

namespace Coevery.Core.ClientRoute {
    public class DefaultClientRouteTableManager : IClientRouteTableManager {
        private readonly IEnumerable<Meta<IClientRouteProvider>> _clientRouteProviders;
        private readonly ICacheManager _cacheManager;
        private readonly IParallelCacheContext _parallelCacheContext;

        public DefaultClientRouteTableManager(
            IEnumerable<Meta<IClientRouteProvider>> clientRouteProviders,
            ICacheManager cacheManager,
            IParallelCacheContext parallelCacheContext) {
            _clientRouteProviders = clientRouteProviders;
            _cacheManager = cacheManager;
            _parallelCacheContext = parallelCacheContext;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public object GetRouteTable() {

            //return _cacheManager.Get<string, object>("ClientRouteTable", x => {
                Logger.Information("Start building shape table");

                var alterationSets = _parallelCacheContext.RunInParallel(_clientRouteProviders, provider => {
                    Feature feature = provider.Metadata.ContainsKey("Feature") ?
                                                         (Feature) provider.Metadata["Feature"] :
                                                         null;

                    var builder = new ClientRouteTableBuilder(feature);
                    provider.Value.Discover(builder);
                    return builder.BuildAlterations().ToReadOnlyCollection();
                });

                var alterations = alterationSets
                    .SelectMany(shapeAlterations => shapeAlterations)
                    .ToList();

                var routeNodes = alterations.GroupBy(alteration => alteration.RouteName, StringComparer.OrdinalIgnoreCase)
                                            .Select(group => new ClientRouteNode {Name = GetRouteName(group.Key), FullName = group.Key})
                                            .ToList();

                var rootNodes = routeNodes.Where(node => !node.FullName.Contains("."));
                PopulateChildren(routeNodes);

                var routes = GenerateRoutes(rootNodes, alterations);

                Logger.Information("Done building shape table");
                return routes;
            //});
        }

        private IDictionary<string, object> GenerateRoutes(IEnumerable<ClientRouteNode> rootNodes, List<ClientRouteAlteration> alterations) {
            IDictionary<string, object> routes = new ExpandoObject();
            foreach (var node in rootNodes) {
                var descriptor = new ClientRouteDescriptor {RouteName = node.Name};
                foreach (var alteration in alterations.Where(a => a.RouteName == node.FullName).ToList()) {
                    var feature = alteration.Feature;
                    alteration.Alter(descriptor);
                }

                dynamic route = new ExpandoObject();
                route.definition = ConvertRouteDescriptor(descriptor);
                if (node.Children != null && node.Children.Any())
                    route.children = GenerateRoutes(node.Children, alterations);
                routes[descriptor.RouteName] = route;
            }
            return routes;
        }

        private dynamic ConvertRouteDescriptor(ClientRouteDescriptor descriptor) {
            dynamic definition = new ExpandoObject();
            definition.url = descriptor.Url ?? string.Empty;
            if (descriptor.Abstract != null) {
                definition.@abstract = descriptor.Abstract;
            }

            IDictionary<string, object> views = new ExpandoObject();
            foreach (var viewDescriptor in descriptor.Views) {
                dynamic view = new ExpandoObject();
                if (viewDescriptor.TemplateUrl != null) {
                    view.templateUrl = new JRaw(viewDescriptor.TemplateUrl);
                }
                if (viewDescriptor.TemplateProvider != null) {
                    view.templateProvider = new JRaw(viewDescriptor.TemplateProvider);
                }
                if (viewDescriptor.Controller != null) {
                    view.controller = viewDescriptor.Controller;
                }
                views[viewDescriptor.Name ?? string.Empty] = view;
            }
            definition.views = views;


            if (descriptor.Dependencies != null) {
                definition.dependencies = descriptor.Dependencies;
            }
            return definition;
        }

        private string[] ToClientUrl(string baseUrl, IEnumerable<string> scripts) {
            if (scripts == null) return null;
            var results = scripts.Select(scriptPath => VirtualPathUtility.Combine(VirtualPathUtility.Combine(baseUrl, "Scripts/"), scriptPath + ".js"))
                .Select(VirtualPathUtility.ToAbsolute).ToArray();
            return results;
        }

        private static void PopulateChildren(List<ClientRouteNode> routeNodes) {
            foreach (var node in routeNodes) {
                var parent = node;
                var children = routeNodes.Where(n => n.FullName == parent.FullName + "." + n.Name).ToList();
                if (children.Any()) {
                    parent.Children = children;
                    PopulateChildren(parent.Children);
                }
            }
        }

        private string GetRouteName(string routeName) {
            int lastSegmentIndex = routeName.LastIndexOf('.');
            if (lastSegmentIndex > -1)
                return routeName.Substring(lastSegmentIndex + 1);
            return routeName;
        }

        private class ClientRouteNode {
            public string Name { get; set; }
            public string FullName { get; set; }
            public List<ClientRouteNode> Children { get; set; }
        }
    }

}