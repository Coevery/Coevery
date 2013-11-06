using System.Web;
using Coevery.Environment.Extensions.Models;

namespace Coevery.Mvc.ClientRoute
{
    public interface IClientRouteProvider : IDependency {
        Feature Feature { get; }
        bool IsFrontEnd { get; }
        void Discover(ClientRouteTableBuilder builder);
    }

    public abstract class ClientRouteProviderBase : IClientRouteProvider {
        public Feature Feature { get; set; }
        public bool IsFrontEnd { get; set; }

        public abstract void Discover(ClientRouteTableBuilder builder);

        protected string ToAbsoluteScriptUrl(string url) {
            var basePath = VirtualPathUtility.AppendTrailingSlash(Feature.Descriptor.Extension.Location + "/" + Feature.Descriptor.Extension.Id);
            if (url == null) return null;
            var virtualPath = VirtualPathUtility.Combine(VirtualPathUtility.Combine(basePath, "Scripts/"), url + ".js");
            return VirtualPathUtility.ToAbsolute(virtualPath);
        }

        protected string BasePath {
            get {
                var basePath = "~/Coevery/";
                if (!IsFrontEnd) basePath = "~/SystemAdmin/";
                var absolutePath = VirtualPathUtility.ToAbsolute(basePath);
                return absolutePath;
            }

        }

        protected string ModuleBasePath {
            get {
                return BasePath + Feature.Descriptor.Extension.Path + "/";
            }
        }
    }
}