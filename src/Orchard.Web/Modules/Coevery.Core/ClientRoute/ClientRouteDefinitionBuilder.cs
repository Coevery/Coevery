using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Core.ClientRoute {
    public class ClientRouteDefinitionBuilder {
        private readonly string _baseUrl;
        private readonly string _name;
        private string _url;
        private string _templateUrl;
        private string _templateProvider;
        private string _controller;
        private string[] _dependencies;
        private bool? _abstract;
        private readonly List<ClientRouteDefinitionBuilder> _children = new List<ClientRouteDefinitionBuilder>();

        public ClientRouteDefinitionBuilder(string name, string baseUrl) {
            _name = name;
            _baseUrl = baseUrl;
        }

        public ClientRouteDefinitionBuilder Url(string url) {
            _url = url;
            return this;
        }

        public ClientRouteDefinitionBuilder TemplateUrl(string templateUrl) {
            _templateUrl = templateUrl;
            return this;
        }

        public ClientRouteDefinitionBuilder TemplateProvider(string templateProvider) {
            _templateProvider = templateProvider;
            return this;
        }

        public ClientRouteDefinitionBuilder Controller(string controller) {
            _controller = controller;
            return this;
        }

        public ClientRouteDefinitionBuilder Dependencies(params string[] dependencies) {
            _dependencies = dependencies;
            return this;
        }

        public ClientRouteDefinitionBuilder Abstract(bool @abstract) {
            _abstract = @abstract;
            return this;
        }

        public ClientRouteDefinitionBuilder Children(string name, Action<ClientRouteDefinitionBuilder> action) {
            var buidler = new ClientRouteDefinitionBuilder(name,_baseUrl);
            action(buidler);
            _children.Add(buidler);
            return this;
        }

        public object Build() {

            dynamic route = new ExpandoObject();
            route.definition = BuildDefinition();

            if (_children.Any()) {
                route.children = BuildChildren(); ;
            }
            return route;
        }

        private IDictionary<string, object> BuildChildren() {
            IDictionary<string, Object> children = new ExpandoObject();
            _children.ForEach(buidler => { children[buidler._name] = buidler.Build(); });
            return children;
        }

        private dynamic BuildDefinition() {
            dynamic definition = new ExpandoObject();
            definition.url = _url;
            if (_abstract != null) {
                definition.@abstract = _abstract;
            }
            if (_templateUrl != null) {
                definition.templateUrl = new JRaw(_templateUrl);
            }
            if (_templateProvider != null) {
                definition.templateProvider = new JRaw(_templateProvider);
            }
            if (_controller != null) {
                definition.controller = _controller;
            }
            if (_dependencies != null) {
                definition.dependencies = _dependencies.Select(GetAbsolutePath).ToArray();
            }
            return definition;
        }

        private string GetAbsolutePath(string path) {
            string relativePath = VirtualPathUtility.Combine(VirtualPathUtility.Combine(_baseUrl, "Scripts/"), path+".js");
            return VirtualPathUtility.ToAbsolute(relativePath);
        }
    }
}